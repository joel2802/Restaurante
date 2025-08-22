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

namespace Restaurante.Consultas
{
    public partial class ConsultaProductos : Form
    {
        SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;");
        public ConsultaProductos()
        {
            InitializeComponent();
            CargarProductosEnGrid();
        }

        private void CargarProductosEnGrid()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            if (dataGridView1 == null)
            {
                MessageBox.Show("El DataGridView no está inicializado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string query = @"
                    SELECT 
                        p.IdProducto,
                        p.CodigoProducto,
                        p.Nombre,
                        p.IdCategoria,
                        c.nombre AS Categoria,
                        p.PrecioVenta,
                        p.IdImpuesto,
                        i.nombre AS Impuesto,
                        p.IdProveedor,
                        pr.nombre AS Proveedor,
                        p.PrecioCompra,
                        p.descuento,
                        p.Existencia,
                        p.NoLote,
                        p.StockMinimo,
                        p.StockMaximo,
                        p.FechaElaboracion,
                        p.FechaExpiracion,
                        p.CodigoDeBarra,
                        p.RutaFoto,
                        p.Estado,
                        p.EsProductoFinal
                    FROM Productos p
                    LEFT JOIN categorias c ON p.IdCategoria = c.idcategoria
                    LEFT JOIN impuestos i ON p.IdImpuesto = i.idimpuesto
                    LEFT JOIN proveedor pr ON p.IdProveedor = pr.idproveedor
                    WHERE p.estado = 1";

                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        da.Fill(dt);
                    }
                }

                dataGridView1.DataSource = dt;

                // Ocultar columnas internas
                string[] columnasOcultar = { "IdProducto", "IdCategoria", "IdImpuesto", "IdProveedor", "RutaFoto" };
                foreach (string col in columnasOcultar)
                {
                    if (dataGridView1.Columns.Contains(col))
                        dataGridView1.Columns[col].Visible = false;
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConsultaProductos_Load(object sender, EventArgs e)
        {
            cbxcampo.Items.Clear();

            
            cbxcampo.Items.Add("CodigoProducto");   
            cbxcampo.Items.Add("Nombre");           
            cbxcampo.Items.Add("PrecioVenta");     
            cbxcampo.Items.Add("PrecioCompra");     
            cbxcampo.Items.Add("descuento");        
            cbxcampo.Items.Add("Existencia");       
            cbxcampo.Items.Add("NoLote");          
            cbxcampo.Items.Add("StockMinimo");      
            cbxcampo.Items.Add("StockMaximo");      
            cbxcampo.Items.Add("FechaElaboracion"); 
            cbxcampo.Items.Add("FechaExpiracion");  
            cbxcampo.Items.Add("CodigoDeBarra");    

           
            cbxcampo.Items.Add("Categoria");   
            cbxcampo.Items.Add("Impuesto");    
            cbxcampo.Items.Add("Proveedor");  

           
            cbxcampo.SelectedItem = "Nombre";

            dataGridView1.ClearSelection();

        }

        private void btnmostrar_Click(object sender, EventArgs e)
        {
            txtbuscar.Text = "";
          
            cbxcampo.SelectedIndex = 0;
           
            CargarProductosEnGrid();
            cbxcampo.SelectedIndex = -1;
            dataGridView1.ClearSelection();
        }

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView1 == null)
                return;

            string campoSeleccionado = cbxcampo.SelectedItem?.ToString().ToLower();
            string valorBusqueda = txtbuscar.Text.Trim();

