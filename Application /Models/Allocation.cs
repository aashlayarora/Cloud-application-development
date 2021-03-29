using System;
using System.Collections.Generic;

namespace Application_1.Models
{
    // Allocation Model Class
    public class Allocation
    {
        // Parameterised Constructor
        public Allocation(Double programRuntime)
        {
            ProgramRuntime = programRuntime;
            AllocationSet = new Dictionary<int, int[]>();
        }

        // Attributes
        public Double ProgramRuntime { get; set; }
        public Double AllocationEnergy { get; set; }
        public Dictionary<int, int[]> AllocationSet { get; set; }

        // Add Allocation Set
        public void addAllocationSet(int processorId, int[] taskIds)
        {
            AllocationSet.Add(processorId, taskIds);
        }

        // Calculated Allocation Energy
        public Double computeAllocationEnergy(List<Task> tasks, List<Processor> processors)
        {
            AllocationEnergy = 0;
            foreach (KeyValuePair<int, int[]> set in AllocationSet)
            {
                foreach (int taskId in set.Value)
                    AllocationEnergy += tasks[taskId].EnergyConsumed(processors[set.Key]);
            }

            return AllocationEnergy;
        }
    }
}
