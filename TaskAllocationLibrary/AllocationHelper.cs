using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskAllocationLibrary
{
    public static class AllocationHelper
    {
        /// <summary>
        /// Computes the energy consumed by a task allocated to an NGHz processor
        /// </summary>
        /// <returns>
        /// The energy consumed by a task
        /// </returns>
        /// <param name="coefficients">Coefficients of a quadratic formulas</param>
        /// <param name="frequency">Processor frequency</param>
        /// <param name="runtime">Runtime of the task</param>
        public static float GetEnergyConsumedPerTask(List<float> coefficients, float frequency, float runtime)
        {
            return (coefficients[2] * frequency * frequency + coefficients[1] * frequency + coefficients[0]) * runtime;
        }
    }
}