            if (string.IsNullOrEmpty(campoSeleccionado) || string.IsNullOrEmpty(valorBusqueda))
            {
                CargarProductosEnGrid(); 
                return;
            }

            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            try
            {
                string queryBase = @"
                SELECT 
                    p.IdProducto,
                    p.CodigoProducto,
                    p.Nombre,
                    p.IdCategoria,
                    c.nombre AS Categoria,
                    p.PrecioVenta,
                    p.IdImpuesto,
                    i.nombre AS Impuesto,
                    p.IdProveedor,
                    pr.nombre AS Proveedor,
                    p.PrecioCompra,
                    p.descuento,
                    p.Existencia,
                    p.NoLote,
                    p.StockMinimo,
                    p.StockMaximo,
                    p.FechaElaboracion,
                    p.FechaExpiracion,
                    p.CodigoDeBarra,
                    p.RutaFoto,
                    p.Estado,
                    p.EsProductoFinal
                FROM Productos p
                LEFT JOIN categorias c ON p.IdCategoria = c.idcategoria
                LEFT JOIN impuestos i ON p.IdImpuesto = i.idimpuesto
                LEFT JOIN proveedor pr ON p.IdProveedor = pr.idproveedor
                WHERE p.estado = 1 AND ";

                string filtro = "";
                SqlCommand cmd = new SqlCommand();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    cmd.Connection = con;

                    
                    if (campoSeleccionado == "idproducto" || campoSeleccionado == "idcategoria" || campoSeleccionado == "idimpuesto" || campoSeleccionado == "idproveedor")
                    {
                        if (!int.TryParse(valorBusqueda, out int valorInt))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        filtro = $"p.{campoSeleccionado} = @valor";
                        cmd.Parameters.AddWithValue("@valor", valorInt);
                    }
                    else if (campoSeleccionado == "categoria")
                    {
                        filtro = "c.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "impuesto")
                    {
                        filtro = "i.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "proveedor")
                    {
                        filtro = "pr.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else
                    {
                        filtro = $"p.{campoSeleccionado} LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }

                    cmd.CommandText = queryBase + filtro;

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    dataGridView1.DataSource = dt;

                    string[] columnasOcultar = { "IdProducto", "IdCategoria", "IdImpuesto", "IdProveedor", "RutaFoto" };
                    foreach (string col in columnasOcultar)
                    {
                        if (dataGridView1.Columns.Contains(col))
                            dataGridView1.Columns[col].Visible = false;
                    }

                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la búsqueda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtbuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;

            if (dataGridView1 == null)
            {
                MessageBox.Show("El DataGridView no está inicializado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string campoSeleccionado = cbxcampo.SelectedItem?.ToString().ToLower();
            string valorBusqueda = txtbuscar.Text.Trim();

            if (string.IsNullOrEmpty(campoSeleccionado) || string.IsNullOrEmpty(valorBusqueda))
            {
                MessageBox.Show("Selecciona un campo y escribe un valor para buscar.");
                return;
            }

            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string queryBase = @"
                        SELECT 
                            p.nombre_vendedor,
                            p.idproveedor,
                            p.nodocumento,
                            p.idtipodocumento,
                            td.descripcion AS TipoDocumentoNombre,
                            p.nombre,
                            p.telefono,
                            p.idprovincia,
                            prov.nombre AS ProvinciaNombre,
                            p.iddepartamento,
                            dep.nombre AS DepartamentoNombre,
                            p.direccion,
                            p.correo,
                            p.estado
                        FROM proveedor p
                        INNER JOIN tipo_documento td ON p.idtipodocumento = td.idtipo
                        INNER JOIN provincia prov ON p.idprovincia = prov.idprovincia
                        INNER JOIN departamentos dep ON p.iddepartamento = dep.iddepartamento
                        WHERE p.estado = 1 AND ";

                    string filtro = "";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    
                    if (campoSeleccionado == "idproveedor" || campoSeleccionado == "idtipodocumento" ||
                        campoSeleccionado == "idprovincia" || campoSeleccionado == "iddepartamento")
                    {
                        if (!int.TryParse(valorBusqueda, out int valorInt))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        filtro = $"p.{campoSeleccionado} = @valor";
                        cmd.Parameters.AddWithValue("@valor", valorInt);
                    }
                    
                    else if (campoSeleccionado == "tipodocumentonombre")
                    {
                        filtro = "td.descripcion LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "provincianombre")
                    {
                        filtro = "prov.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "departamentonombre")
                    {
                        filtro = "dep.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                  
                    else
                    {
                        filtro = $"p.{campoSeleccionado} LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }

                    cmd.CommandText = queryBase + filtro;

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    dataGridView1.DataSource = dt;

              
                    string[] columnasOcultar = { "idproveedor", "idtipodocumento", "idprovincia", "iddepartamento", "estado" };
                    foreach (string col in columnasOcultar)
                    {
                        if (dataGridView1.Columns.Contains(col))
                            dataGridView1.Columns[col].Visible = false;
                    }

                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la búsqueda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

