using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Collections;

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

    public class Allocation
    {
        private Dictionary<string, List<string>> processors;
        private Dictionary<string, float> processorFrequencies;
        private int tasksCount;
        private int assignedTasksCount;
        private float energyConsumed;
        private List<float> coefficients;

        public Allocation(int tasksCount, Dictionary<string, float> processorFrequencies, List<float> coefficients)
        {
            this.processors = new Dictionary<string, List<string>>();
            this.tasksCount = tasksCount;
            this.coefficients = coefficients;
            this.processorFrequencies = processorFrequencies;
            this.assignedTasksCount = 0;
            this.energyConsumed = 0;
        }

        public int TasksCount { get => tasksCount; set => tasksCount = value; }

        public bool IsDone { get => tasksCount == assignedTasksCount; }
        public Dictionary<string, List<string>> Processors { get => processors; set => processors = value; }
        public float EnergyConsumed { get => energyConsumed; set => energyConsumed = value; }

        public void AssignTaskToProcessor(string taskId, string processorId, float runtime)
        {
            List<string> taskIds;
            assignedTasksCount = assignedTasksCount + 1;
            this.energyConsumed += AllocationHelper.GetEnergyConsumedPerTask(this.coefficients, processorFrequencies[processorId], runtime);

            if (this.processors.ContainsKey(processorId)) {
                taskIds = this.processors[processorId];
                taskIds.Add(taskId);
            }
            else {
                taskIds = new List<string>() { taskId };
            }

            if (this.processors.ContainsKey(processorId)) {
                this.processors[processorId] = taskIds;
            }
            else {
                this.processors.Add(processorId, taskIds);
            }
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

    public class Grid
    {
        private Dictionary<string, Dictionary<string, float>> content;

        public Grid(Dictionary<string, float> tasks, Dictionary<string, float> processors, float refFrequency)
        {
            // {[taskId: string]: {
            //    [processorId: string]: taskRuntimeOnThatProcessor // in ASCENDING order
            // }
            Dictionary<string, Dictionary<string, float>> grid = new Dictionary<string, Dictionary<string, float>>();

            foreach (var task in tasks) {
                Dictionary<string, float> gridRow = new Dictionary<string, float>();

                foreach (var processor in processors) {
                    float taskRuntime = task.Value * (refFrequency / processor.Value);
                    gridRow.Add(processor.Key, taskRuntime);
                }

                grid.Add(task.Key, gridRow);
            }

            this.content = grid;
            this.sortGrid();
        }

        public void RemoveRow(string taskId)
        {
            this.content.Remove(taskId);
        }

        public float GetRuntime(string taskId, string processorId)
        {
            return this.content[taskId][processorId];
        }

        private void sortGrid()
        {
            Dictionary<string, Dictionary<string, float>> sortedGrid = new Dictionary<string, Dictionary<string, float>>();

            foreach (var taskRow in this.content) {
                Dictionary<string, float> sortedRowByCompletionTime = taskRow.Value.OrderBy(r => r.Value)
                      .ToDictionary(c => c.Key as string, d => Convert.ToSingle(d.Value));

                sortedGrid.Add(taskRow.Key, sortedRowByCompletionTime);
            }

            this.content = sortedGrid;
        }

        public void UpdateProcessorRuntime(string processorId, float increaseBy)
        {
            var tmp = new Dictionary<string, Dictionary< string, float>>(this.content);
            foreach (var gridRow in tmp) {
                var updatedRowValue = gridRow.Value;
                updatedRowValue[processorId] += increaseBy;
                this.content[gridRow.Key] = updatedRowValue;
            }
            this.sortGrid();
        }

        public Dictionary<string, Dictionary<string, float>> Content { get => content; set => content = value; }
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