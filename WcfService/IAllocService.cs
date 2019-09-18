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
        List<AllocOutput> GetAllocationsSortMid(AllocInput allocInput);
    }


    [DataContract]
    public class AllocInput
    {
        private float maxDuration;
        private float refFrequency;
        private Dictionary<string, float> tasks;
        private Dictionary<string, float> processors;
        private List<float> coefficients;

        public AllocInput(float maxDuration, float refFrequency, Dictionary<string, float> tasks, Dictionary<string, float> processors, List<float> coefficients)
        {
            this.maxDuration = maxDuration;
            this.refFrequency = refFrequency;
            this.tasks = tasks;
            this.processors = processors;
            this.coefficients = coefficients;
        }

        [DataMember]
        public float MaxDuration { get => maxDuration; set => maxDuration = value; }

        [DataMember]
        public float RefFrequency { get => refFrequency; set => refFrequency = value; }

        [DataMember]
        public Dictionary<string, float> Tasks { get => tasks; set => tasks = value; }

        [DataMember]
        public Dictionary<string, float> Processors { get => processors; set => processors = value; }

        [DataMember]
        public List<float> Coefficients { get => coefficients; set => coefficients = value; }
    }

    [DataContract]
    public class AllocOutput
    {
        private string allocationId;
        private float timeConsumed;
        private float energyConsumed;
        private Dictionary<string, List<string>> processors;

        public AllocOutput(string allocationId, float timeConsumed, float energyConsumed, Dictionary<string, List<string>> processors)
        {
            this.allocationId = allocationId;
            this.timeConsumed = timeConsumed;
            this.energyConsumed = energyConsumed;
            this.processors = processors;
        }

        [DataMember]
        public string AllocationId { get => allocationId; set => allocationId = value; }

        [DataMember]
        public float TimeConsumed { get => timeConsumed; set => timeConsumed = value; }

        [DataMember]
        public float EnergyConsumed { get => energyConsumed; set => energyConsumed = value; }

        [DataMember]
        public Dictionary<string, List<string>> Processors { get => processors; set => processors = value; }
    }
}
