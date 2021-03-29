using Application_1.Models;
using Application_1.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Application_1
{
    public partial class AllocationsForm : Form
    {
        // Common Fields used across the Project.
        public static readonly string commentPrefix = "//";
        public static readonly char[] seperators = { '=', '"', ',' };
        private static readonly List<String> validationMessages = new List<string>();
        private static readonly List<String> errorMessages = new List<string>();
        private static readonly List<String> allocations = new List<String>();
        private static readonly List<string> configurations = new List<string>();
        private readonly OpenFileDialog openFileDialog;

        // Program Data
        private float programRuntime;
        private int numberOfTasks;
        private int numberOfProcessors;

        // Reference Frequency
        private float referenceFrequency;

        // List of Model Classes containing File Data.
        private readonly List<Task> tasks = new List<Task>();
        private readonly List<Processor> processors = new List<Processor>();
        private readonly List<Allocation> allocationsData = new List<Allocation>();

        public AllocationsForm()
        {
            InitializeComponent();

            // Configuring Open File Dialog to accept TAFF files.
            openFileDialog = new OpenFileDialog
            {
                Title = "Select Task Allocation File (TAFF)",
                DefaultExt = "taff",
                Filter = "Task Allocation (*.taff)|*.taff",
                RestoreDirectory = true
            };
        }

        #region Setters
        // Setter Methods for Global Fields.
        public static string ValidationMessage
        {
            set { validationMessages.Add(value); }
        }

        public static string Error
        {
            set { errorMessages.Add(value); }
        }

        public static string Allocation
        {
            set { allocations.Add(value); }
        }

        public static string Configuration
        {
            set { configurations.Add(value); }
        }
        #endregion

        #region Event Handlers
        // Exit Application Event Handler
        private void ExitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        // File - Open Event Handler
        private void OpenToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Reset Data
                ClearData();
                // Validate TAFF & CFF Files. If VALID then ENABLE VALIDATE - ALLOCATIONS
                TaskAllocation taskAllocation = new TaskAllocation(openFileDialog.FileName);
                if (taskAllocation.Validate())
                    allocationsToolStripMenuItem.Enabled = true;
                else
                    allocations.Clear();

                // Print Validation Messages.
                foreach (string message in validationMessages)
                    outputTextBox.Text += message + Environment.NewLine;

                // Print Error Messages.
                outputTextBox.Text += Environment.NewLine;
                foreach (string message in errorMessages)
                    outputTextBox.Text += message + Environment.NewLine;

                // Print Allocations.
                foreach (string allocation in allocations)
                    outputTextBox.Text += allocation + Environment.NewLine;
            }
        }

        // Validate Allocations Event Handler
        private void AllocationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Loop through Configuration Data & create TASK, PROCESSOR & PROCESSOR SPEC objects.
            foreach (string line in configurations)
            {
                string[] words = line.Split(seperators);
                // Set Program Data
                if (words.First() == "PROGRAM-DATA")
                {
                    programRuntime = (float)Double.Parse(words[1]);
                    numberOfTasks = Int32.Parse(words[2]);
                    numberOfProcessors = Int32.Parse(words[3]);
                }
                // Set Reference Frequency
                else if (words.First() == "REFERENCE-FREQUENCY")
                {
                    referenceFrequency = (float)Double.Parse(words[1]);
                }
                else if (words.First() == "TASK-RUNTIME-RAM")
                {
                    // Index of First Task's Runtime
                    int valueIndex = 1;
                    // Loop Number of Task Times.
                    for (int index = 0; index < numberOfTasks; index++)
                    {
                        Task task = new Task(referenceFrequency, Double.Parse(words[valueIndex]), Int32.Parse(words[valueIndex + 1]));
                        tasks.Add(task);
                        valueIndex += 2;
                    }
                }
                else if (words.First() == "PROCESSORS-FREQUENCIES-RAM")
                {
                    // Index of First Processor's Name
                    int valueIndex = 1;
                    // Loop Number of Processors Times
                    for (int index = 0; index < numberOfProcessors; index++)
                    {
                        Processor processor = new Processor(words[valueIndex], Double.Parse(words[valueIndex + 1]), Int32.Parse(words[valueIndex + 2]));
                        processors.Add(processor);
                        valueIndex += 3;
                    }
                }
                else if (words.First() == "PROCESSORS-COEFFICIENTS")
                {
                    // Index of First Processor Spec's Name
                    int valueIndex = 1;
                    // Loop Number of Processors Times
                    for (int index = 0; index < numberOfProcessors; index++)
                    {
                        ProcessorSpec spec = new ProcessorSpec(words[valueIndex], Double.Parse(words[valueIndex + 3]), Math.Abs(Double.Parse(words[valueIndex + 2])), Double.Parse(words[valueIndex + 1]));
                        // Loop Processors
                        foreach (Processor processor in processors)
                        {
                            // Find Correct Processor To Add Spec
                            if (processor.ProcessorName == spec.ProcessorName)
                                processor.ProcSpec = spec;
                        }
                        valueIndex += 4;
                    }
                }
            }

            // Allocation Model Object
            Allocation aAllocation = new Allocation(programRuntime);
            // Processor ID
            int processorId = 0;
            // Loop through Allocations Data to create Allocation objects.
            foreach (string line in allocations)
            {
                string[] words = line.Split(seperators);
                // Add Allocation Set Data to current Allocation.
                if (words.Length == numberOfTasks)
                {
                    List<int> taskIds = new List<int>();
                    for (int index = 0; index < words.Length; index++)
                    {
                        if (Int32.Parse(words[index]) == 1)
                            taskIds.Add(index);
                    }
                    aAllocation.addAllocationSet(processorId, taskIds.ToArray());
                    processorId++;
                }
                // Reset & Add New Allocation Object to List
                else
                {
                    aAllocation = new Allocation(programRuntime);
                    processorId = 0;
                    allocationsData.Add(aAllocation);
                }
            }

            // Print Allocation Energy
            outputTextBox.Text += Environment.NewLine;
            for (int index = 1; index <= allocationsData.Count; index++)
            {
                outputTextBox.Text += "Allocation ID - " + index + " - Energy = " + allocationsData[index - 1].computeAllocationEnergy(tasks, processors);
                outputTextBox.Text += Environment.NewLine;
            }
        }

        // About Form Event Handler
        private void AboutAllocationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.Show();
        }
        #endregion

        // Clear All Data (Reset)
        private void ClearData()
        {
            validationMessages.Clear();
            errorMessages.Clear();
            allocations.Clear();
            configurations.Clear();
            outputTextBox.Clear();
            allocationsToolStripMenuItem.Enabled = false;

            allocationsData.Clear();
            tasks.Clear();
            processors.Clear();
        }
    }
}
