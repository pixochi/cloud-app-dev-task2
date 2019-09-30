using System.Linq;

namespace SortMidService.SortMidAlg
{
    public class TaskAssignment
    {
        private string taskId;
        private string processorId;
        private float maxMidValue;

        public TaskAssignment(Grid sortedGrid)
        {
            maxMidValue = 0;
            if (sortedGrid.Content.Count != 1) {
                foreach (var gridRow in sortedGrid.Content) {
                    var row = new GridRow(gridRow);
                    float rowMidValue = row.GetMidValue();

                    if (rowMidValue > maxMidValue) {
                        maxMidValue = rowMidValue;
                        taskId = row.TaskId;
                        processorId = row.GetFastestProcessorId();
                    }
                }
            }
            // Assign the last unallocated task to an available processor with the minimum completion time
            else {
                taskId = sortedGrid.Content.First().Key;
                processorId = sortedGrid.Content.First().Value.First().Key;
            }
        }

        public string TaskId { get => taskId; set => taskId = value; }
        public string ProcessorId { get => processorId; set => processorId = value; }
    }

}