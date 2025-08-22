using Microsoft.Reporting.WinForms;
using Restaurante.Movimientos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restaurante.Reportes
{
    public partial class Ventas : Form
    {
        private string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
        public Ventas()
        {
            InitializeComponent();
            LlenarComboBoxMes(cbxmes, DateTime.Now.Year);
            LlenarComboBoxAno(cbxano);

            if (cbxtipo.Items.Count > 0)
                cbxtipo.SelectedIndex = 0;

            cbxano.SelectedIndexChanged += cbxano_SelectedIndexChanged;
            cbxano.SelectedIndex = cbxano.Items.Count - 1;
        }
        public event EventHandler GenerateReport;
        public (decimal Total, decimal Subtotal, decimal Impuesto) GetVentasTotals(int month, int year)
        {
            decimal total = 0m;
            decimal subtotal = 0m;
            decimal impuesto = 0m;

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
            SELECT 
                SUM(total) AS Total, 
                SUM(subtotal) AS Subtotal, 
                SUM(impuesto) AS Impuesto 
            FROM facturas
            WHERE 
                MONTH(fecha) = @Month AND YEAR(fecha) = @Year";

                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        total = reader["Total"] != DBNull.Value ? (decimal)reader["Total"] : 0m;
                        subtotal = reader["Subtotal"] != DBNull.Value ? (decimal)reader["Subtotal"] : 0m;
                        impuesto = reader["Impuesto"] != DBNull.Value ? (decimal)reader["Impuesto"] : 0m;
                    }
                }
            }

            return (total, subtotal, impuesto);
        }


        private DataTable GetVentasPorCategoria(int month, int year)
        {
            DataTable dt = new DataTable();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("GetVentasPorCategoria", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }

        private DataTable GetVentasPorProducto(int month, int year)
        {
            DataTable dt = new DataTable();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("GetVentasPorProducto", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    dt.Load(reader); 
                }
            }

            return dt;
        }

        private DataTable GetVentasPorUsuario(int month, int year)
        {
            DataTable dt = new DataTable();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("GetVentasPorUsuario", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    dt.Load(reader); 
                }
            }

            return dt;
        }

        public byte? Mes
        {
            get
            {
                string selectedMonth = cbxmes.SelectedItem?.ToString();
                return selectedMonth != null ? (byte)(DateTime.ParseExact(selectedMonth, "MMMM", null).Month) : (byte?)null;
            }
            set
            {
                cbxmes.SelectedItem = new DateTime(1, (int)value, 1).ToString("MMMM");
            }
        }

        public int? Año
        {
            get
            {
                if (cbxano.SelectedItem != null)
                {
                    return Convert.ToInt32(cbxano.SelectedItem.ToString());
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue && value > 0)
                {
                    cbxano.SelectedItem = value.ToString();
                }
                else
                {
                    cbxano.SelectedIndex = -1;
                }
            }
        }

        public decimal Total
        {
            get => decimal.TryParse(lblTotalGeneral.Text, out var total) ? total : 0m;
            set => lblTotalGeneral.Text = value.ToString("N2");
        }

        public decimal Subtotal
        {
            get => decimal.TryParse(lblSubtotal.Text, out var subtotal) ? subtotal : 0m;
            set => lblSubtotal.Text = value.ToString("N2");
        }

        public decimal Impuesto
        {
            get => decimal.TryParse(lblItbis.Text, out var impuesto) ? impuesto : 0m;
            set => lblItbis.Text = value.ToString("N2");
        }
        public string Tipo
        {
            get { return cbxtipo.SelectedItem?.ToString(); }
            set { cbxtipo.SelectedItem = value.ToString(); }
        }

        private void LlenarComboBoxAno(ComboBox comboBox)
        {
            comboBox.Items.Clear();

            int anioInicial = 2023;
            int anioActual = DateTime.Now.Year;

            for (int anio = anioInicial; anio <= anioActual; anio++)
            {
                comboBox.Items.Add(anio.ToString());
            }
        }
        private void LlenarComboBoxMes(ComboBox comboBox, int anioSeleccionado)
        {
            comboBox.Items.Clear();

            int mesInicial = 1;
            int mesFinal = 12;

            if (anioSeleccionado == DateTime.Now.Year)
            {
                
                mesFinal = DateTime.Now.Month;
            }

            for (int mes = mesInicial; mes <= mesFinal; mes++)
            {
                string nombreMes = new DateTime(anioSeleccionado, mes, 1).ToString("MMMM", CultureInfo.CurrentCulture);
                comboBox.Items.Add(nombreMes);
            }

           
            comboBox.SelectedIndex = 0;
        }

        private void Ventas_Load(object sender, EventArgs e)
        {
            cbxtipo.Items.Clear(); 
            cbxtipo.Items.Add("Usuario");
            cbxtipo.Items.Add("Producto");
            cbxtipo.Items.Add("Categoria");

           
            cbxtipo.SelectedIndex = 0;

           

           

        }

                   

        private void cbxano_SelectedIndexChanged(object sender, EventArgs e)
        {
            int anioSeleccionado = int.Parse(cbxano.SelectedItem.ToString());
            LlenarComboBoxMes(cbxmes, anioSeleccionado);
        }

        private void cbxtipo_SelectedIndexChanged(object sender, EventArgs e)
        {
           

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btngenerar_Click(object sender, EventArgs e)
        {


            if (cbxmes.SelectedIndex == -1 || cbxano.SelectedIndex == -1 || cbxtipo.SelectedIndex == -1)
            {
                MessageBox.Show("Debes seleccionar Mes, Año y Tipo de reporte.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int mes = cbxmes.SelectedIndex + 1;
            int anio = int.Parse(cbxano.SelectedItem.ToString());
            string tipoReporte = cbxtipo.SelectedItem.ToString();

            DataTable dt = new DataTable();

            // Ejecutar consulta según el tipo de reporte
            switch (tipoReporte)
            {
                case "Categoria":
                    dt = GetVentasPorCategoria(mes, anio);
                    break;

                case "Producto":
                    dt = GetVentasPorProducto(mes, anio);
                    break;

                case "Usuario":
                    dt = GetVentasPorUsuario(mes, anio);
                    break;

                default:
                    MessageBox.Show("Tipo de reporte no válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            // Mostrar resultados
            dataGridView1.DataSource = dt;

            // Formato de columnas numéricas
            if (dt.Columns.Contains("Subtotal"))
                dataGridView1.Columns["Subtotal"].DefaultCellStyle.Format = "N2";
            if (dt.Columns.Contains("Impuesto"))
                dataGridView1.Columns["Impuesto"].DefaultCellStyle.Format = "N2";
            if (dt.Columns.Contains("Total"))
                dataGridView1.Columns["Total"].DefaultCellStyle.Format = "N2";

            // Reiniciar los acumuladores SIEMPRE
            decimal subtotal = 0m;
            decimal impuesto = 0m;
            decimal total = 0m;

            // Sumar cada fila de la consulta
            foreach (DataRow row in dt.Rows)
            {
                subtotal += row.Field<decimal?>("Subtotal") ?? 0m;
                impuesto += row.Field<decimal?>("Impuesto") ?? 0m;
                total += row.Field<decimal?>("Total") ?? 0m;
            }

            // Mostrar resultados en labels
            lblSubtotal.Text = "Subtotal: RD$ " + subtotal.ToString("N2");
            lblItbis.Text = "ITBIS: RD$ " + impuesto.ToString("N2");
            lblTotalGeneral.Text = "Total General: RD$ " + total.ToString("N2");
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("No hay ningún pedido seleccionado.");
                return;
            }

            try
            {
                
                string cliente = dataGridView1.CurrentRow.Cells["NombreCliente"].Value?.ToString() ?? "Sin nombre";
                decimal subtotal = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["SubTotal"].Value);
                decimal impuesto = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Impuesto"].Value);
                decimal total = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Total"].Value);
                string usuario = dataGridView1.CurrentRow.Cells["Usuario"].Value?.ToString() ?? "Desconocido";

              
                byte mes = (byte)(cbxmes.SelectedIndex + 1);
                int año = Convert.ToInt32(cbxano.SelectedItem);
                string tipoReporte = cbxtipo.SelectedItem?.ToString() ?? "General";

                
                DataTable datosFactura = new DataTable();
                datosFactura.Columns.Add("NombreCliente", typeof(string));
                datosFactura.Columns.Add("SubTotal", typeof(decimal));
                datosFactura.Columns.Add("Impuesto", typeof(decimal));
                datosFactura.Columns.Add("Total", typeof(decimal));
                datosFactura.Columns.Add("Usuario", typeof(string));

                DataRow fila = datosFactura.NewRow();
                fila["NombreCliente"] = cliente;
                fila["SubTotal"] = subtotal;
                fila["Impuesto"] = impuesto;
                fila["Total"] = total;
                fila["Usuario"] = usuario;
                datosFactura.Rows.Add(fila);

                ReporteFinal reporte = new ReporteFinal(usuario, tipoReporte, mes, año, total, subtotal, impuesto, datosFactura);
                reporte.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte: " + ex.Message);
            }
        }
    }
}

