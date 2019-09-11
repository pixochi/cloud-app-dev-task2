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
        public AllocInput GetAllocations(AllocInput allocInput)
        {
            return allocInput;
        }
    }
}
