using Medallion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using WcfService.Allocation_Algos.Shared;

namespace WcfService.Allocation_Algos.GA
{
    public class GA
    {
        private static int MINUTE = 1000 * 60;
        private static int MAX_TRAINING_DURATION_MS = 5 * MINUTE;
        private static float MUTATION_RATE = 0.05f;
        private static float CROSSOVER_RATE = 0.7f;
        private static int POPULATION_SIZE = 30;
        private static int TOURNAMENT_SIZE = 15;
        private static int MUTATE_TASKS_COUNT = 1;
        private Dictionary<string, float> tasks;
        private Dictionary<string, float> processors;
        private List<float> coefficients;
        private float refFreq;
        private Population lastGeneration;
        private float maxDuration;
        private List<Allocation> correctAllocs; 

        public GA(Dictionary<string, float> tasks, Dictionary<string, float> processors, List<float> coefficients, float refFreq, float maxDuration)
        {
            this.tasks = tasks;
            this.processors = processors;
            this.coefficients = coefficients;
            this.refFreq = refFreq;
            this.maxDuration = maxDuration;
            this.correctAllocs = new List<Allocation>();
        }

        private Population evolvePopulation(Population population)
        {
            Population newPopulation = new Population(population.Size, false, this.coefficients, this.refFreq, this.tasks, this.processors);

            Allocation fittest = population.GetFittest();
            newPopulation.SaveAlloc(fittest);

            if (fittest.ProgramRuntime < this.maxDuration) {
                correctAllocs.Add(fittest);
            }

            for (int allocIndex = 1; allocIndex < population.Size; allocIndex++) {

                if (Rand.NextDouble() < CROSSOVER_RATE) {
                    Allocation parent1 = this.tournamentSelect(population);
                    Allocation parent2 = this.tournamentSelect(population);
                    Allocation child = this.crossover(parent1, parent2);

                    newPopulation.SaveAlloc(child);
                }
                else {
                    newPopulation.SaveAlloc(population.GetAllocation(allocIndex));
                }

            }

            for (int allocationIndex = 0; allocationIndex < newPopulation.Size; allocationIndex++) {
                Allocation mutatedAlloc = this.mutate(newPopulation.GetAllocation(allocationIndex));
                newPopulation.ReplaceAlloc(allocationIndex, mutatedAlloc);
            }

            return newPopulation;
        }

        private Allocation tournamentSelect(Population population)
        {
            Population tournament = new Population(TOURNAMENT_SIZE, false, this.coefficients, this.refFreq, this.tasks, this.processors);

            for (int allocationIndex = 0; allocationIndex < TOURNAMENT_SIZE; allocationIndex++) {
                int randomAllocationIndex = Rand.Next(0, population.Size);
                tournament.SaveAlloc(population.GetAllocation(randomAllocationIndex));
            }

            Allocation fittest = tournament.GetFittest();

            return fittest;
        }

        private Allocation crossover(Allocation parent1, Allocation parent2)
        {
            Allocation child = new Allocation(this.coefficients, this.refFreq, this.tasks, this.processors, true);
            List<string> processorIds = parent1.ProcessorIds;
            int startPos = Rand.Next(0, processorIds.Count);
            int endPos = Rand.Next(startPos + 1, processorIds.Count);

            while (startPos >= endPos) {
                startPos = Rand.Next(0, processorIds.Count);
                endPos = Rand.Next(startPos + 1, processorIds.Count);
            }

            // Select processors from parent 1
            for (int processorIndex = startPos; processorIndex < endPos; processorIndex++) {
                string processorId = processorIds[processorIndex];
                child.SetProcessor(processorId, parent1.GetProcessor(processorId));
            }

            // Remove duplicated tasks
            //parent2.RemoveDuplicatedTasks(child.GetAssignedTasks());

            // Select processors from parent 2
            for (int processorIndex = 0; processorIndex < processorIds.Count; processorIndex++) {
                string processorId = processorIds[processorIndex];

                if (!child.ContainsProcessor(processorId)) {
                    child.SetProcessor(processorId, parent2.GetProcessor(processorId));
                }
            }

            var unassignedTasks = child.GetUnassignedTasks();
            foreach (var taskId in unassignedTasks) {
                string randomProcessorId = processorIds[Rand.Next(0, processorIds.Count)];
                child.AssignTaskToProcessor(randomProcessorId, taskId);
            }

            return child;
        }

