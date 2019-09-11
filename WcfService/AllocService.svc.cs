using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    public class AllocService : IAllocService
    {
        public List<AllocOutput> GetAllocations(AllocInput allocInput)
        {
            var possibleAllocations = new List<AllocOutput>();
            var processors = new Dictionary<string, List<bool>>();
            var processor1 = new List<bool>() {true, true, false};
            processors.Add("1", processor1);
            var allocOutput = new AllocOutput("1", 4.2f, 55, processors);
            possibleAllocations.Add(allocOutput);

            return possibleAllocations;
        }
    }
}
