using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Application_1.Models;
using System.Collections.Generic;

namespace Application_1_UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TaskRamIsSufficient()
        {
            // Arrange.
            Double referenceFrequency = 2.0;
            Double runtime = 3.0;
            int ram = 2;
            Task task = new Task(referenceFrequency, runtime, ram);

            string name = "Intel i5";
            Processor processor = new Processor(name, referenceFrequency, ram);

            // Act.
            bool isRamSufficient = task.IsRamSufficient(processor);

            // Assert.
            Assert.AreEqual(true, isRamSufficient, "Task RAM is sufficient with respect to Processor.");
        }

        [TestMethod]
        public void TaskElapseTime()
        {
            // Arrange.
            Double referenceFrequency = 2.0;
            Double runtime = 3.0;
            int ram = 2;
            Task task = new Task(referenceFrequency, runtime, ram);

            string name = "Intel i5";
            Double frequency = 1.8;
            Processor processor = new Processor(name, frequency, ram);

            Double expectedElapseTime = 3.333333;

            // Act.
            Double elapseTime = task.ElapseTime(processor);

            // Assert.
            Assert.AreEqual(expectedElapseTime, elapseTime, 0.0001, "Correct Expected Elapse Time.");
        }

        [TestMethod]
        public void TaskProcessorEnergy()
        {
            // Arrange.
            Double referenceFrequency = 2.0;
            Double runtime = 3.0;
            int ram = 2;
            Task task = new Task(referenceFrequency, runtime, ram);

            string name = "Intel i5";
            Double frequency = 1.8;
            Processor processor = new Processor(name, frequency, ram);

            Double coefficientOne = 10;
            Double coefficientTwo = 25;
            Double coefficientThree = 24;
            ProcessorSpec spec = new ProcessorSpec(name, coefficientOne, coefficientTwo, coefficientThree);
            processor.ProcSpec = spec;

            Double expectedProcessorEnergy = 11.4;

            // Act.
            Double processorEnergy = task.ProcessorEnergy(processor);

            // Assert.
            Assert.AreEqual(expectedProcessorEnergy, processorEnergy, 0.0001, "Correct Processor Energy.");
        }

        [TestMethod]
        public void TaskEnergyConsumed()
        {
            // Arrange.
            Double referenceFrequency = 2.0;
            Double runtime = 3.0;
            int ram = 2;
            Task task = new Task(referenceFrequency, runtime, ram);

            string name = "Intel i5";
            Double frequency = 1.8;
            Processor processor = new Processor(name, frequency, ram);

            Double coefficientOne = 10;
            Double coefficientTwo = 25;
            Double coefficientThree = 24;
            ProcessorSpec spec = new ProcessorSpec(name, coefficientOne, coefficientTwo, coefficientThree);
            processor.ProcSpec = spec;

            Double expectedEnergyConsumed = 37.99999;

            // Act.
            Double taskEnergy = task.EnergyConsumed(processor);

            // Assert.
            Assert.AreEqual(expectedEnergyConsumed, taskEnergy, 0.0001, "Correct Energy Consumed.");
        }

        [TestMethod]
        public void AllocationEnergyConsumed()
        {
            // Arrange.
            Double referenceFrequency = 2.0;
            Double runtime = 3.0;
            int ram = 2;
            Task task = new Task(referenceFrequency, runtime, ram);

            string name = "Intel i5";
            Double frequency = 1.8;
            Processor processor = new Processor(name, frequency, ram);

            Double coefficientOne = 10;
            Double coefficientTwo = 25;
            Double coefficientThree = 24;
            ProcessorSpec spec = new ProcessorSpec(name, coefficientOne, coefficientTwo, coefficientThree);
            processor.ProcSpec = spec;

            List<Task> tasks = new List<Task>();
            tasks.Add(task);
            List<Processor> processors = new List<Processor>();
            processors.Add(processor);
            Allocation allocation = new Allocation(referenceFrequency);
            int[] taskIds = { 0 };
            allocation.addAllocationSet(0, taskIds);

            Double expectedEnergyConsumed = 37.99999;

            // Act.
            Double allocationEnergy = allocation.computeAllocationEnergy(tasks, processors);

            // Assert.
            Assert.AreEqual(expectedEnergyConsumed, allocationEnergy, 0.0001, "Correct Allocation Energy Consumed.");
        }
    }
}
