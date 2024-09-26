using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using System.Data.SqlClient;

namespace MachineMonitoring
{
    public partial class DataSql : Form
    {
        // SQL SERVER DATABASE
        private DataTable datatable;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        string connectionString = "data source=" + "BTMSQL01\\DIGITMONT_DEV" +
            ";Persist Security Info=false;database=" + "dbIOT_WireSaw" +
            ";user id=" + "iot_user" + ";password=" +
            "iot123" + ";Connection Timeout = 5";

        public double SG3WC;
        public double SG3C;
        public double SG5WC;
        public double SG5C;
        public double SG6WC;
        public double SG6C;
        public double SP3TP1;
        public double SP3TP2;
        public double SP3TP3;
        public double SP3T;
        public double SP3B;
        public int id = 0;

        public DataSql()
        {
            CreateTabelData();
            InitializeComponent();
            StartPeriodicTask();
        }

        public void CreateTabelData()
        {
            datatable = new DataTable();
            datatable.Columns.Add("Id", typeof(int));
            datatable.Columns.Add("Year", typeof(int));
            datatable.Columns.Add("Date", typeof(string));
            datatable.Columns.Add("SG3WheelCoolant", typeof(double));
            datatable.Columns.Add("SG3Chiller", typeof(double));
            datatable.Columns.Add("SG5WheelCoolant", typeof(double));
            datatable.Columns.Add("SG5Chiller", typeof(double));
            datatable.Columns.Add("SG6WheelCoolant", typeof(double));
            datatable.Columns.Add("SG6Chiller", typeof(double));
            datatable.Columns.Add("SP3TurboPump1", typeof(double));
            datatable.Columns.Add("SP3TurboPump2", typeof(double));
            datatable.Columns.Add("SP3TurboPump3", typeof(double));
            datatable.Columns.Add("SP3Top", typeof(double));
            datatable.Columns.Add("SP3Bottom", typeof(double));
        }

        public void AddData()
        {
            DataRow row = datatable.NewRow();
            DateTime currentDate = DateTime.Now;

            id++;
            row["Id"] = id;
            row["Year"] = currentDate.Year;
            row["Date"] = currentDate;
            row["SG3WheelCoolant"] = SG3WC;
            row["SG3Chiller"] = SG3C;
            row["SG5WheelCoolant"] = SG5WC;
            row["SG5Chiller"] = SG5C;
            row["SG6WheelCoolant"] = SG6WC;
            row["SG6Chiller"] = SG6C;
            row["SP3TurboPump1"] = SP3TP1;
            row["SP3TurboPump2"] = SP3TP2;
            row["SP3TurboPump3"] = SP3TP3;
            row["SP3Top"] = SP3T;
            row["SP3Bottom"] = SP3B;
            datatable.Rows.Add(row);
            
            /*
            // Gabungkan semua data menjadi satu string
            StringBuilder allData = new StringBuilder();
            foreach (DataRow dataRow in datatable.Rows)
            {
                string rowData = string.Join(", ", dataRow.ItemArray);
                allData.AppendLine(rowData);
            }

            // Tampilkan semua data di MessageBox
            MessageBox.Show("Semua Data:\n" + allData.ToString());*/
        }

        private void DataSql_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Select();
        }

