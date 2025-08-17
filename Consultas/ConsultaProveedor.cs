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
    public partial class ConsultaProveedor : Form
    {
        string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

        public int? ProveedorIdSeleccionado { get; private set; }
        public string NombreProveedorSeleccionado { get; private set; }

        public ConsultaProveedor()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
            SELECT 
                p.nombre_vendedor, -- lo ponemos de primero
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
            FROM 
                proveedor p
            INNER JOIN 
                tipo_documento td ON p.idtipodocumento = td.idtipo
            INNER JOIN 
                provincia prov ON p.idprovincia = prov.idprovincia
            INNER JOIN 
                departamentos dep ON p.iddepartamento = dep.iddepartamento
            WHERE p.estado = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable tabla = new DataTable();
                        adapter.Fill(tabla);

                        BindingSource bs = new BindingSource();
                        bs.DataSource = tabla;
                        dataGridView1.DataSource = bs;

                        // 🔹 Evitar edición en consulta
                        dataGridView1.ReadOnly = true;
                        dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        dataGridView1.MultiSelect = false;

                        // 🔹 Ocultar columnas internas
                        string[] columnasOcultas =
                        {
                    "idproveedor",
                    "idtipodocumento",
                    "idprovincia",
                    "iddepartamento",
                    "estado"
                };

                        foreach (string col in columnasOcultas)
                        {
                            if (dataGridView1.Columns.Contains(col))
                                dataGridView1.Columns[col].Visible = false;
                        }

                        // 🔹 Ajustes visuales
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = null;
                        dataGridView1.ScrollBars = ScrollBars.Both;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // 🔹 Encabezados con filtro (si usas DataGridViewAutoFilter)
                        foreach (DataGridViewColumn col in dataGridView1.Columns)
                        {
                            col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los proveedores: " + ex.Message);
                }
            }
        }
        private void ConsultaProveedor_Load(object sender, EventArgs e)
        {
            cbxcampo.Items.Clear();

            cbxcampo.Items.Add("nombre_vendedor");     // p.nombre_vendedor
            cbxcampo.Items.Add("nodocumento");          // p.nodocumento
            cbxcampo.Items.Add("tipodocumentonombre");  // td.descripcion AS TipoDocumentoNombre
            cbxcampo.Items.Add("nombre");                // p.nombre
            cbxcampo.Items.Add("telefono");              // p.telefono
            cbxcampo.Items.Add("provincianombre");      // prov.nombre AS ProvinciaNombre
            cbxcampo.Items.Add("departamentonombre");   // dep.nombre AS DepartamentoNombre
            cbxcampo.Items.Add("direccion");             // p.direccion
            cbxcampo.Items.Add("correo");        // p.observaciones

            cbxcampo.SelectedItem = "nombre_vendedor"; // Campo por defecto

            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["idproveedor"].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                ProveedorIdSeleccionado = Convert.ToInt32(row.Cells["idproveedor"].Value);
                NombreProveedorSeleccionado = row.Cells["nombre_vendedor"].Value.ToString();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnmostrar_Click(object sender, EventArgs e)
        {
            txtbuscar.Text = "";
            // Opcional: resetear selección en combo
            cbxcampo.SelectedIndex = 0;
            // Recarga los datos
            CargarDatos();
            cbxcampo.SelectedIndex = -1;
            dataGridView1.ClearSelection();
        }

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            string campoSeleccionado = cbxcampo.SelectedItem?.ToString().ToLower();
            string valorBusqueda = txtbuscar.Text.Trim();

            if (string.IsNullOrEmpty(campoSeleccionado) || string.IsNullOrEmpty(valorBusqueda))
            {
                // Si no hay filtro, cargamos todos
                CargarDatos();
                return;
            }

            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

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
                    cmd.Connection = conn;

                    // Campos numéricos que deben coincidir exactamente
                    if (campoSeleccionado == "idproveedor" || campoSeleccionado == "idtipodocumento" ||
                        campoSeleccionado == "idprovincia" || campoSeleccionado == "iddepartamento")
                    {
                        filtro = $"p.{campoSeleccionado} = @valor";
                        if (!int.TryParse(valorBusqueda, out int valorInt))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
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
                        // Búsqueda en cualquier campo de texto del proveedor
                        filtro = $"p.{campoSeleccionado} LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }

                    cmd.CommandText = queryBase + filtro;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable tabla = new DataTable();
                    adapter.Fill(tabla);

                    BindingSource bs = new BindingSource();
                    bs.DataSource = tabla;
                    dataGridView1.DataSource = bs;

                    // Ocultar columnas internas
                    string[] columnasOcultar = { "idproveedor", "idtipodocumento", "idprovincia", "iddepartamento", "estado" };
                    foreach (var col in columnasOcultar)
                    {
                        if (dataGridView1.Columns.Contains(col))
                            dataGridView1.Columns[col].Visible = false;
                    }

                    // Limpiar selección
                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;

                    // Encabezados con filtro
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error en la búsqueda: " + ex.Message);
                }
            }
        }

        private void txtbuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                string campoSeleccionado = cbxcampo.SelectedItem?.ToString().ToLower();
                string valorBusqueda = txtbuscar.Text.Trim();

                if (string.IsNullOrEmpty(campoSeleccionado) || string.IsNullOrEmpty(valorBusqueda))
                {
                    MessageBox.Show("Selecciona un campo y escribe un valor para buscar.");
                    return;
                }

                string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        string queryBase = @"
                SELECT 
                    p.nombre_vendedor, -- primero
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

                        // Campos numéricos exactos
                        if (campoSeleccionado == "idproveedor" || campoSeleccionado == "idtipodocumento" ||
                            campoSeleccionado == "idprovincia" || campoSeleccionado == "iddepartamento")
                        {
                            filtro = $"p.{campoSeleccionado} = @valor";
                            if (!int.TryParse(valorBusqueda, out int valorInt))
                            {
                                dataGridView1.DataSource = null;
                                return;
                            }
                            cmd.Parameters.AddWithValue("@valor", valorInt);
                        }
                        // Campos de tablas relacionadas
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
                        // Cualquier otro campo texto
                        else
                        {
                            filtro = $"p.{campoSeleccionado} LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }

                        cmd.CommandText = queryBase + filtro;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;

                        // Ocultar columnas internas
                        string[] columnasOcultar = { "idproveedor", "idtipodocumento", "idprovincia", "iddepartamento", "estado" };
                        foreach (string col in columnasOcultar)
                        {
                            if (dataGridView1.Columns.Contains(col))
                                dataGridView1.Columns[col].Visible = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error en la búsqueda: " + ex.Message);
                    }
                }
            }
        }
    }
}
