using System;
using System.Collections.Generic;
using System.Web;
using TaskAllocationLibrary;

namespace SortMidService.SortMidAlg
{
    // Based on:
    // https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4642166/
    public static class SortMidAlg
    {
        public static List<TaskAllocationOutput> GetAllocations(TaskAllocationInput allocInput)
        {
            List<TaskAllocationOutput> allocations = new List<TaskAllocationOutput>();
            Allocation newAllocation = new Allocation(allocInput.Tasks.Count, allocInput.Processors, allocInput.Coefficients);

            var grid = new Grid(allocInput.Tasks, allocInput.Processors, allocInput.RefFrequency);

            while (!newAllocation.IsDone) {
                TaskAssignment newTaskAssignment = new TaskAssignment(grid);
                var taskRuntime = grid.Content[newTaskAssignment.TaskId][newTaskAssignment.ProcessorId];
                newAllocation.AssignTaskToProcessor(newTaskAssignment.TaskId, newTaskAssignment.ProcessorId, taskRuntime);
                grid.UpdateProcessorRuntime(newTaskAssignment.ProcessorId, grid.GetRuntime(newTaskAssignment.TaskId, newTaskAssignment.ProcessorId));
                grid.RemoveRow(newTaskAssignment.TaskId);
            }

            var allocOutput = new TaskAllocationOutput((allocations.Count + 1).ToString(), 0, newAllocation.EnergyConsumed, newAllocation.Processors);
            allocations.Add(allocOutput);

            return allocations;
        }

    }
}