using System.Collections.Generic;

namespace GAWcfService.GA_Alg
{
    public class Population
    {
        private List<Allocation> allocList;

        public int Size { get => allocList.Count; }

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
}