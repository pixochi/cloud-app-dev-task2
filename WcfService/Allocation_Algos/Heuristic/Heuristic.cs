using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using WcfService.Allocation_Algos.Shared;

namespace WcfService.Allocation_Algos.Heuristic
{
    public static class Heuristic
    {
        private static uint MAX_DURATION_MS = 4000;
        private static float TIME_STEP = 0.0001f;
        private static uint MAX_ALLOCATIONS = 1000;
        public static List<AllocOutput> GetAllocations(AllocInput allocInput)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<AllocOutput> allocations = new List<AllocOutput>();
            uint trialCount = 0;

            while (stopWatch.ElapsedMilliseconds < MAX_DURATION_MS && allocations.Count < MAX_ALLOCATIONS) {
                Allocation newAllocation = new Allocation(allocInput.Tasks.Count, allocInput.Processors, allocInput.Coefficients);
                var unassignedTasks = new Dictionary<string, float>(allocInput.Tasks);
                var processorRuntimes = new Dictionary<string, float>();
                var shouldStopAllocating = false;

                while (!newAllocation.IsDone && !shouldStopAllocating) {
                    var availableProcessors = new Dictionary<string, float>(allocInput.Processors);
                    string longestRuntimeTaskId;
                    string hiqhestFreqProcessorId;
                    float taskRuntime;

                    do {
                        longestRuntimeTaskId = unassignedTasks.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                        hiqhestFreqProcessorId = availableProcessors.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                        taskRuntime = unassignedTasks[longestRuntimeTaskId] * (allocInput.RefFrequency / availableProcessors[hiqhestFreqProcessorId]);
                        availableProcessors.Remove(hiqhestFreqProcessorId);

                        if (!processorRuntimes.ContainsKey(hiqhestFreqProcessorId)) {
                            processorRuntimes.Add(hiqhestFreqProcessorId, 0);
                        }

                    } while ((processorRuntimes[hiqhestFreqProcessorId] + taskRuntime) > (allocInput.MaxDuration - (trialCount * TIME_STEP)) && availableProcessors.Count != 0);

                    if (!String.IsNullOrEmpty(hiqhestFreqProcessorId)) {
                        newAllocation.AssignTaskToProcessor(longestRuntimeTaskId, hiqhestFreqProcessorId, taskRuntime);
                        processorRuntimes[hiqhestFreqProcessorId] += taskRuntime;
                        unassignedTasks.Remove(longestRuntimeTaskId);
                    }
                    else {
                        shouldStopAllocating = true;
                    }
                }

                float programRuntime = processorRuntimes.Max((x) => x.Value);

                if (newAllocation.IsDone && programRuntime <= allocInput.MaxDuration) {
                    AllocOutput allocOutput = new AllocOutput((allocations.Count + 1).ToString(), programRuntime, newAllocation.EnergyConsumed, newAllocation.Processors);
                    bool isAllocationUnique = true;

                    // Check if the same allocation already exists
                    foreach (var allocation in allocations) {
                        if (allocation.GetUniqueId() == allocOutput.GetUniqueId()){
                            isAllocationUnique = false;
                        }
                    }

                    if (isAllocationUnique) {
                        allocations.Add(allocOutput);
                    }
                }

                trialCount += 1;
            }

            return allocations.ToList();
        }
    }
}