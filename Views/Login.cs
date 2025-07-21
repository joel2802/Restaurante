using Restaurante.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Restaurante
{
    public partial class Login : Form, ILoginView
    {

        public string Username
        {
            get { return txtuser.Text; }
            set { txtuser.Text = value; }
        }
        string ILoginView.Password
        {
            get { return txtpass.Text; }
            set { txtpass.Text = value; }
        }
        public Login()
        {
            InitializeComponent();
            noshowpass.Hide();
            showpass.Show();
            txtpass.UseSystemPasswordChar = true;



            //activate btnlogin event
            btnlogin.Click += delegate { LoginEvent?.Invoke(this, EventArgs.Empty); };

        }
        


        public event EventHandler LoginEvent;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void noshowpass_Click(object sender, EventArgs e)
        {
            noshowpass.Hide();
            showpass.Show();
            txtpass.UseSystemPasswordChar = true;
        }

        
        private void showpass_Click(object sender, EventArgs e)
        {
            showpass.Hide();
            noshowpass.Show();
            txtpass.UseSystemPasswordChar = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void HideWindow()
        {
            this.Hide();
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void txtuser_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)//Al presionar enter pasa al siguiente cuadro
            {
                txtpass.Focus();
                e.Handled = true; // Esto evita que se genere un 'beep' al presionar Enter
            }
        }

        private void txtpass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnlogin.PerformClick();
                e.Handled = true; // Esto evita que se genere un 'beep' al presionar Enter
            }
        }
        private void btntest_Click(object sender, EventArgs e)
        {
            txtuser.Text = "cesar18";
            txtpass.Text = "12345";
            btnlogin.PerformClick();
        }
        private void btnlogin_Click(object sender, EventArgs e)
        {
          

           
        }
    }
}
