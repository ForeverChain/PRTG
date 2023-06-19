using PrtgAPI;
using PrtgAPI.Parameters;
using PrtgAPI.Request;
using System.Threading;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private PrtgClient prtgClient;
        public CancellationToken cancellationToken = CancellationToken.None;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Code executed when the form is closing
            // Disconnect from the PRTG server and clean up resources
            prtgClient = null;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            string prtgServerUrl = "http://127.0.0.1/";
            string username = "prtgadmin";
            string password = "prtgadmin";

            prtgClient = new PrtgClient(prtgServerUrl, username, password);

            try
            {
                var devices = await prtgClient.GetDevicesAsync();
                int deviceCount = devices.Count;
                label1.Text = "Total devices: " + deviceCount;

                int downCount = 0;
                int warningCount = 0;
                int upCount = 0;


                foreach (var device in devices)
                {
                    var sensors = await prtgClient.GetSensorsAsync(Property.ParentId, device.Id);
                    foreach (var sensor in sensors)
                    {
                        if (sensor.Status == Status.Down)
                        {
                            downCount++;
                        }
                        else if (sensor.Status == Status.Warning)
                        {
                            warningCount++;
                        }
                        else if (sensor.Status == Status.Up)
                        {
                            upCount++;
                        }
                    }
                }

                label2.Text = string.Format("Sensors: {0} Up, {1} Warning, {2} Down", upCount, warningCount, downCount);
            }
            catch (Exception ex)
            {
                // Code to execute if there is an exception
                Console.WriteLine("Error connecting to PRTG server: " + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }
    }
}
