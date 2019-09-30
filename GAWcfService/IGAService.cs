using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using TaskAllocationLibrary;

namespace GAWcfService
{
    [ServiceContract]
    public interface IGAService
    {
        [OperationContract]
        List<TaskAllocationOutput> GetAllocations(TaskAllocationInput input);
    }    
}
