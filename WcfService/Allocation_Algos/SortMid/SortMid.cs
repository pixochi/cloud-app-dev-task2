using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Collections;
using WcfService.Allocation_Algos.Shared;

namespace WcfService.Allocation_Algos.SortMid
{
    // Based on:
    // https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4642166/
    public static class SortMid
    {
        public static List<AllocOutput> GetAllocations(AllocInput allocInput)
        {
            List<AllocOutput> allocations = new List<AllocOutput>();
            Allocation newAllocation = new Allocation(allocInput.Tasks.Count, allocInput.Processors, allocInput.Coefficients);

            var grid = new Grid(allocInput.Tasks, allocInput.Processors, allocInput.RefFrequency);

            while (!newAllocation.IsDone) {
                TaskAssignment newTaskAssignment = new TaskAssignment(grid);
                var taskRuntime = grid.Content[newTaskAssignment.TaskId][newTaskAssignment.ProcessorId];
                newAllocation.AssignTaskToProcessor(newTaskAssignment.TaskId, newTaskAssignment.ProcessorId, taskRuntime);
                grid.UpdateProcessorRuntime(newTaskAssignment.ProcessorId, grid.GetRuntime(newTaskAssignment.TaskId, newTaskAssignment.ProcessorId));
                grid.RemoveRow(newTaskAssignment.TaskId);
            }

            var allocOutput = new AllocOutput((allocations.Count + 1).ToString(), 0, newAllocation.EnergyConsumed, newAllocation.Processors);
            allocations.Add(allocOutput);

            return allocations;
        }
    
    }

    public class TaskAssignment
    {
        private string taskId;
        private string processorId;
        private float maxMidValue;

        public TaskAssignment(Grid sortedGrid)
        {
            maxMidValue = 0;
            if (sortedGrid.Content.Count != 1) {
                foreach (var gridRow in sortedGrid.Content) {
                    var row = new GridRow(gridRow);
                    float rowMidValue = row.GetMidValue();

                    if (rowMidValue > maxMidValue) {
                        maxMidValue = rowMidValue;
                        taskId = row.TaskId;
                        processorId = row.GetFastestProcessorId();
                    }
                }
            }
            // Assign the last unallocated task to an available processor with the minimum completion time
            else {
                taskId = sortedGrid.Content.First().Key;
                processorId = sortedGrid.Content.First().Value.First().Key;
            }
        }

        public string TaskId { get => taskId; set => taskId = value; }
        public string ProcessorId { get => processorId; set => processorId = value; }
    }

    public class GridRow
    {
        private KeyValuePair<string, Dictionary<string, float>> row;
        private string taskId;

        public GridRow(KeyValuePair<string, Dictionary<string, float>> gridRow)
        {
            this.row = gridRow;
            this.taskId = gridRow.Key;
        }

        public KeyValuePair<string, Dictionary<string, float>> Row { get => row; set => row = value; }
        public string TaskId { get => taskId; set => taskId = value; }

        // MidValue is the average value of 2 consecutive completion times in the middle of the row
        public float GetMidValue()
        {
            int processorsCount = this.row.Value.Count;
            int midIndex = processorsCount / 2;
            var taskCompletionTimes = this.row.Value;
            float midTime1 = taskCompletionTimes.ElementAt(midIndex).Value;
            float midTime2 = taskCompletionTimes.ElementAt(midIndex + 1).Value;
            float midValue = (Convert.ToSingle(midTime1) + Convert.ToSingle(midTime2)) / 2;

            return midValue;
        }

        public string GetFastestProcessorId()
        {
            return this.row.Value.First().Key;
        }
    }


}