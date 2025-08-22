using Restaurante.Movimientos;
using Restaurante.Views.Mantenimientos;
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

namespace Restaurante.Views.Ventanas
{
    public partial class PedidosPendientes : Form
    {
        string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

        public PedidosPendientes()
        {
            InitializeComponent();
            CargarPedidosPendientes();

            CargarPedidosPendientes(); 

           

        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
           
                int idPedido = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["idpedido"].Value);
                
            }
        }
        private void CargarPedidosPendientes()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                SELECT 
                    p.idpedido,
                    ISNULL(p.cliente, '') AS Cliente,
                    m.nombre AS NombreMesa,
                    p.total,
                    p.impuesto,
                    p.subtotal,
                    p.fecha,
                    CASE WHEN p.estado = 0 THEN 'Pendiente' ELSE 'Entregado' END AS Estado
                FROM pedidos p
                INNER JOIN mesas m ON p.idmesa = m.idmesa
                WHERE p.estado = 0
                ORDER BY p.fecha DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        dataGridView1.DataSource = null;
                        return; 
                    }

                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                  
                    if (dataGridView1.Columns.Contains("idpedido"))
                        dataGridView1.Columns["idpedido"].Visible = false;

                  
                    if (dataGridView1.Columns.Contains("Cliente"))
                        dataGridView1.Columns["Cliente"].HeaderText = "Cliente";
                    if (dataGridView1.Columns.Contains("NombreMesa"))
                        dataGridView1.Columns["NombreMesa"].HeaderText = "Mesa";
                    if (dataGridView1.Columns.Contains("Total"))
                        dataGridView1.Columns["Total"].DefaultCellStyle.Format = "N2";
                    if (dataGridView1.Columns.Contains("Impuesto"))
                        dataGridView1.Columns["Impuesto"].DefaultCellStyle.Format = "N2";
                    if (dataGridView1.Columns.Contains("SubTotal"))
                        dataGridView1.Columns["SubTotal"].DefaultCellStyle.Format = "N2";
                    if (dataGridView1.Columns.Contains("Fecha"))
                        dataGridView1.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    if (dataGridView1.Columns.Contains("Estado"))
                        dataGridView1.Columns["Estado"].HeaderText = "Estado";

                    dataGridView1.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pedidos: " + ex.Message);
            }
        }

      
       
        public event Action PedidoEntregado;

        public void CargarDatosMesas()
        {
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("No hay ningún pedido seleccionado.");
                return;
            }

            try
            {
               
                int idPedido = int.TryParse(dataGridView1.CurrentRow.Cells["idpedido"].Value?.ToString(), out var tempId) ? tempId : 0;
                if (idPedido <= 0)
                {
                    MessageBox.Show("ID de pedido inválido.");
                    return;
                }

                string cliente = dataGridView1.CurrentRow.Cells["Cliente"].Value?.ToString() ?? "Sin nombre";

                decimal subtotal = decimal.TryParse(dataGridView1.CurrentRow.Cells["SubTotal"].Value?.ToString(), out var tempSub) ? tempSub : 0;
                decimal impuesto = decimal.TryParse(dataGridView1.CurrentRow.Cells["Impuesto"].Value?.ToString(), out var tempImp) ? tempImp : 0;
                decimal total = decimal.TryParse(dataGridView1.CurrentRow.Cells["Total"].Value?.ToString(), out var tempTot) ? tempTot : 0;

                
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                UPDATE pedidos SET Estado = 1 WHERE IdPedido = @IdPedido;
                UPDATE mesas 
                SET Disponible = 1
                WHERE IdMesa = (SELECT IdMesa FROM pedidos WHERE IdPedido = @IdPedido);";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@IdPedido", idPedido);
                        cmd.ExecuteNonQuery();
                    }
                }

              
                CargarPedidosPendientes();

                foreach (Form frm in Application.OpenForms)
                {
                    if (frm is MovimientoPOS frmMov)
                    {
                        frmMov.CargarMesas();
                        break;
                    }
                }

                DataTable productos = ObtenerProductosDelPedido(idPedido);
                if (productos.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontraron productos para el pedido #" + idPedido);
                    return;
                }
                DataTable dtProductos = ObtenerProductosDelPedido(idPedido);

            
                FacturaView factura = new FacturaView(productos, "Cajero1", "", 0, 0, total, subtotal, impuesto)
                {
                    IdPedido = idPedido,
                    Cliente = cliente,
                    NCF = "N/A"
                };
                factura.ShowDialog();

                MessageBox.Show("Pedido entregado, mesa liberada y factura generada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar el pedido: " + ex.Message);
            }
        }

        private DataTable ObtenerProductosDelPedido(int idPedido)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
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
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontraron productos para el pedido #" + idPedido);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener productos del pedido: " + ex.Message);
            }

            return dt;
        }


        private void PedidosPendientes_Load(object sender, EventArgs e)
        {
           
        }
    }

 }

