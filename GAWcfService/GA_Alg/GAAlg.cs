using Medallion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using TaskAllocationLibrary;

namespace GAWcfService.GA_Alg
{
    public class GAAlg
    {
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

        public GAAlg(Dictionary<string, float> tasks, Dictionary<string, float> processors, List<float> coefficients, float refFreq, float maxDuration)
        {
            this.tasks = tasks;
            this.processors = processors;
            this.coefficients = coefficients;
            this.refFreq = refFreq;
            this.maxDuration = maxDuration;
            this.correctAllocs = new List<Allocation>();
        }

        public Population GetLastGeneration()
        {
            return this.lastGeneration;
        }


        public void Train()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Population population = new Population(POPULATION_SIZE, true, this.coefficients, this.refFreq, this.tasks, this.processors);

            while (stopWatch.ElapsedMilliseconds < Constants.MAX_PROGRAM_DURATION_MS) {
                population = this.evolvePopulation(population);
                //Debug.WriteLine(population.GetFittest().EnergyConsumed.ToString());
            }

            this.lastGeneration = population;
        }

        public List<TaskAllocationOutput> GetCorrectAllocs()
        {
            
            List<TaskAllocationOutput> allocations = new List<TaskAllocationOutput>();

            Population pop = this.GetLastGeneration();

            var uniqueCorrectAllocs = this.correctAllocs.GroupBy(elem => elem.GetUniqueId()).Select(group => group.First());

            foreach (var alloc in uniqueCorrectAllocs) {
                Dictionary<string, List<string>> processors = new Dictionary<string, List<string>>();
                foreach (var proc in alloc.Processors) {
                    processors.Add(proc.Value.Id, proc.Value.Tasks);
                }

                TaskAllocationOutput allocOutput = new TaskAllocationOutput((allocations.Count + 1).ToString(), alloc.ProgramRuntime, alloc.EnergyConsumed, processors);
                allocations.Add(allocOutput);
            }

            return allocations;
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
    }
}