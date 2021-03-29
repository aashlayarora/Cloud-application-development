using Application_1.Validations;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application_1.Process
{
    class Configuration
    {
        // Configuration File Name
        private readonly string configurationFileName;
        // Keyword Names
        private static readonly string[] keywordRegexNames = { "DEFAULT-LOGFILE", "LIMITS-TASKS", "LIMITS-PROCESSORS", "LIMITS-PROCESSOR-FREQUENCIES", "LIMITS-RAM", "PROGRAM-DATA", "REFERENCE-FREQUENCY" };
        private static readonly string[] keywordNames = { "TASK-RUNTIME-RAM", "PROCESSORS-FREQUENCIES-RAM", "PROCESSORS-COEFFICIENTS", "LOCAL-COMMUNICATION", "REMOTE-COMMUNICATION" };
        // Regular Expressions
        private static readonly Regex logFileRegex = new Regex(@"^DEFAULT-LOGFILE="".+.txt""$");
        private static readonly Regex limitsRegex = new Regex(@"^(LIMITS-TASKS|LIMITS-PROCESSORS|LIMITS-RAM)=[1-9]+\d*,[1-9]+\d*$");
        private static readonly Regex limitsFrequencyRegex = new Regex(@"^LIMITS-PROCESSOR-FREQUENCIES=\d+\.\d{1,2},\d+\.\d{1,2}$");
        private static readonly Regex programDataRegex = new Regex(@"^PROGRAM-DATA=(\d+|\d+\.\d{1,2}),[1-9]+\d*,[1-9]+\d*$");
        private static readonly Regex referenceFrequencyRegex = new Regex(@"^REFERENCE-FREQUENCY=\d+\.\d{1,2}$");
        private static readonly Regex taskRuntimeRamRegex = new Regex(@"^TASK-RUNTIME-RAM=(\d+|\d+\.\d{1,2}),[1-9]+\d*(,(\d+|\d+\.\d{1,2}),[1-9]+\d*)*$");
        private static readonly Regex processorFreqRamRegex = new Regex(@"^PROCESSORS-FREQUENCIES-RAM=[A-Za-z0-9_]+[A-Za-z0-9 _]*,(\d+|\d+\.\d{1,2}),[1-9]+\d*(,([A-Za-z0-9_]+[A-Za-z0-9 _]*,(\d+|\d+\.\d{1,2}),[1-9]+\d*))*$");
        private static readonly Regex processorCoefRegex = new Regex(@"^PROCESSORS-COEFFICIENTS=[A-Za-z0-9_]+[A-Za-z0-9 _]*,(\d+|\d+\.\d{1,2}),-(\d+|\d+\.\d{1,2}),(\d+|\d+\.\d{1,2})(,([A-Za-z0-9_]+[A-Za-z0-9 _]*,(\d+|\d+\.\d{1,2}),-(\d+|\d+\.\d{1,2}),(\d+|\d+\.\d{1,2})))*$");
        private static readonly Regex communicationRegex = new Regex(@"^(\d+|\d+\.\d+)(,(\d+|\d+\.\d+))*$");
        private readonly Validation validation;

        // Parameterised Constructor
        public Configuration(string fileName)
        {
            this.configurationFileName = fileName;
            validation = new Validation();
        }

        // Check If File is Valid
        public bool Validate()
        {
            bool validFile = true;

            try
            {
                // Read File
                StreamReader streamReader = new StreamReader(this.configurationFileName);
                // Loop Each Line
                while (streamReader.Peek() >= 0)
                {
                    string line = streamReader.ReadLine().Trim();
                    // Validate Through Each Condition in Correct Order
                    bool isValidLine = validation.ValidateLineIsComment(line) || ValidateLineIsData(line) || ValidateLineIsCommunication(line) || validation.ValidateLineIsEmpty(line);

                    // If INVALID then Add Line Error.
                    if (!isValidLine)
                    {
                        validFile = isValidLine;
                        AllocationsForm.Error = "Invalid Line - " + line;
                    }
                }

                if (validFile)
                    AllocationsForm.ValidationMessage = "Configuration File - VALID";
                else
                    AllocationsForm.ValidationMessage = "Configuration File - INVALID";

                streamReader.Close();
            }
            // Invalid File
            catch (Exception ex)
            {
                AllocationsForm.ValidationMessage = "Configuration File - INVALID";
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
            // Check If Starting Word is a Keyword
            if (keywordRegexNames.Contains(words.First()) && !words.Last().Contains(AllocationsForm.commentPrefix))
            {
                validDataLine = logFileRegex.IsMatch(line) || limitsRegex.IsMatch(line) || limitsFrequencyRegex.IsMatch(line);
                // Add Program Data & Reference Frequency for Later Use.
                if (programDataRegex.IsMatch(line) || referenceFrequencyRegex.IsMatch(line))
                {
                    AllocationsForm.Configuration = line;
                    validDataLine = true;
                }
            }
            else if (keywordNames.Contains(words.First()) && !words.Last().Contains(AllocationsForm.commentPrefix))
            {
                validDataLine = true;
                // Add Task, Processor & Spec Data for Later Use.
                if (taskRuntimeRamRegex.IsMatch(line) || processorFreqRamRegex.IsMatch(line) || processorCoefRegex.IsMatch(line))
                    AllocationsForm.Configuration = line;
            }

            return validDataLine;
        }

        // Validating Line is Communication Data.
        private bool ValidateLineIsCommunication(string line)
        {
            string[] words = line.Split(AllocationsForm.seperators);
            return !words.Last().Contains(AllocationsForm.commentPrefix) && communicationRegex.IsMatch(line);
        }
    }
}