        public async void StartPeriodicTask()
        {
            while (true)
            {
                await Task.Delay(5 * 60 * 1000); // Tunggu 5 menit
                //SendDataToSql1(); // Panggil fungsi Anda

                await _semaphore.WaitAsync();
                try
                {
                    await SendDataToSql1(); // Panggil fungsi Anda
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        private async Task SendDataToSql1()
        {
            if (datatable.Rows.Count > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "dtFlowrateGrindingSputtering";

                        // Add column mappings if needed
                        bulkCopy.ColumnMappings.Add("Id", "ID");
                        bulkCopy.ColumnMappings.Add("Year", "Year");
                        bulkCopy.ColumnMappings.Add("Date", "Datetime");
                        bulkCopy.ColumnMappings.Add("SG3WheelCoolant", "SG3_Wheel_Coolant");
                        bulkCopy.ColumnMappings.Add("SG3Chiller", "SG3_Chiller");
                        bulkCopy.ColumnMappings.Add("SG5WheelCoolant", "SG5_Wheel_Coolant");
                        bulkCopy.ColumnMappings.Add("SG5Chiller", "SG5_Chiller");
                        bulkCopy.ColumnMappings.Add("SG6WheelCoolant", "SG6_Wheel_Coolant");
                        bulkCopy.ColumnMappings.Add("SG6Chiller", "SG6_Chiller");
                        bulkCopy.ColumnMappings.Add("SP3TurboPump1", "SP3_Turbo_Pump1");
                        bulkCopy.ColumnMappings.Add("SP3TurboPump2", "SP3_Turbo_Pump2");
                        bulkCopy.ColumnMappings.Add("SP3TurboPump3", "SP3_Turbo_Pump3");
                        bulkCopy.ColumnMappings.Add("SP3Top", "SP3_Top");
                        bulkCopy.ColumnMappings.Add("SP3Bottom", "SP3_Bottom");

                        try
                        {
                            bulkCopy.WriteToServer(datatable);
                            datatable.Clear(); // Clear buffer after sending data
                            id = 0;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void SendDataToSql2()
        {
            if (datatable.Rows.Count > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        foreach (DataRow row in datatable.Rows)
                        {
                            string query = "INSERT INTO dtFlowrateGrindingSputtering (Year, Date, SG3WheelCoolant, SG3Chiller, SG5WheelCoolant, SG5Chiller, SG6WheelCoolant, SG6Chiller, SP3TurboPump1, SP3TurboPump2, SP3TurboPump3, SP3Top, SP3Bottom) " +
                                           "VALUES (@Year, @Datetime, @SG3_Wheel_Coolant, @SG3_Chiller, @SG5_Wheel_Coolant, @SG5_Chiller, @SG6_Wheel_Coolant, @SG6_Chiller, @SP3_Turbo_Pump1, @SP3_Turbo_Pump2, @SP3_Turbo_Pump3, @SP3_Top, @SP3_Bottom)";

                            using (SqlCommand cmd = new SqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("@Year", row["Year"]);
                                cmd.Parameters.AddWithValue("@Datetime", row["Date"]);
                                cmd.Parameters.AddWithValue("@SG3_Wheel_Coolant", row["SG3WheelCoolant"]);
                                cmd.Parameters.AddWithValue("@SG3_Chiller", row["SG3Chiller"]);
                                cmd.Parameters.AddWithValue("@SG5_Wheel_Coolant", row["SG5WheelCoolant"]);
                                cmd.Parameters.AddWithValue("@SG5_Chiller", row["SG5Chiller"]);
                                cmd.Parameters.AddWithValue("@SG6_Wheel_Coolant", row["SG6WheelCoolant"]);
                                cmd.Parameters.AddWithValue("@SG6_Chiller", row["SG6Chiller"]);
                                cmd.Parameters.AddWithValue("@SP3_Turbo_Pump1", row["SP3TurboPump1"]);
                                cmd.Parameters.AddWithValue("@SP3_Turbo_Pump2", row["SP3TurboPump2"]);
                                cmd.Parameters.AddWithValue("@SP3_Turbo_Pump3", row["SP3TurboPump3"]);
                                cmd.Parameters.AddWithValue("@SP3_Top", row["SP3Top"]);
                                cmd.Parameters.AddWithValue("@SP3_Bottom", row["SP3Bottom"]);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saat mengirim data ke SQL Server: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close(); // Tutup koneksi setelah selesai mengirim data
                    }
                }
            }

            // Kosongkan DataTable setelah mengirim data ke SQL Server
            datatable.Clear();
        }

        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection();

            conn.ConnectionString = "data source=" + "BTMSQL01\\DIGITMONT_DEV" +
            ";Persist Security Info=false;database=" + "dbIOT_WireSaw" +
            ";user id=" + "iot_user" + ";password=" +
            "iot123" + ";Connection Timeout = 5";

            DataTable dt = new DataTable();
            try
            {
                string sql = "SELECT * FROM dtFlowrateGrindingSputtering";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }
    }
}
