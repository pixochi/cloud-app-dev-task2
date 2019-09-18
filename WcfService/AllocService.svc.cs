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
    }
}
