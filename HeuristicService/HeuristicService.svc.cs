using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TaskAllocationLibrary;

namespace HeuristicService
{
    public class HeuristicService : IHeuristicService
    {
        public List<TaskAllocationOutput> GetAllocations(TaskAllocationInput input)
        {
            return HeuristicAlg.GetAllocations(input);
        }
    }
}
