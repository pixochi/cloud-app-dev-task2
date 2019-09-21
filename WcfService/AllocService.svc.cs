using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Threading.Tasks;
using System.Diagnostics;
using WcfService.Allocation_Algos.SortMid;
using WcfService.Allocation_Algos.Heuristic;
using WcfService.Allocation_Algos.GA;

namespace WcfService
{
    public class AllocService : IAllocService
    {
        public List<AllocOutput> GetAllocationsSortMid(AllocInput allocInput)
        {
            return SortMid.GetAllocations(allocInput);
        }

        public List<AllocOutput> GetAllocationsHeuristic(AllocInput allocInput)
        {
            return Heuristic.GetAllocations(allocInput);
        }

        public List<AllocOutput> GetAllocationsGA(AllocInput allocInput)
        {
            List<AllocOutput> allocations = new List<AllocOutput>();

            GA GAlgo = new GA(10, allocInput.Tasks, allocInput.Processors, allocInput.Coefficients, allocInput.RefFrequency, allocInput.MaxDuration);
            GAlgo.Train();

            Population pop = GAlgo.GetLastGeneration();

            foreach (var alloc in GAlgo.GetCorrectAllocs()) {
                float runtime = alloc.ProgramRuntime;
                var a = runtime;
                pop.SaveAlloc(alloc);
            }

            foreach (var alloc in pop.GetIndividuals()) {
                Dictionary<string, List<string>> processors = new Dictionary<string, List<string>>();
                foreach (var proc in alloc.Processors) {
                    processors.Add(proc.Value.Id, proc.Value.Tasks);
                }

                AllocOutput allocOutput = new AllocOutput((allocations.Count + 1).ToString(), alloc.ProgramRuntime, alloc.EnergyConsumed, processors);
                allocations.Add(allocOutput);
            }

            return allocations;
        }
    }
}
