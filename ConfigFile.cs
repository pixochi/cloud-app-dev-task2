using GUI.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    class ConfigFile
    {
        private string content;
        private int[] limitsTasks = new int[2];
        private float maxDuration;
        private float refFrequency;
        private Dictionary<string, float> tasks;
        private Dictionary<string, float> processors;

        public ConfigFile(string configFileContent)
        {
            this.Content = configFileContent;
            this.MaxDuration = ConfigFileParser.GetMaximumDuration(configFileContent);
            this.RefFrequency = ConfigFileParser.GetRuntimeReferenceFrequency(configFileContent);
            this.Tasks = ConfigFileParser.GetTasks(configFileContent);
            this.Processors = ConfigFileParser.GetProcessors(configFileContent);
        }

        public string Content { get => content; set => content = value; }
        public int[] LimitsTasks { get => limitsTasks; set => limitsTasks = value; }
        public float MaxDuration { get => maxDuration; set => maxDuration = value; }
        public float RefFrequency { get => refFrequency; set => refFrequency = value; }
        public Dictionary<string, float> Tasks { get => tasks; set => tasks = value; }
        public Dictionary<string, float> Processors { get => processors; set => processors = value; }
    }
}
