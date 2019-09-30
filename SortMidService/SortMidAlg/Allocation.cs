using System.Collections.Generic;
using TaskAllocationLibrary;

namespace SortMidService.SortMidAlg
{
    // Structure of an allocation for SortMid algorithm
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

        // Select a processor on which a given task will run
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
}