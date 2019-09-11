using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace WcfService
{
    public class AllocService : IAllocService
    {
        // Based on:
        // https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4642166/
        public List<AllocOutput> GetAllocations(AllocInput allocInput)
        {
            var possibleAllocations = new List<AllocOutput>();
            int tasksCount = allocInput.Tasks.Count;
            int allocatedTasksCount = 0;
            int processorsCount = allocInput.Processors.Count;
            int K = processorsCount / 2;
            Dictionary<string, List<bool>> processors = this.initProcessors(allocInput.Processors.Count, allocInput.Tasks.Count);
            List<float> processorCompletionTimes = new List<float>();

            while (allocatedTasksCount != tasksCount) {
                if (allocatedTasksCount == tasksCount - 1) {
                    float maxValue = 0;
                    int indexProcessor = 0;
                    int indexTask = 0;

                    // 1. Sort the completion times(SCT) of each task Ti in T in increasing order.
                    for (int i = 0; i < allocInput.Tasks.Count; i++) {
                        var taskRuntimes = new List<float>();
                        float maxTaskRuntime = 0;
                        int maxProcessorRuntimeIndex = 0;
                        for (int j = 0; j < allocInput.Processors.Count; j++) {
                            float taskRuntime = allocInput.Tasks.Values.ElementAt(i) * (allocInput.RefFrequency / allocInput.Processors.Values.ElementAt(j));
                            taskRuntimes.Add(taskRuntime);

                            if (taskRuntime > maxTaskRuntime) {
                                maxTaskRuntime = taskRuntime;
                                maxProcessorRuntimeIndex = j;
                            }
                        }
                        taskRuntimes.Sort();

                        // 2.The introduced scheduling decision is based on computing the average value AV of two consecutive completion times
                        // in SCT for each Ti.
                        float AV = (taskRuntimes[K] + taskRuntimes[K + 1]) / 2;

                        // 3. In the second step, the task having the maximum AV is selected.
                        if (AV > maxValue) {
                            maxValue = AV;
                            indexTask = i;
                            indexProcessor = maxProcessorRuntimeIndex;
                        }

                        // 4. In the third step, the task is assigned to the machine possessing minimum completion time.
                        var processor = processors[indexProcessor.ToString()];
                        processor[indexTask] = true;
                        processors[indexProcessor.ToString()] = processor;
                    }
                }
                else {
                    //Assign the remaining task to the machine having the minimum completion time and delete it;
                    //Update waiting time of machine executing it;
                }
            }

            // 5. Next, the assigned task is deleted from T.

            // 6. Finally, the waiting time for the machine that executes this task is updated.

            // 7. These steps are repeated until all n tasks are scheduled on m machines.

            return possibleAllocations;
        }

        private Dictionary<string, List<bool>> initProcessors(int processorsCount, int tasksCount)
        {
            var processors = new Dictionary<string, List<bool>>();

            for (int i = 0; i < processorsCount; i++) {
                var tasks = new List<bool>();
                for (int j = 0; j < tasksCount; j++) {
                    tasks.Add(false);
                }
            }

            return processors;
        }

        public List<AllocOutput> GetAllocationsSortMid(AllocInput allocInput)
        {
            var tasksByCompletionTime = getTasksByCompletionTime(allocInput.Tasks, allocInput.Processors, allocInput.RefFrequency);
            Dictionary<string, List<string>> processorsWithTasks = new Dictionary<string, List<string>>();
            Dictionary<string, float> processorsComputationalTime = new Dictionary<string, float>();


            while (tasksByCompletionTime.Count != 0) {
                if (tasksByCompletionTime.Count != 1) {
                    float maxMidValue = 0;
                    string taskToAllocateId = "";
                    string optimalProcessorId = "";

                    foreach (var task in tasksByCompletionTime) {
                        int midCompletionTimeIndex = task.Value.Count / 2;
                        float midCompletionTimeValue = (Convert.ToSingle(task.Value.ElementAt(midCompletionTimeIndex).Value) + Convert.ToSingle(task.Value.ElementAt(midCompletionTimeIndex + 1).Value)) / 2;
                        if (midCompletionTimeValue > maxMidValue) {
                            maxMidValue = midCompletionTimeValue;
                            taskToAllocateId = task.Key;
                            optimalProcessorId = task.Value.ElementAt(0).Key;
                        }
                    }

                    processorsWithTasks[optimalProcessorId].Add(taskToAllocateId);
                    tasksByCompletionTime.Remove(taskToAllocateId);
                    processorsComputationalTime[optimalProcessorId] = processorsComputationalTime[optimalProcessorId] + tasksByCompletionTime[taskToAllocateId][optimalProcessorId];
                }
                else {
                    // assign the last unallocated task to the processor with the minimum completion time
                    processorsWithTasks[tasksByCompletionTime.First().Value.First().Key].Add(tasksByCompletionTime.First().Key);
                    tasksByCompletionTime.Remove(tasksByCompletionTime.First().Key);
                    processorsComputationalTime[tasksByCompletionTime.First().Value.First().Key] = processorsComputationalTime[tasksByCompletionTime.First().Value.First().Key] + tasksByCompletionTime.First().Value.First().Value;
                }
            }
        }

        private Dictionary<string, Dictionary<string, float>> getTasksByCompletionTime(Dictionary<string, float> tasks, Dictionary<string, float> processors, float refFrequency)
        {
            Dictionary<string, Dictionary<string, float>> allTasksByCompletionTime = new Dictionary<string, Dictionary<string, float>>();

            foreach (var task in tasks) {
                OrderedDictionary taskByCompletionTime = new OrderedDictionary();

                foreach (var processor in processors) {
                    float taskRuntime = task.Value * (refFrequency / processor.Value);
                    taskByCompletionTime.Add(processor.Key, taskRuntime);
                }

                Dictionary<string, float> sortedTaskByCompletionTime = taskByCompletionTime.Cast<DictionaryEntry>()
                       .OrderBy(r => r.Value)
                       .ToDictionary(c => c.Key as string, d => Convert.ToSingle(d.Value));

                var sortedDictionary = new SortedDictionary<string, float>(sortedTaskByCompletionTime);
                allTasksByCompletionTime.Add(task.Key, sortedTaskByCompletionTime);
            }


            return allTasksByCompletionTime;
        }
    }
}
