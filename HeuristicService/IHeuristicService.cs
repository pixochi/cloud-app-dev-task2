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
    [ServiceContract]
    public interface IHeuristicService
    {
        [OperationContract]
        List<TaskAllocationOutput> GetAllocations(TaskAllocationInput input);
    }
}
