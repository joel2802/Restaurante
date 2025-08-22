using Restaurante.Movimientos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restaurante.Views.Ventanas
{
    public partial class ControlFacturas : Form
    {

        string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
        public ControlFacturas()
        {
            InitializeComponent();
            CargarFacturas();
            CargarFacturass();
            RefrescarHistorialFacturas(); 

        }




        public DataTable GetAllFacturas()
        {
            DataTable facturasTable = new DataTable();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
                SELECT f.*,
                       p.cliente AS NombreCliente,
                       u.nombre AS NombreUsuario
                FROM facturas f
                INNER JOIN pedidos p ON f.idpedido = p.idpedido
                INNER JOIN usuarios u ON p.idusuario = u.idusuario";

                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(facturasTable);
                }
            }

            return facturasTable;
        }

        private DataTable GetFacturasTable()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT f.idfactura, 
                   f.idpedido, 
                   f.total, 
                   f.subtotal, 
                   f.impuesto, 
                   f.estado, 
                   f.fecha,
                   p.cliente AS NombreCliente, 
                   u.nombre AS NombreUsuario
            FROM facturas f
            INNER JOIN pedidos p ON f.idpedido = p.idpedido
            INNER JOIN usuarios u ON p.idusuario = u.idusuario
            ORDER BY f.idfactura ASC"; 
        
        using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }
        public void RefrescarHistorialFacturas()
        {
            try
            {
                dataGridView1.DataSource = GetFacturasTable();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns["idpedido"] != null)
                    dataGridView1.Columns["idpedido"].Visible = false;

                if (dataGridView1.Columns["estado"] != null)
                    dataGridView1.Columns["estado"].Visible = false;

                if (dataGridView1.Columns["NombreUsuario"] != null)
                    dataGridView1.Columns["NombreUsuario"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar facturas: " + ex.Message);
            }
        }



        private void CargarFacturass()
        {
            dataGridView1.DataSource = GetAllFacturas();
        }
        private void CargarFacturas()
        {
            dataGridView1.DataSource = GetFacturasTable();
        }
        private void ControlFacturas_Load(object sender, EventArgs e)
        {

        }

        private DataTable ObtenerProductosDelPedido(int idPedido)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
        SELECT 
            pp.idpedido   AS IdPedido, 
            pp.idproducto    AS IdProducto, 
            pr.codigoproducto AS CodigoProducto,
            pr.nombre     AS NombreProducto,
            pp.cantidad     AS Cantidad, 
            pp.preciounitario AS PrecioUnitario, 
            pp.preciototal   AS PrecioTotal, 
            (pp.preciounitario * pp.cantidad * (i.porcentaje / 100)) AS Impuesto
        FROM pedidoproducto pp
        INNER JOIN productos pr ON pp.idproducto = pr.idproducto
        INNER JOIN impuestos i ON pr.idimpuesto = i.idimpuesto
        WHERE pp.idpedido = @IdPedido";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdPedido", idPedido);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("No hay ningún pedido seleccionado.");
                return;
            }
            MessageBox.Show("Factura generada correctamente.");

            try
            {
                int idPedido = Convert.ToInt32(dataGridView1.CurrentRow.Cells["idpedido"].Value);
                string cliente = dataGridView1.CurrentRow.Cells["NombreCliente"].Value?.ToString() ?? "Sin nombre";
                decimal subtotal = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["SubTotal"].Value);
                decimal impuesto = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Impuesto"].Value);
                decimal total = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Total"].Value);

                RefrescarHistorialFacturas();

                DataTable productos = ObtenerProductosDelPedido(idPedido);
                if (productos.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontraron productos para el pedido #" + idPedido);
                    return;
                }

               
                FacturaView factura = new FacturaView(productos, "Cajero1", "", 0, 0, total, subtotal, impuesto)
                {
                    IdPedido = idPedido,
                    Cliente = cliente,
                    NCF = "N/A"
                };
                factura.ShowDialog();

              

                dataGridView1.DataSource = GetAllFacturas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la factura: " + ex.Message);
            }
        }
    }
}

