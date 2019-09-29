using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TaskAllocationLibrary;

namespace HeuristicWcfService
{
    public class HeuristicService1 : IHeuristicService1
    {
        public List<TaskAllocationOutput> GetAllocations(TaskAllocationInput input)
        {
            return HeuristicAlg.GetAllocations(input);
        }
    }
}
