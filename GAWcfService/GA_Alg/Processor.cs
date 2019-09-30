using Medallion;
using System.Collections.Generic;

namespace GAWcfService.GA_Alg
{
    // Structure that handles assignment of tasks on a single processor
    public class Processor
    {
        private List<string> tasks;
        private string id;
        private float freq;
        private float runtime;

        public Processor(string id, float freq)
        {
            this.tasks = new List<string>();
            this.id = id;
            this.freq = freq;
            this.runtime = 0;
        }

        public string Id { get => id; set => id = value; }
        public List<string> Tasks { get => tasks; set => tasks = value; }
        public float Freq { get => freq; set => freq = value; }
        public float Runtime { get => runtime; set => runtime = value; }

        public string GetRandomTask()
        {
            if (this.tasks.Count == 0) {
                return "";
            }

            int taskIndex = Rand.Next(0, this.tasks.Count);

            return this.tasks[taskIndex];
        }

        public void AssignTask(string taskId)
        {
            if (!this.tasks.Contains(taskId)) {
                this.tasks.Add(taskId);
            }
        }

        public void RemoveTask(string taskId)
        {
            this.tasks.Remove(taskId);
        }

        public void AddRuntime(float increaseBy)
        {
            this.runtime += increaseBy;
        }

        public Processor Clone()
        {
            Processor newProc = new Processor(this.id, this.freq);
            newProc.tasks = new List<string>(this.tasks);
            newProc.runtime = this.runtime;
            return newProc;
        }
    }
}