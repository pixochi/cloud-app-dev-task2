using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskAllocationLibrary;

namespace GUI
{
    public static class Visualizer
    {
        private static string Allocation(TaskAllocationOutput allocOutput)
        {
            string output = $"Allocation ID = {allocOutput.AllocationId}, Time = {allocOutput.TimeConsumed}, Energy = {allocOutput.EnergyConsumed}";
            output += Environment.NewLine;
            foreach (var processor in allocOutput.Processors) {
                output += "P" + processor.Key + ": ";
                for (int taskIndex = 0; taskIndex < processor.Value.Count; taskIndex++) {
                    output += processor.Value.ElementAt(taskIndex);
                    output += taskIndex == (processor.Value.Count - 1) ? "" : ", ";
                }
                output += Environment.NewLine;
            }
            output += Environment.NewLine;
            output += "--------------------------------------";
            output += Environment.NewLine;
            output += Environment.NewLine;

            return output;
        }

        public static string Allocations(List<TaskAllocationOutput> allocations)
        {
            string output = "";

            foreach (var allocOutput in allocations) {
                output += Allocation(allocOutput);
            }

            return output;
        }
    }
}
