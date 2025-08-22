using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Restaurante.Consultas;
using Restaurante.Movimientos;
using Restaurante.Presenters;
using Restaurante.Reportes;
using Restaurante.Repositories;
using Restaurante.Views;
using Restaurante.Views.Mantenimientos;

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
            this.IsMdiContainer = true;
        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoCliente ver = new MantenimientoCliente();
            ver.Show();
        }

        private void categoriasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoCategorias ver = new MantenimientoCategorias();
            ver.Show();
        }

        private void salasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoSalas ver = new MantenimientoSalas();
            ver.Show();
        }

        private void empleadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoUsuarios ver = new MantenimientoUsuarios();
            ver.Show();
        }

        private void medidasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoMedida ver = new MantenimientoMedida();
            ver.Show();
        }

        private void mesasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoMesas ver = new MantenimientoMesas();
            ver.Show();
        }

        private void proveedorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoProveedor ver = new MantenimientoProveedor();
            ver.Show();
        }

        private void clientesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConsultaClientes ver = new ConsultaClientes();
            ver.Show();
        }

        private void categoriasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConsultaCategorias ver = new ConsultaCategorias();
            ver.Show();
        }

        private void salasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConsultaSalas ver = new ConsultaSalas();
            ver.Show();
        }

        private void empleadosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConsultaMedidas ver = new ConsultaMedidas();
            ver.Show();
        }

        private void mesasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConsultaMesas ver = new ConsultaMesas();
            ver.Show();
        }

        private void proveedorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConsultaProveedor ver = new ConsultaProveedor();
            ver.Show();
        }

        private void productosToolStripMenuItem_Click(object sender, EventArgs e)
        {
           MantenimientoProductos ver = new MantenimientoProductos();
            ver.Show();
        }

        private void clientesToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MantenimientoCliente ver = new MantenimientoCliente();
            ver.Show();
        }

        private void categoriasToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MantenimientoCategorias ver = new MantenimientoCategorias();
            ver.Show();
        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoSalas ver = new MantenimientoSalas();
            ver.Show();
        }

        private void empleadosToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MantenimientoUsuarios ver = new MantenimientoUsuarios();
            ver.Show();
        }

        private void medidasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MantenimientoMedida ver = new MantenimientoMedida();
            ver.Show();
        }

        private void mesasToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MantenimientoMesas ver = new MantenimientoMesas();
            ver.Show();
        }

        private void proovedorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MantenimientoProveedor ver = new MantenimientoProveedor();
            ver.Show();
        }

        private void productosToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MantenimientoProductos ver = new MantenimientoProductos();
            ver.Show();
        }

        private void pOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MovimientoPOS ver = new MovimientoPOS();
            ver.Show();
        }

        private void ventasToolStripMenuItem_Click(object sender, EventArgs e)
        {
           Ventas ver = new Ventas();
            ver.Show();
        }

        private void empleadosToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ConsultaEmpleados ver = new ConsultaEmpleados();
            ver.Show();
        }

        private void productosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConsultaProductos ver = new  ConsultaProductos();
            ver.Show();
        }
    }
}
