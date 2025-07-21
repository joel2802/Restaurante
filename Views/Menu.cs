using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Restaurante.Presenters;
using Restaurante.Repositories;
using Restaurante.Views;

namespace Restaurante
{
    public partial class Menu : Form 
    {

        private string _user;
        private int tipouser;
        public string nombre
        {
            get { return _user; }
            set { _user = value; }
        }

        public int IdTipoUsuario
        {
            get { return tipouser; }
            set { tipouser = value; }
        }


        public Menu(string username)
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
