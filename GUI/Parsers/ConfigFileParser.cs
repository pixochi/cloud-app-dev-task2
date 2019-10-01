using GUI.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GUI.Parsers
{
    // Parser for a configuration text file
    public static class ConfigFileParser
    {
        /// <summary>
        /// Extracts coefficient of a quadratic formula to compute
        /// the energy consumed per second by a processor.
        /// </summary>
        /// <returns>
        /// Coefficients of a quadratic formula
        /// </returns>
        /// <param name="configFileContent">Content of a configuration file</param>
        public static List<float> GetCoefficients(string configFileContent)
        {
            Regex coefficientsRx = new Regex($@"COEFFICIENT-ID,VALUE(?:{FileParser.NewLineRx})(?:\s*\d+,(-?\d+(?:\.?\d+)?)(?:{FileParser.NewLineRx})?)+");
            List<string> coefficients = FileParser.GetCaptureValues(configFileContent, coefficientsRx);
            return ListConverter.StringToFloat(coefficients);
        }

        /// <summary>
        /// Extracts frequencies of provided processors.
        /// </summary>
        /// <returns>
        /// Frequencies of processors
        /// </returns>
        /// <param name="configFileContent">Content of a configuration file</param>
        public static List<float> GetProcessorFrequencies(string configFileContent)
        {
            Regex frequenciesRx = new Regex($@"PROCESSOR-ID,FREQUENCY(?:{FileParser.NewLineRx})(?:\s*\d+,(\d+(?:\.?\d+)?)(?:{FileParser.NewLineRx})?)+");
            List<string> frequencies = FileParser.GetCaptureValues(configFileContent, frequenciesRx);

            return ListConverter.StringToFloat(frequencies);
        }

        /// <summary>
        /// Extracts processors, their ids and frequencies.
        /// </summary>
        /// <returns>
        ///  Processors - their ids and frequencies
        /// </returns>
        /// <param name="configFileContent">Content of a configuration file</param>
        public static Dictionary<string, float> GetProcessors(string configFileContent)
        {
            Regex processorsRx = new Regex($@"PROCESSOR-ID,FREQUENCY(?:{FileParser.NewLineRx})(?:\s*(\d+),(\d+(?:\.\d+))?(?:{FileParser.NewLineRx})?)+");
            List<string> processorsData = FileParser.GetCaptureValues(configFileContent, processorsRx);
            Dictionary<string, float> processors = new Dictionary<string, float>();

            // processorsData includes processors-ids and their frequencies
            int processorsCount = processorsData.Count / 2;

            for (int i = 0; i < processorsCount; i++)
            {
                processors.Add(processorsData[i].ToString(), float.Parse(processorsData[processorsCount + i]));
            }

            return processors;
        }

        /// <summary>
        /// Extracts tasks, their ids and runtimes
        /// </summary>
        /// <returns>
        /// All tasks from the configuration file
        /// </returns>
        /// <param name="configFileContent">Content of a configuration file</param>
        public static Dictionary<string, float> GetTasks(string configFileContent)
        {
            Regex taksksRx = new Regex($@"TASK-ID,RUNTIME(?:{FileParser.NewLineRx})(?:\s*(\d+),(\d+(?:\.\d+)?)(?:{FileParser.NewLineRx})?)+");
            List<string> tasksData = FileParser.GetCaptureValues(configFileContent, taksksRx);
            Dictionary<string, float> tasks = new Dictionary<string, float>();

            // tasksData includes task-ids and task runtimes 
            int tasksCount = tasksData.Count / 2;

            for (int i = 0; i < tasksCount; i++)
            {
                tasks.Add(tasksData[i].ToString(), float.Parse(tasksData[tasksCount + i]));
            }

            return tasks;
        }

        /// <summary>
        /// Extracts a reference frequency used to determine runtime of each task
        /// </summary>
        /// <returns>
        /// Reference frequency or -1 if it is not provided in a configuration file
        /// </returns>
        /// <param name="configFileContent">Content of a configuration file</param>
        public static float GetRuntimeReferenceFrequency(string configFileContent)
        {
            Regex runtimeReferenceFrequencyRx = new Regex(@"RUNTIME-REFERENCE-FREQUENCY,(\d+(?:\.\d+)?)");
            List<string> captureValues = FileParser.GetCaptureValues(configFileContent, runtimeReferenceFrequencyRx);

            if (captureValues.Count != 0)
            {
                return float.Parse(captureValues.First());
            }

            return -1;
        }

        /// <summary>
        /// Extracts the maximum program duration
        /// </summary>
        /// <returns>
        /// Reference the maximum duration or -1 if it is not provided in a configuration file
        /// </returns>
        /// <param name="configFileContent">Content of a configuration file</param>
        public static float GetMaximumDuration(string configFileContent)
        {
            Regex maximumDurationRx = new Regex(@"PROGRAM-MAXIMUM-DURATION,(\d+(?:\.\d+)?)");
            List<string> captureValues = FileParser.GetCaptureValues(configFileContent, maximumDurationRx);

            if (captureValues.Count != 0)
            {
                return float.Parse(captureValues.First());
            }

            return -1;
        }
    }
}
