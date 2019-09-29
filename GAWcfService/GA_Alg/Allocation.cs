using Medallion;
using System.Collections.Generic;
using System.Linq;
using TaskAllocationLibrary;

namespace GAWcfService.GA_Alg
{
    public class Allocation
    {
        private float energyConsumed;
        private Dictionary<string, Processor> processors;
        private float refFreq;
        private List<float> coefficients;
        private Dictionary<string, float> tasks;
        private Dictionary<string, float> processorFreqs;
        private float programRuntime;
        private int assignedTasksCount;

        public Allocation(List<float> coefficients, float refFreq, Dictionary<string, float> tasks, Dictionary<string, float> processorFreqs, bool shouldBeEmpty = false)
        {
            this.energyConsumed = 0;
            this.assignedTasksCount = 0;
            this.programRuntime = 0;
            this.processors = new Dictionary<string, Processor>();
            this.refFreq = refFreq;
            this.coefficients = coefficients;
            this.tasks = tasks;
            this.processorFreqs = processorFreqs;

            // Create an instance for each processor
            foreach (var proc in this.processorFreqs) {
                this.processors.Add(proc.Key, new Processor(proc.Key, this.processorFreqs[proc.Key]));
            }

            if (!shouldBeEmpty) {
                // Randomly assign each task to a processor
                foreach (var task in this.tasks) {
                    int processorIndex = Rand.Next(0, this.processors.Count);
                    this.AssignTaskToProcessor(this.processors.ElementAt(processorIndex).Key, task.Key);
                }
            }
        }

        public Allocation Clone()
        {
            var clonedAlloc = new Allocation(this.coefficients, this.refFreq, this.tasks, this.processorFreqs, true) {
                energyConsumed = this.energyConsumed
            };

            var clonedProcessors = new Dictionary<string, Processor>();
            foreach (var proc in this.processors) {
                clonedProcessors.Add(proc.Key, proc.Value.Clone());
            }
            clonedAlloc.processors = clonedProcessors;

            return clonedAlloc;
        }

        public List<string> ProcessorIds { get => Processors.Keys.ToList(); }
        public Dictionary<string, Processor> Processors { get => processors; set => processors = value; }
        public float EnergyConsumed { get => this.getEnergyConsumed(); }
        public float ProgramRuntime { get => this.getProgramRuntime(); set => programRuntime = value; }
        public int AssignedTasksCount
        {
            get {
                int count = 0;

                foreach (var item in this.processors) {
                    count += item.Value.Tasks.Count;
                }

                return count;
            }
            set => assignedTasksCount = value;
        }

        private float getProgramRuntime()
        {
            float programRuntime = 0;

            foreach (var processor in this.processors) {
                float processorRuntime = processor.Value.Runtime;

                if (processorRuntime > programRuntime) {
                    programRuntime = processorRuntime;
                }
            }

            return programRuntime;
        }

        private float getEnergyConsumed()
        {
            float energyConsumed = 0;
            foreach (var proc in processors) {
                foreach (var taskId in proc.Value.Tasks) {
                    // Store energy consumed by the task
                    float runtimePerTask = this.tasks[taskId] * (this.refFreq / this.processorFreqs[proc.Key]);
                    energyConsumed += AllocationHelper.GetEnergyConsumedPerTask(this.coefficients, this.processorFreqs[proc.Key], runtimePerTask);
                }
            }
            return energyConsumed;
        }

        public double GetFitness()
        {
            return 1 / this.getProgramRuntime();
        }

        public Processor GetProcessor(string id)
        {
            return this.processors[id].Clone();
        }

        public void SetProcessor(string procId, Processor proc)
        {
            this.processors[procId] = proc;
        }

        public void AssignTaskToProcessor(string processorId, string taskId)
        {
            Processor proc;

            if (this.processors.ContainsKey(processorId)) {
                proc = this.processors[processorId];
            }
            else {
                proc = new Processor(processorId, this.processorFreqs[processorId]);
            }

            proc.AssignTask(taskId);

            // Store runtime of the task on the selected processor
            float runtimePerTask = this.tasks[taskId] * (this.refFreq / this.processorFreqs[processorId]);
            proc.AddRuntime(runtimePerTask);
            this.processors[processorId] = proc;
        }

        public void RemoveTask(string processorId, string taskId)
        {
            Processor proc = this.processors[processorId];
            proc.RemoveTask(taskId);
            this.processors[processorId] = proc;
        }

        public bool ContainsProcessor(string processorId)
        {
            return this.processors.ContainsKey(processorId);
        }

        public bool AllTasksAssigned()
        {
            int assignedTasksCount = 0;
            foreach (var proc in this.processors) {
                assignedTasksCount += proc.Value.Tasks.Count;
            }

            return (assignedTasksCount == this.tasks.Count);
        }

        public List<string> GetUnassignedTasks()
        {
            List<string> taskIds = this.tasks.Keys.ToList();

            foreach (var proc in this.processors) {
                foreach (var taskId in proc.Value.Tasks) {
                    taskIds.Remove(taskId);
                }
            }

            return taskIds;
        }

        public void RemoveDuplicatedTasks(List<string> assignedTasks)
        {
            var tmpProcessors = new Dictionary<string, Processor>(this.processors);
            foreach (var proc in tmpProcessors) {
                var tmp = new List<string>(proc.Value.Tasks);
                foreach (var taskId in tmp) {
                    int index = assignedTasks.FindIndex(t => t == taskId);
                    if (index != 1) {
                        this.RemoveTask(proc.Key, taskId);
                    }
                }
            }
        }

        public List<string> GetAssignedTasks()
        {
            List<string> taskIds = new List<string>();

            foreach (var proc in this.processors) {
                foreach (var taskId in proc.Value.Tasks) {
                    taskIds.Add(taskId);
                }
            }

            return taskIds;
        }

        public string GetUniqueId()
        {
            string id = "";

            foreach (var proc in this.processors) {
                id += proc.Key + ":";
                proc.Value.Tasks.Sort();
                foreach (var taskId in proc.Value.Tasks) {
                    id += taskId + ",";
                }
            }

            return id;
        }
    }
}