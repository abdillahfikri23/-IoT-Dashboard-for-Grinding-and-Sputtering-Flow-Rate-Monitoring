using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using Sres.Net.EEIP;
using System.Threading;
using MachineMonitoring.Properties;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace MachineMonitoring
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll")]
        private static extern uint SetThreadExecutionState(uint esFlags);

        // Define execution state flags
        const uint ES_CONTINUOUS = 0x80000000;
        const uint ES_SYSTEM_REQUIRED = 0x00000001;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;

        private BackgroundWorker backgroundWorker;
        private string ipAddress;
        private int port;
        DataSql datasql = new DataSql();

        bool isGrindingOn = false;
        bool isSputteringOn = false;

        //Variabel Global
        public double SG3WheelCoolant { get; set; }
        public double SG3Chiller { get; set; }
        public double SG5WheelCoolant { get; set; }
        public double SG5Chiller { get; set; }
        public double SG6WheelCoolant { get; set; }
        public double SG6Chiller { get; set; }
        public double SP3TurboPump1 { get; set; }
        public double SP3TurboPump2 { get; set; }
        public double SP3TurboPump3 { get; set; }
        public double SP3Top { get; set; }
        public double SP3Bottom { get; set; }
        public double SP3Noise { get; set; }

        public double SG3WheelCoolant1 { get; set; }
        public double SG3Chiller1 { get; set; }
        public double SG5WheelCoolant1 { get; set; }
        public double SG5Chiller1 { get; set; }
        public double SG6WheelCoolant1 { get; set; }
        public double SG6Chiller1 { get; set; }
        public double SP3TurboPump11 { get; set; }
        public double SP3TurboPump21 { get; set; }
        public double SP3TurboPump31 { get; set; }
        public double SP3Top1 { get; set; }
        public double SP3Bottom1 { get; set; }
        public double SP3Noise1 { get; set; }

        //GRINDING
        public Int16[] grindingAnalogCH = new Int16[6];

        //SPUTTERING
        public Int16[] sputteringAnalogCH = new Int16[6];

        EEIPClient client = new EEIPClient();
        EEIPClient eeipGrinding = new EEIPClient();
        EEIPClient eeipSputtering = new EEIPClient();
        
        public Form1(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public Form1()
        {
            InitializeComponent();
            this.ipAddress = GetLocalIPAddress2();
            this.port = 2323;
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Prevent the system from sleeping
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
            //MessageBox.Show("PC will not sleep while this application is running.", "Prevent Sleep", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            
            eeipSputtering.IPAddress = "169.254.0.16";
            eeipSputtering.RegisterSession();

            eeipGrinding.IPAddress = "169.254.0.15";
            eeipGrinding.RegisterSession();

            //MENU
            this.Menu = new MainMenu();
            MenuItem item1 = new MenuItem("MENU");
            this.Menu.MenuItems.Add(item1);
            item1.MenuItems.Add("CALIBRATION_DATA", new EventHandler(Calibration_Click));
            item1.MenuItems.Add("SQL_DATABASE", new EventHandler(SQL_Click));
            datasql.StartPeriodicTask();
        }

        private void Calibration_Click(object sender, EventArgs e)
        {
            FormCalibration calibrationform = new FormCalibration();
            calibrationform.Show();
        }

        private void SQL_Click(object sender, EventArgs e)
        {
            DataSql datasql = new DataSql();
            datasql.Show();
        }

        private string GetLocalIPAddress()
        {
            string ipAddress = "";
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Prioritaskan hanya Wi-Fi network interfaces
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                        {
                            ipAddress = ip.Address.ToString();
                            return ipAddress; // Mengembalikan IP address dari Wi-Fi
                        }
                    }
                }
            }

            // Jika tidak ada Wi-Fi, bisa mencoba Ethernet atau mengembalikan IP kosong
            return ipAddress;
        }

        // Method untuk mendapatkan IP address secara otomatis
        private string GetLocalIPAddress2()
        {
            string ipAddress = "";
            bool isConnected = false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) && ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                        {
                            ipAddress = ip.Address.ToString();
                            isConnected = true;
                            return ipAddress;
                        }
                    }
                }
            }

            if (!isConnected)
            {
                MessageBox.Show("The PC is not connected to any Wi-Fi or Ethernet. Please check the network connection.", "Network Connection Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }

            return ipAddress;
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            // Jalankan server di Task asynchronous
            Task.Run(() =>
            {
                Server server = new Server(this);
                server.Start();
            });

            while (!worker.CancellationPending)
            {
                try
                {
                    analogReading();
                    calibrationValue();

                    SG3WheelCoolant1 = Math.Round(SG3WheelCoolant, 2);
                    SG3Chiller1 = Math.Round(SG3Chiller, 2);
                    SG5WheelCoolant1 = Math.Round(SG5WheelCoolant, 2);
                    SG5Chiller1 = Math.Round(SG5Chiller, 2);
                    SG6WheelCoolant1 = Math.Round(SG6WheelCoolant, 2);
                    SG6Chiller1 = Math.Round(SG6Chiller, 2);
                    SP3TurboPump11 = Math.Round(SP3TurboPump1, 2);
                    SP3TurboPump21 = Math.Round(SP3TurboPump2, 2);
                    SP3TurboPump31 = Math.Round(SP3TurboPump3, 2);
                    SP3Top1 = Math.Round(SP3Top, 2);
                    SP3Bottom1 = Math.Round(SP3Bottom, 2);
                    SP3Noise1 = Math.Round(SP3Noise, 2);

                    worker.ReportProgress(0, SG3WheelCoolant1);
                    worker.ReportProgress(0, SG3Chiller1);
                    worker.ReportProgress(0, SG5WheelCoolant1);
                    worker.ReportProgress(0, SG5Chiller1);
                    worker.ReportProgress(0, SG6WheelCoolant1);
                    worker.ReportProgress(0, SG6Chiller1);
                    worker.ReportProgress(0, SP3TurboPump11);
                    worker.ReportProgress(0, SP3TurboPump21);
                    worker.ReportProgress(0, SP3TurboPump31);
                    worker.ReportProgress(0, SP3Top1);
                    worker.ReportProgress(0, SP3Bottom1);
                    worker.ReportProgress(0, SP3Noise1);

                    datasql.SG3WC = SG3WheelCoolant1;
                    datasql.SG3C = SG3Chiller1;
                    datasql.SG5WC = SG5WheelCoolant1;
                    datasql.SG5C = SG5Chiller1;
                    datasql.SG6WC = SG6WheelCoolant1;
                    datasql.SG6C = SG6Chiller1;
                    datasql.SP3TP1 = SP3TurboPump11;
                    datasql.SP3TP2 = SP3TurboPump21;
                    datasql.SP3TP3 = SP3TurboPump31;
                    datasql.SP3T = SP3Top1;
                    datasql.SP3B = SP3Bottom1;
                    datasql.AddData();
                }
                catch (Exception ex)
                {
                    worker.ReportProgress(0, "Error: " + ex.Message);
                }

                System.Threading.Thread.Sleep(1000); 

                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                if (e.UserState is string)
                {
                    string errorMessage = (string)e.UserState;
                }
                else
                {
                    int grfr1_X = ch_GRFR1.Series[0].Points.Count + 1;
                    int grfr2_X = ch_GRFR2.Series[0].Points.Count + 1;
                    int grfr3_X = ch_GRFR3.Series[0].Points.Count + 1;
                    int grfr4_X = ch_GRFR4.Series[0].Points.Count + 1;
                    int grfr5_X = ch_GRFR5.Series[0].Points.Count + 1;
                    int grfr6_X = ch_GRFR6.Series[0].Points.Count + 1;

                    int spfr1_X = ch_SPFR1.Series[0].Points.Count + 1;
                    int spfr2_X = ch_SPFR2.Series[0].Points.Count + 1;
                    int spfr3_X = ch_SPFR3.Series[0].Points.Count + 1;
                    int spfr4_X = ch_SPFR4.Series[0].Points.Count + 1;

                    ch_GRFR1.Series["SG"].Points.AddXY(grfr1_X, SG3WheelCoolant1);
                    ch_GRFR2.Series["SG"].Points.AddXY(grfr2_X, SG3Chiller1);
                    ch_GRFR3.Series["SG"].Points.AddXY(grfr3_X, SG5WheelCoolant1);
                    ch_GRFR4.Series["SG"].Points.AddXY(grfr4_X, SG5Chiller1);
                    ch_GRFR5.Series["SG"].Points.AddXY(grfr5_X, SG6WheelCoolant1);
                    ch_GRFR6.Series["SG"].Points.AddXY(grfr6_X, SG6Chiller1);

                    ch_SPFR1.Series["SP"].Points.AddXY(spfr1_X, SP3TurboPump11);
                    ch_SPFR2.Series["SP"].Points.AddXY(spfr2_X, SP3TurboPump21);
                    ch_SPFR3.Series["SP"].Points.AddXY(spfr3_X, SP3TurboPump31);
                    ch_SPFR4.Series["SP"].Points.AddXY(spfr4_X, SP3Top1);

                    lb_grfr1.Text = SG3WheelCoolant.ToString("N2");
                    lb_grfr2.Text = SG3Chiller.ToString("N2");
                    lb_grfr3.Text = SG5WheelCoolant.ToString("N2");
                    lb_grfr4.Text = SG5Chiller.ToString("N2");
                    lb_grfr5.Text = SG6WheelCoolant.ToString("N2");
                    lb_grfr6.Text = SG6Chiller.ToString("N2");

                    lb_spfr1.Text = SP3TurboPump1.ToString("N2");
                    lb_spfr2.Text = SP3TurboPump2.ToString("N2");
                    lb_spfr3.Text = SP3TurboPump3.ToString("N2");
                    lb_spfr4.Text = SP3Top.ToString("N2");

                    RemovePointChart(ch_GRFR1.Series["SG"], grfr1_X, SG3WheelCoolant);
                    RemovePointChart(ch_GRFR2.Series["SG"], grfr2_X, SG3Chiller);
                    RemovePointChart(ch_GRFR3.Series["SG"], grfr3_X, SG5WheelCoolant);
                    RemovePointChart(ch_GRFR4.Series["SG"], grfr4_X, SG5Chiller);
                    RemovePointChart(ch_GRFR5.Series["SG"], grfr5_X, SG6WheelCoolant);
                    RemovePointChart(ch_GRFR6.Series["SG"], grfr6_X, SG6Chiller);

                    RemovePointChart(ch_SPFR1.Series["SP"], spfr1_X, SP3TurboPump1);
                    RemovePointChart(ch_SPFR2.Series["SP"], spfr2_X, SP3TurboPump2);
                    RemovePointChart(ch_SPFR3.Series["SP"], spfr3_X, SP3TurboPump3);
                    RemovePointChart(ch_SPFR4.Series["SP"], spfr4_X, SP3Top);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Data retrieval cancelled.");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }
            else
            {
                MessageBox.Show("Data retrieval completed.");
            }
        }

        private void bt_Stop_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy && backgroundWorker.WorkerSupportsCancellation) // Cek apakah mendukung pembatalan
            {
                backgroundWorker1.CancelAsync();
            }
        }

        private void bt_Start_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy && backgroundWorker1.WorkerSupportsCancellation)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            else if (backgroundWorker1.IsBusy)
            {
                MessageBox.Show("Server is Deactivated");
            }
        }

        public void analogReading()
        {
            //Read out analog input
            byte[] resultGrinding = eeipGrinding.AssemblyObject.getInstance(100);

            grindingAnalogCH[0] = (short)((resultGrinding[71] * 255) + resultGrinding[70]);
            grindingAnalogCH[1] = (short)((resultGrinding[81] * 255) + resultGrinding[80]);
            grindingAnalogCH[2] = (short)((resultGrinding[91] * 255) + resultGrinding[90]);
            grindingAnalogCH[3] = (short)((resultGrinding[101] * 255) + resultGrinding[100]);
            grindingAnalogCH[4] = (short)((resultGrinding[115] * 255) + resultGrinding[114]);
            grindingAnalogCH[5] = (short)((resultGrinding[125] * 255) + resultGrinding[124]);

            Console.WriteLine("============================ Grinding Read out analog input ============================");
            Console.WriteLine("S3_Wheel_Coolant : {0}", grindingAnalogCH[0]);
            Console.WriteLine("S3_Chiller       : {0}", grindingAnalogCH[1]);
            Console.WriteLine("S5_Wheel_Coolant : {0}", grindingAnalogCH[2]);
            Console.WriteLine("S5_Chiller       : {0}", grindingAnalogCH[3]);
            Console.WriteLine("S6_Wheel_Coolant : {0}", grindingAnalogCH[4]);
            Console.WriteLine("S6_Chiller       : {0}", grindingAnalogCH[5]);

            /*
            for (int i = 0; i < resultGrinding.Length; i++)
            {
                Console.WriteLine("Byte {0}: Data Grinding : {1}", i, resultGrinding[i]);
            }*/

            System.Threading.Thread.Sleep(100);
            
            byte[] resultSputtering = eeipSputtering.AssemblyObject.getInstance(100);

            sputteringAnalogCH[0] = (short)((resultSputtering[71] * 255) + resultSputtering[70]); 
            sputteringAnalogCH[1] = (short)((resultSputtering[81] * 255) + resultSputtering[80]);
            sputteringAnalogCH[2] = (short)((resultSputtering[91] * 255) + resultSputtering[90]);
            sputteringAnalogCH[3] = (short)((resultSputtering[101] * 255) + resultSputtering[100]);
            sputteringAnalogCH[4] = (short)((resultSputtering[115] * 255) + resultSputtering[114]);
            sputteringAnalogCH[5] = (short)((resultSputtering[125] * 255) + resultSputtering[124]);

            Console.WriteLine("============================ Sputtering Read out analog input ============================");
            Console.WriteLine("SP3_Turbo_Pump1 : {0}", sputteringAnalogCH[0]);
            Console.WriteLine("SP3_Turbo_Pump2 : {0}", sputteringAnalogCH[1]);
            Console.WriteLine("SP3_Turbo_Pump3 : {0}", sputteringAnalogCH[2]);
            Console.WriteLine("SP3_Top         : {0}", sputteringAnalogCH[3]);
            Console.WriteLine("SP3_Bottom      : {0}", sputteringAnalogCH[4]);
            Console.WriteLine("SP3_Noise       : {0}", sputteringAnalogCH[5]);

            /*
            for (int i = 0; i < resultSputtering.Length; i++)
            {
                Console.WriteLine("Byte {0}: Data Sputtering : {1}", i, resultSputtering[i]);
            }*/
        }

        public void calibrationValue()
        {
            //GRINDING
            SG3WheelCoolant = ((Calibration.Default.grflow1_A * grindingAnalogCH[0]) + Calibration.Default.grflow1_B);
            SG3Chiller = ((Calibration.Default.grflow2_A * grindingAnalogCH[1]) + Calibration.Default.grflow2_B);
            SG5WheelCoolant = ((Calibration.Default.grflow3_A * grindingAnalogCH[2]) + Calibration.Default.grflow3_B);
            SG5Chiller = ((Calibration.Default.grflow4_A * grindingAnalogCH[3]) + Calibration.Default.grflow4_B);
            SG6WheelCoolant = ((Calibration.Default.grflow5_A * grindingAnalogCH[4]) + Calibration.Default.grflow5_B);
            SG6Chiller = ((Calibration.Default.grflow6_A * grindingAnalogCH[5]) + Calibration.Default.grflow6_B);

            //SPUTTERING
            SP3TurboPump1 = ((Calibration.Default.spflow1_A * sputteringAnalogCH[0]) + Calibration.Default.spflow1_B);
            SP3TurboPump2 = ((Calibration.Default.spflow2_A * sputteringAnalogCH[1]) + Calibration.Default.spflow2_B);
            SP3TurboPump3 = ((Calibration.Default.spflow3_A * sputteringAnalogCH[2]) + Calibration.Default.spflow3_B);
            SP3Top = ((Calibration.Default.spflow4_A * sputteringAnalogCH[3]) + Calibration.Default.spflow4_B);
            SP3Bottom = ((Calibration.Default.spflow5_A * sputteringAnalogCH[4]) + Calibration.Default.spflow5_B);
            SP3Noise = ((Calibration.Default.spflow6_A * sputteringAnalogCH[5]) + Calibration.Default.spflow6_B);
        }

        /*
        int maxPoints = 1000;
        void RemovePointChart(Series series, double xValue, double yValue)
        {
            // Add the new point
            series.Points.AddXY(xValue, yValue);

            // Remove the first point if the series exceeds the maximum number of points
            if (series.Points.Count > maxPoints)
            {
                series.Points.RemoveAt(0);
                //series.Points.Clear();
            }
        }*/

        int maxPoints = 1000;
        int resetThreshold = 5000; // Misalnya, reset chart setelah 5000 point
        void RemovePointChart(Series series, double xValue, double yValue)
        {
            // Add the new point
            series.Points.AddXY(xValue, yValue);

            // Reset the chart if the series exceeds the resetThreshold number of points
            if (series.Points.Count > resetThreshold)
            {
                series.Points.Clear();
            }
            else if (series.Points.Count > maxPoints)
            {
                // Remove the first point if the series exceeds the maximum number of points
                series.Points.RemoveAt(0);
            }
        }
    }
}
