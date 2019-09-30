using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TaskAllocationLibrary;

namespace SortMidService
{
    public class Service1 : ISortMidService
    {
        public List<TaskAllocationOutput> GetAllocations(TaskAllocationInput input)
        {
            return SortMidAlg.SortMidAlg.GetAllocations(input);
        }
    }
}
