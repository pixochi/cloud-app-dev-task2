using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskAllocationLibrary;

namespace GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void startReading()
        {
            mainFormOutput.Text = "Processing...";
        }

        private void clearOutput()
        {
            mainFormOutput.Text = "";
        }

        private void printRow(string text = "")
        {
            mainFormOutput.Text += $"{text}{Environment.NewLine}";
        }

        private void printWaitingTime()
        {
            mainFormOutput.Text += $"Please wait for 5 minutes...";
        }

        private void printEndpointAddress(ServiceEndpoint endpoint)
        {
            this.printRow(endpoint.Address.ToString());
        }

        // Get allocations from services on button-click
        private async void getAllocationsButton_Click(object sender, EventArgs e)
        {
            this.startReading();
            string fileUrlInputBox = urlInputBox.Text.Trim();
            string fileUrlComboxBox = filePathsComboBox.Text.Trim();
            string fileUrl = String.IsNullOrEmpty(fileUrlInputBox) ? fileUrlComboxBox : "";

            try {
                string fileContent = FileReader.ReadFromUrl(fileUrl);
                this.clearOutput();

                ConfigFile configFile = new ConfigFile(fileContent);
                var allocInput = new TaskAllocationInput(configFile.Tasks, configFile.Processors, configFile.MaxDuration, configFile.RefFrequency, configFile.Coefficients);
                var allocations = new List<TaskAllocationOutput>();

                // All clients
                var GAServiceClient = new GAServiceReference.GAServiceClient();
                var heuristicClient = new HeuristicServiceReference.HeuristicServiceClient();
                var sortMidClient = new SortMidServiceReference.SortMidServiceClient();

                // Get allocations from GA Service 1
                var GATask = Task.Factory.StartNew(() =>
                {
                    var GAAllocc = GAServiceClient.GetAllocations(allocInput);
                    allocations.AddRange(GAAllocc);
                });

                // Get allocations from Heuristic Service
                var heuristicTask = Task.Factory.StartNew(() =>
                {
                    var heuristicAllocs = heuristicClient.GetAllocations(allocInput);
                    allocations.AddRange(heuristicAllocs);
                });

                // Get allocations from SortMid Service
                var sortMidTask = Task.Factory.StartNew(() =>
                {
                    var sortMidAllocs = sortMidClient.GetAllocations(allocInput);
                    allocations.AddRange(sortMidAllocs);
                });

                // UI feedback
                this.printEndpointAddress(GAServiceClient.Endpoint);
                this.printEndpointAddress(heuristicClient.Endpoint);
                this.printEndpointAddress(sortMidClient.Endpoint);
                this.printWaitingTime();

                await Task.WhenAll(GATask, heuristicTask, sortMidTask);

                // Print correct allocations
                var topAllocations = allocations.OrderBy(a => a.EnergyConsumed).Take(3).ToList();
                string formattedAllocations = Visualizer.Allocations(topAllocations);
                this.printRow();
                this.printRow(formattedAllocations);

            }
            catch (Exception ex) {
                this.clearOutput();
                this.printRow(ex.Message);
            }
        }

        // Select the first config file by default
        private void MainForm_Load(object sender, EventArgs e)
        {
            filePathsComboBox.SelectedIndex = 0;
        }
    }
}