        private Allocation mutate(Allocation alloc)
        {
            Allocation mutatedAlloc = alloc.Clone();

            if (Rand.NextDouble() < MUTATION_RATE) {
                for (int _ = 0; _ < MUTATE_TASKS_COUNT; _++) {
                    string taskId = "";
                    string originalProcId = "";

                    do {
                        // Get a random task id
                        int randOriginalProcIndex = Rand.Next(0, mutatedAlloc.ProcessorIds.Count);
                        originalProcId = mutatedAlloc.ProcessorIds[randOriginalProcIndex];
                        Processor proc = mutatedAlloc.GetProcessor(originalProcId);
                        taskId = proc.GetRandomTask();
                    } while (taskId == "");

                    // Assign the task id to another processor
                    string newProcId;
                    do {
                        int randNewProcIndex = Rand.Next(0, mutatedAlloc.ProcessorIds.Count);
                        newProcId = mutatedAlloc.ProcessorIds[randNewProcIndex];
                    } while (newProcId == originalProcId);

                    mutatedAlloc.AssignTaskToProcessor(newProcId, taskId);
                    mutatedAlloc.RemoveTask(originalProcId, taskId);
                }
            }

            return mutatedAlloc;
        }

        public Allocation GetBestAlloc()
        {
            Population lastGeneration = this.GetLastGeneration();
            return lastGeneration.GetFittest();
        }

        public Population GetLastGeneration()
        {
            return this.lastGeneration;
        }

        public List<Allocation> GetCorrectAllocs()
        {
            return this.correctAllocs;
        }

        public void Train()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Population population = new Population(POPULATION_SIZE, true, this.coefficients, this.refFreq, this.tasks, this.processors);

            while (stopWatch.ElapsedMilliseconds < MAX_TRAINING_DURATION_MS) {
                population = this.evolvePopulation(population);
                //Debug.WriteLine(population.GetFittest().EnergyConsumed.ToString());
            }

            this.lastGeneration = population;
        }
    }

    public class Population
    {
        private List<Allocation> allocList;

        public int Size { get => allocList.Count;}

        public Population(int populationSize, bool isInitial, List<float> coefficients, float refFreq, Dictionary<string, float> tasks, Dictionary<string, float> processors)
        {
            this.allocList = new List<Allocation>();

            if (isInitial) {
                for (int allocationIndex = 0; allocationIndex < populationSize; allocationIndex++) {
                    Allocation alloc = new Allocation(coefficients, refFreq, tasks, processors);

                    this.SaveAlloc(alloc);
                }
            }
        }

        public void SaveAlloc(Allocation alloc)
        {
            this.allocList.Add(alloc);
        }
        public void ReplaceAlloc(int allocIndex, Allocation alloc)
        {
            this.allocList[allocIndex] = alloc;
        }

        public Allocation GetAllocation(int allocIndex)
        {
            return this.allocList[allocIndex];
        }

        public Allocation GetFittest()
        {

            Allocation fittest = this.GetAllocation(0);

            for (int allocIndex = 1; allocIndex < this.allocList.Count; allocIndex++) {
                if (fittest.GetFitness() < this.GetAllocation(allocIndex).GetFitness()) {
                    fittest = this.GetAllocation(allocIndex);
                }
            }

            return fittest.Clone();
        }

        public List<Allocation> GetIndividuals()
        {
            return this.allocList;
        }

    }

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

        public Allocation(List<float> coefficients, float refFreq, Dictionary <string, float> tasks, Dictionary<string, float> processorFreqs, bool shouldBeEmpty = false)
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
        public int AssignedTasksCount {
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

    public class Processor
    {
        private List<string> tasks;
        private string id;
        private float freq;
        private float runtime;

        public Processor(string id, float freq)
        {
            this.tasks = new List<string>();
            this.id = id;
            this.freq = freq;
            this.runtime = 0;
        }

        public string Id { get => id; set => id = value; }
        public List<string> Tasks { get => tasks; set => tasks = value; }
        public float Freq { get => freq; set => freq = value; }
        public float Runtime { get => runtime; set => runtime = value; }

        public string GetRandomTask()
        {
            if (this.tasks.Count == 0) {
                return "";
            }

            int taskIndex = Rand.Next(0, this.tasks.Count);

            return this.tasks[taskIndex];
        }

        public void AssignTask(string taskId)
        {
            if (!this.tasks.Contains(taskId)) {
                this.tasks.Add(taskId);
            }
        }

        public void RemoveTask(string taskId)
        {
            this.tasks.Remove(taskId);
        }

        public void AddRuntime(float increaseBy)
        {
            this.runtime += increaseBy;
        }

        public Processor Clone()
        {
            Processor newProc = new Processor(this.id, this.freq);
            newProc.tasks = new List<string>(this.tasks);
            newProc.runtime = this.runtime;
            return newProc;
        }
    }

}