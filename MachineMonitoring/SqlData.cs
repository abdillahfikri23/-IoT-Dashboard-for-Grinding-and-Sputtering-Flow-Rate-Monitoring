using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace MachineMonitoring
{
    class SqlData
    {
        string connectionString = "Data Source=etcbtmsql01\\digitmont;Initial Catalog=dbDigitmontDetection;User ID=Metis;Password=Metis123";

        //Variabel Global
        public int year { get; set; }
        public int date { get; set; }
        public double flowRate { get; set; }
        public double tension { get; set; }
        public double temperature { get; set; }
        public double noise { get; set; }

        // SQL SERVER DATABASE
        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            DataTable dt = new DataTable();
            try
            {
                string sql = "SELECT * FROM TESTER_APPLICATION_TUBE";
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

        public bool InsertData(SqlData data)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                string sql = "INSERT INTO TESTER_APPLICATION_TUBE (YEAR, DATE, MACHINE_NUMBER, DEVICE_TYPE, SERIAL_NUMBER, SHIFT, START, FINISH, TEST_JIG, BB_TEMP_SETTING, GS_READING, DUT1_READING, DUT2_READING, DUT3_READING, AVG_READING, DV_READING, RANGE_READING, MC_OFFSET_OLD, MC_OFFSET_NEW, BUYOFF_RESULT, DONE_BY) VALUES (@Year, @Date, @MachineNumber, @DeviceType, @SerialNumber, @Shift, @Start, @Finish, @TestJig, @TempSetting, @Gs_Reading, @Dut1_Reading, @Dut2_Reading, @Dut3_Reading, @Avg_Reading, @Dv_Reading, @Range_Reading, @Mc_OffsetOld, @Mc_OffsetNew, @BuyoffResult, @DoneBy)";
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Add parameters
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Date", date);

                conn.Open();
                int rows1 = cmd.ExecuteNonQuery();
                if (rows1 > 0)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return isSuccess;
        }
    }
}