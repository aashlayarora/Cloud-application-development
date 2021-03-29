using Application_1.Validations;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application_1.Process
{
    class TaskAllocation
    {
        // Task Allocation File Name
        private readonly string taskAllocationFileName;
        // Keyword Names
        private string configurationFileName;
        private bool isAllocationDataPresent = false;
        // Regular Expressions
        private static readonly string[] keywordNames = { "CONFIG-FILE", "ALLOCATIONS-DATA", "ALLOCATION-ID" };
        private static readonly Regex configFileRegex = new Regex(@"^CONFIG-FILE="".+.cff""$");
        private static readonly Regex allocationDataRegex = new Regex(@"^ALLOCATIONS-DATA=[1-9]+\d*,[1-9]+\d*,[1-9]\d*$");
        private static readonly Regex allocationIdRegex = new Regex(@"^ALLOCATION-ID=[1-9]+\d*$");
        private static readonly Regex allocationSetRegex = new Regex(@"^[0-1](,[0,1])*$");
        private readonly Validation validation;

        // Local Allocation Validation Variables
        private int numberOfAllocations;
        private int numberOfTasks;
        private int numberOfProcessors;
        private int numberOfAllocatedProcessors = 0;
        private bool processorCheckStart = false;
        private bool[] allocationTasks;
        private bool taskCheckStart = false;

        // Parameterised Constructor
        public TaskAllocation(string fileName)
        {
            this.taskAllocationFileName = fileName;
            validation = new Validation();
        }

        // Check If File is Valid
        public bool Validate()
        {
            bool validFile = true;

            try
            {
                // Read File
                StreamReader streamReader = new StreamReader(this.taskAllocationFileName);
                // Loop Each Line
                while (streamReader.Peek() >= 0)
                {
                    string line = streamReader.ReadLine().Trim();
                    // Validate Through Each Condition in Correct Order
                    bool isValidLine = validation.ValidateLineIsComment(line) || ValidateLineIsData(line) || ValidateLineIsAllocationSet(line) || validation.ValidateLineIsEmpty(line);

                    // Check If All Tasks Are Allocated in Allocation Set
                    if (taskCheckStart && validation.ValidateLineIsEmpty(line))
                    {
                        taskCheckStart = false;
                        // If INVALID then Add Line Error.
                        if (allocationTasks.Contains(false))
                        {
                            validFile = false;
                            AllocationsForm.Error = "Invalid Allocation Set - Task Not Allocated = " + (Array.IndexOf(allocationTasks, false) + 1);
                        }
                    }

                    // Check If Allocation Set Contains Correct Number of Processors
                    if (processorCheckStart && validation.ValidateLineIsEmpty(line))
                    {
                        processorCheckStart = false;
                        // If INVALID then Add Line Error.
                        if (numberOfAllocatedProcessors != numberOfProcessors)
                        {
                            validFile = false;
                            AllocationsForm.Error = "Invalid Allocation Set - Number Of Processors = " + numberOfAllocatedProcessors + " / " + numberOfProcessors;
                        }
                        numberOfAllocatedProcessors = 0;
                    }

                    // If INVALID then Add Line Error.
                    if (!isValidLine)
                    {
                        validFile = isValidLine;
                        AllocationsForm.Error = "Invalid Line - " + line;
                    }
                }
                streamReader.Close();

                if (validFile)
                    AllocationsForm.ValidationMessage = "Task Allocation File - VALID";
                else
                    AllocationsForm.ValidationMessage = "Task Allocation File - INVALID";

                // Validate Configuration File
                Configuration configuration = new Configuration(configurationFileName);
                validFile = configuration.Validate() && validFile;
            }
            // Invalid File
            catch (Exception ex)
            {
                AllocationsForm.ValidationMessage = "Task Allocation File - INVALID";
                AllocationsForm.Error = ex.Message;
                validFile = false;
            }

            return validFile;
        }

        // Validating File is Data (Keyword) & Processing Accordingly
        private bool ValidateLineIsData(string line)
        {
            bool validDataLine = false;
            string[] words = line.Split(AllocationsForm.seperators);
            // Check If First Word is a Keyword
            if (keywordNames.Contains(words.First()) && !words.Last().Contains(AllocationsForm.commentPrefix))
            {
                // Get Configuration File Name
                if (words.First().Equals(keywordNames.First()) && String.IsNullOrEmpty(configurationFileName))
                {
                    validDataLine = configFileRegex.IsMatch(line);
                    configurationFileName = words[2];
                }
                // Get Allocation Data Details
                else if (allocationDataRegex.IsMatch(line) && !isAllocationDataPresent)
                {
                    numberOfAllocations = Int32.Parse(words[1]);
                    numberOfTasks = Int32.Parse(words[2]);
                    numberOfProcessors = Int32.Parse(words[3]);
                    validDataLine = isAllocationDataPresent = true;
                }
                // Check If Allocation ID Keyword
                else if (allocationIdRegex.IsMatch(line))
                {
                    // Add For Later Use.
                    AllocationsForm.Allocation = Environment.NewLine + line;
                    // Create [false] Array of Number of Tasks
                    allocationTasks = Enumerable.Repeat(false, numberOfTasks).ToArray();
                    validDataLine = Int32.Parse(words.Last()) <= numberOfAllocations;
                }
                else
                    allocationTasks = Enumerable.Repeat(false, numberOfTasks).ToArray();
            }

            return validDataLine;
        }

        // Validating File is Allocation Set & Processing Accordingly
        private bool ValidateLineIsAllocationSet(string line)
        {
            bool validAllocationSet = false;
            string[] words = line.Split(AllocationsForm.seperators);
            // Check If Data is 0s & 1s Seperated by ','
            if (!words.Last().Contains(AllocationsForm.commentPrefix) && allocationSetRegex.IsMatch(line))
            {
                // Add For Later Use
                AllocationsForm.Allocation = line;
                processorCheckStart = true;
                numberOfAllocatedProcessors++;
                // Check If Correct Lenght Set
                if (words.Length == numberOfTasks)
                {
                    validAllocationSet = taskCheckStart = true;
                    // Loop Each Task & Check If Previously Allocated to a Processor.
                    for (int index = 0; index < words.Length; index++)
                    {
                        if (words[index].Equals("1"))
                        {
                            if (!allocationTasks[index])
                                validAllocationSet = allocationTasks[index] = true;
                            else
                                validAllocationSet = false;
                        }
                    }
                }
            }

            return validAllocationSet;
        }
    }
}
