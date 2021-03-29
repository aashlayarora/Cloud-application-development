using System;

namespace Application_1.Models
{
    // Processor Model Class
    public class Processor
    {
        // Parameterised Constructor
        public Processor(string name, Double frequency, int ram)
        {
            ProcessorName = name;
            Frequency = frequency;
            Ram = ram;
        }

        // Attributes
        public string ProcessorName { get; set; }
        public Double Frequency { get; set; }
        public int Ram { get; set; }
        public ProcessorSpec ProcSpec { get; set; }
    }
}
