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
    public partial class Security : Form
    {
        public bool IsAuthenticated { get; private set; }
        public Security()
        {
            InitializeComponent();
            IsAuthenticated = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        string userName = "waterflowiot";
        string passWord = "flowrate238";

        private void bt_Enter_Click(object sender, EventArgs e)
        {
            if ( tb_user.Text == userName && tb_pass.Text == passWord)
            {
                IsAuthenticated = true;
                this.Close();
            }
            else if (tb_user.Text != userName && tb_pass.Text != passWord)
            {
                MessageBox.Show("Incorrect Username and Password, Please Try Again!!!");
            }
            else if(tb_user.Text != userName)
            {
                MessageBox.Show("Incorrect Username, Please Try Again!!!");
            }
            else if(tb_pass.Text != passWord)
            {
                MessageBox.Show("Incorrect Password, Please Try Again!!!");
            }
        }

        private void Security_Load(object sender, EventArgs e)
        {

        }
    }
}
