using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TaskAllocationLibrary;

namespace AllocationHeuristicWcfService
{
    public class Service1 : IService1
    {
        public List<TaskAllocationOutput> GetAllocations(TaskAllocationInput input)
        {
            return HeuristicAlg.GetAllocations(input);
        }
    }
}
