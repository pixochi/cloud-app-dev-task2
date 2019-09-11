using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    [ServiceContract]
    public interface IAllocService
    {
        [OperationContract]
        AllocInput GetAllocations(AllocInput allocInput);
    }


    [DataContract]
    public class AllocInput
    {
        private float maxDuration;
        private float refFrequency;
        private Dictionary<string, float> tasks;
        private Dictionary<string, float> processors;

        public AllocInput(Dictionary<string, float> tasks, Dictionary<string, float> processors, float maxDuration, float refFrequency)
        {
            this.tasks = tasks;
            this.processors = processors;
            this.maxDuration = maxDuration;
            this.refFrequency = refFrequency;
        }

        [DataMember]
        public float MaxDuration { get => maxDuration; set => maxDuration = value; }

        [DataMember]
        public float RefFrequency { get => refFrequency; set => refFrequency = value; }

        [DataMember]
        public Dictionary<string, float> Tasks { get => tasks; set => tasks = value; }

        [DataMember]
        public Dictionary<string, float> Processors { get => processors; set => processors = value; }
    }
}
