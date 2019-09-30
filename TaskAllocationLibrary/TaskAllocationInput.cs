using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TaskAllocationLibrary
{
    // Input format from a client
    [DataContract]
    public class TaskAllocationInput
    {
        private float maxDuration;
        private float refFrequency;
        private Dictionary<string, float> tasks;
        private Dictionary<string, float> processors;
        private List<float> coefficients;

        public TaskAllocationInput(float maxDuration, float refFrequency, Dictionary<string, float> tasks, Dictionary<string, float> processors, List<float> coefficients)
        {
            this.maxDuration = maxDuration;
            this.refFrequency = refFrequency;
            this.tasks = tasks;
            this.processors = processors;
            this.coefficients = coefficients;
        }

        public TaskAllocationInput(Dictionary<string, float> tasks, Dictionary<string, float> processors, float maxDuration, float refFrequency, List<float> coefficients)
        {
            this.tasks = tasks;
            this.processors = processors;
            this.maxDuration = maxDuration;
            this.refFrequency = refFrequency;
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
}
