using GAWcfService.GA_Alg;
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
    public class GAService : IGAService
    {
        public List<TaskAllocationOutput> GetAllocations(TaskAllocationInput input)
        {
            var GA = new GAAlg(input.Tasks, input.Processors, input.Coefficients, input.RefFrequency, input.MaxDuration);
            GA.Train();

            return GA.GetCorrectAllocs();
        }
    }
}
