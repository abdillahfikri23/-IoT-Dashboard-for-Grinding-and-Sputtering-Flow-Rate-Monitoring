using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MachineMonitoring
{
    public partial class FormCalibration : Form
    {
        public FormCalibration()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            Calibration calibration = new Calibration();
            Security security = new Security();
            security.ShowDialog();
            if (security.IsAuthenticated)
            {
                try
                {
                    //Grinding
                    double grflow1_A = double.Parse(tb_grfr1_A.Text);
                    double grflow1_B = double.Parse(tb_grfr1_B.Text);
                    double grflow2_A = double.Parse(tb_grfr2_A.Text);
                    double grflow2_B = double.Parse(tb_grfr2_B.Text);
                    double grflow3_A = double.Parse(tb_grfr3_A.Text);
                    double grflow3_B = double.Parse(tb_grfr3_B.Text);
                    double grflow4_A = double.Parse(tb_grfr4_A.Text);
                    double grflow4_B = double.Parse(tb_grfr4_B.Text);
                    double grflow5_A = double.Parse(tb_grfr5_A.Text);
                    double grflow5_B = double.Parse(tb_grfr5_B.Text);
                    double grflow6_A = double.Parse(tb_grfr6_A.Text);
                    double grflow6_B = double.Parse(tb_grfr6_B.Text);

                    //Sputtering
                    double spflow1_A = double.Parse(tb_spfr1_A.Text);
                    double spflow1_B = double.Parse(tb_spfr1_B.Text);
                    double spflow2_A = double.Parse(tb_spfr2_A.Text);
                    double spflow2_B = double.Parse(tb_spfr2_B.Text);
                    double spflow3_A = double.Parse(tb_spfr3_A.Text);
                    double spflow3_B = double.Parse(tb_spfr3_B.Text);
                    double spflow4_A = double.Parse(tb_spfr4_A.Text);
                    double spflow4_B = double.Parse(tb_spfr4_B.Text);
                    double spflow5_A = double.Parse(tb_spfr5_A.Text);
                    double spflow5_B = double.Parse(tb_spfr5_B.Text);
                    double spflow6_A = double.Parse(tb_spfr6_A.Text);
                    double spflow6_B = double.Parse(tb_spfr6_B.Text);

                    // Simpan nilai ke Settings
                    Calibration.Default.grflow1_A = grflow1_A;
                    Calibration.Default.grflow1_B = grflow1_B;
                    Calibration.Default.grflow2_A = grflow2_A;
                    Calibration.Default.grflow2_B = grflow2_B;
                    Calibration.Default.grflow3_A = grflow3_A;
                    Calibration.Default.grflow3_B = grflow3_B;
                    Calibration.Default.grflow4_A = grflow4_A;
                    Calibration.Default.grflow4_B = grflow4_B;
                    Calibration.Default.grflow5_A = grflow5_A;
                    Calibration.Default.grflow5_B = grflow5_B;
                    Calibration.Default.grflow6_A = grflow6_A;
                    Calibration.Default.grflow6_B = grflow6_B;
                    Calibration.Default.Save();

                    // Simpan nilai ke Settings
                    Calibration.Default.spflow1_A = spflow1_A;
                    Calibration.Default.spflow1_B = spflow1_B;
                    Calibration.Default.spflow2_A = spflow2_A;
                    Calibration.Default.spflow2_B = spflow2_B;
                    Calibration.Default.spflow3_A = spflow3_A;
                    Calibration.Default.spflow3_B = spflow3_B;
                    Calibration.Default.spflow4_A = spflow4_A;
                    Calibration.Default.spflow4_B = spflow4_B;
                    Calibration.Default.spflow5_A = spflow5_A;
                    Calibration.Default.spflow5_B = spflow5_B;
                    Calibration.Default.spflow6_A = spflow6_A;
                    Calibration.Default.spflow6_B = spflow6_B;

                    // Simpan perubahan
                    Calibration.Default.Save();

                    MessageBox.Show("Values saved successfully!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void FormCalibration_Load(object sender, EventArgs e)
        {
            tb_grfr1_A.Text = Calibration.Default.grflow1_A.ToString();
            tb_grfr1_B.Text = Calibration.Default.grflow1_B.ToString();
            tb_grfr2_A.Text = Calibration.Default.grflow2_A.ToString();
            tb_grfr2_B.Text = Calibration.Default.grflow2_B.ToString();
            tb_grfr3_A.Text = Calibration.Default.grflow3_A.ToString();
            tb_grfr3_B.Text = Calibration.Default.grflow3_B.ToString();
            tb_grfr4_A.Text = Calibration.Default.grflow4_A.ToString();
            tb_grfr4_B.Text = Calibration.Default.grflow4_B.ToString();
            tb_grfr5_A.Text = Calibration.Default.grflow5_A.ToString();
            tb_grfr5_B.Text = Calibration.Default.grflow5_B.ToString();
            tb_grfr6_A.Text = Calibration.Default.grflow6_A.ToString();
            tb_grfr6_B.Text = Calibration.Default.grflow6_B.ToString();

            tb_spfr1_A.Text = Calibration.Default.spflow1_A.ToString();
            tb_spfr1_B.Text = Calibration.Default.spflow1_B.ToString();
            tb_spfr2_A.Text = Calibration.Default.spflow2_A.ToString();
            tb_spfr2_B.Text = Calibration.Default.spflow2_B.ToString();
            tb_spfr3_A.Text = Calibration.Default.spflow3_A.ToString();
            tb_spfr3_B.Text = Calibration.Default.spflow3_B.ToString();
            tb_spfr4_A.Text = Calibration.Default.spflow4_A.ToString();
            tb_spfr4_B.Text = Calibration.Default.spflow4_B.ToString();
            tb_spfr5_A.Text = Calibration.Default.spflow5_A.ToString();
            tb_spfr5_B.Text = Calibration.Default.spflow5_B.ToString();
            tb_spfr6_A.Text = Calibration.Default.spflow6_A.ToString();
            tb_spfr6_B.Text = Calibration.Default.spflow6_B.ToString();
        }
    }
}
