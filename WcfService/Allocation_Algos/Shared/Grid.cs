using System;
using System.Collections.Generic;
using System.Linq;

namespace WcfService.Allocation_Algos.Shared
{
    public class Grid
    {
        private Dictionary<string, Dictionary<string, float>> content;

        public Grid(Dictionary<string, float> tasks, Dictionary<string, float> processors, float refFrequency)
        {
            // {[taskId: string]: {
            //    [processorId: string]: taskRuntimeOnThatProcessor // in ASCENDING order
            // }
            Dictionary<string, Dictionary<string, float>> grid = new Dictionary<string, Dictionary<string, float>>();

            foreach (var task in tasks) {
                Dictionary<string, float> gridRow = new Dictionary<string, float>();

                foreach (var processor in processors) {
                    float taskRuntime = task.Value * (refFrequency / processor.Value);
                    gridRow.Add(processor.Key, taskRuntime);
                }

                grid.Add(task.Key, gridRow);
            }

            this.content = grid;
            this.sortGrid();
        }

        public void RemoveRow(string taskId)
        {
            this.content.Remove(taskId);
        }

        public float GetRuntime(string taskId, string processorId)
        {
            return this.content[taskId][processorId];
        }

        private void sortGrid()
        {
            Dictionary<string, Dictionary<string, float>> sortedGrid = new Dictionary<string, Dictionary<string, float>>();

            foreach (var taskRow in this.content) {
                Dictionary<string, float> sortedRowByCompletionTime = taskRow.Value.OrderBy(r => r.Value)
                      .ToDictionary(c => c.Key as string, d => Convert.ToSingle(d.Value));

                sortedGrid.Add(taskRow.Key, sortedRowByCompletionTime);
            }

            this.content = sortedGrid;
        }

        public void UpdateProcessorRuntime(string processorId, float increaseBy)
        {
            var tmp = new Dictionary<string, Dictionary< string, float>>(this.content);
            foreach (var gridRow in tmp) {
                var updatedRowValue = gridRow.Value;
                updatedRowValue[processorId] += increaseBy;
                this.content[gridRow.Key] = updatedRowValue;
            }
            this.sortGrid();
        }

        public Dictionary<string, Dictionary<string, float>> Content { get => content; set => content = value; }
    }


}