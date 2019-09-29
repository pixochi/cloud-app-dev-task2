using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TaskAllocationLibrary
{
    [DataContract]
    public class TaskAllocationOutput
    {
        private string allocationId;
        private float timeConsumed;
        private float energyConsumed;
        private Dictionary<string, List<string>> processors;

        public TaskAllocationOutput(string allocationId, float timeConsumed, float energyConsumed, Dictionary<string, List<string>> processors)
        {
            this.allocationId = allocationId;
            this.timeConsumed = timeConsumed;
            this.energyConsumed = energyConsumed;
            this.processors = processors;
        }

        public string GetUniqueId()
        {
            string id = "";
            foreach (var processor in this.processors) {
                id += processor.Key + ":";
                foreach (var taskId in processor.Value) {
                    id += taskId + ",";
                }
            }

            return id;
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
