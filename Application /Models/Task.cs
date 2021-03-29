using System;

namespace Application_1.Models
{
    // Task Model Class
    public class Task
    {
        // Parmeterised Constructor
        public Task(Double referenceFrequency, Double runtime, int ram)
        {
            ReferenceFrequency = referenceFrequency;
            Runtime = runtime;
            Ram = ram;
        }

        // Attributes
        public Double ReferenceFrequency { get; set; }
        public Double Runtime { get; set; }
        public int Ram { get; set; }

        // Check If Ram Is Sufficient
        public Boolean IsRamSufficient(Processor processor)
        {
            return Ram <= processor.Ram;
        }

        // Calculate Elapse Time
        public Double ElapseTime(Processor processor)
        {
            return ReferenceFrequency * Runtime / processor.Frequency;
        }

        // Calculate Processor Energy
        public Double ProcessorEnergy(Processor processor)
        {
            Double coefficientOneCal = processor.ProcSpec.CoefficientOne * processor.Frequency * processor.Frequency;
            Double coefficientTwoCal = processor.ProcSpec.CoefficientTwo * processor.Frequency;

            return coefficientOneCal - coefficientTwoCal + processor.ProcSpec.CoefficientThree;
        }

        // Calculate Task Energy Consumed
        public Double EnergyConsumed(Processor processor)
        {
            return ProcessorEnergy(processor) * ElapseTime(processor);
        }
    }
}
