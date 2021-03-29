using System;

namespace Application_1.Models
{
    // Processor Spec Model Class
    public class ProcessorSpec
    {
        // Parameterised Constructor
        public ProcessorSpec(string name, Double coefficientOne, Double coefficientTwo, Double coefficientThree)
        {
            ProcessorName = name;
            CoefficientOne = coefficientOne;
            CoefficientTwo = coefficientTwo;
            CoefficientThree = coefficientThree;
        }

        // Attributes
        public string ProcessorName { get; set; }
        public Double CoefficientOne { get; set; }
        public Double CoefficientTwo { get; set; }
        public Double CoefficientThree { get; set; }
    }
}
