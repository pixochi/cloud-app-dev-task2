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
    [ServiceContract]
    public interface ISortMidService
    {
        [OperationContract]
        List<TaskAllocationOutput> GetAllocations(TaskAllocationInput input);
    }
}
