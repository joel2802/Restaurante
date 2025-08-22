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
    public partial class ConsultaEmpleados : Form
    {
        public ConsultaEmpleados()
        {
            InitializeComponent();
            CargarDatos();
        }


        private void CargarDatos()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand())
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    connection.Open();
                    command.Connection = connection;

                    // Consulta original usando u.* y joins
                    command.CommandText = @"
                SELECT u.*, s.nombre AS NombreSucursal, tu.descripcion AS TipoUsuario, sx.descripcion AS Sexo
                FROM usuarios u
                INNER JOIN sucursal s ON u.idsucursal = s.idsucursal
                INNER JOIN tipo_usuario tu ON u.idtipo = tu.idtipo
                INNER JOIN sexo sx ON u.idsexo = sx.idsexo";

                    adapter.SelectCommand = command;
                    adapter.Fill(dt);
                }

                // BindingSource local
                BindingSource usuarioBindingSource = new BindingSource();
                usuarioBindingSource.DataSource = dt;
                dataGridView1.DataSource = usuarioBindingSource;

                // Ocultar columnas de ID y contraseña
                string[] columnasOcultas = { "idusuario", "idsexo", "idtipo", "idsucursal", "contrasena" };
                foreach (var col in columnasOcultas)
                {
                    if (dataGridView1.Columns.Contains(col))
                        dataGridView1.Columns[col].Visible = false;
                }

                // Convertir columna estatus a checkbox
                if (dataGridView1.Columns.Contains("estatus"))
                {
                    int colIndex = dataGridView1.Columns["estatus"].Index;
                    DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn
                    {
                        Name = "Estatus",
                        HeaderText = "Estado",
                        DataPropertyName = "estatus",
                        TrueValue = true,
                        FalseValue = false
                    };
                    dataGridView1.Columns.RemoveAt(colIndex);
                    dataGridView1.Columns.Insert(colIndex, chkCol);
                }

                // Activar filtros de columna
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }

                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.ScrollBars = ScrollBars.Both;

                // Enlazar DataBindingComplete para aplicar orden de columnas y ancho fijo
                

                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConsultaEmpleados_Load(object sender, EventArgs e)
        {
            cbxcampo.Items.Clear();

            cbxcampo.Items.Add("no_documento");
            cbxcampo.Items.Add("usuario");
            cbxcampo.Items.Add("telefono");
            cbxcampo.Items.Add("nombre");
            cbxcampo.Items.Add("correo"); 
            cbxcampo.Items.Add("correo");
            cbxcampo.Items.Add("direccion");
            cbxcampo.Items.Add("comision");
           
                   
            cbxcampo.Items.Add("fecha_creacion");      
            cbxcampo.Items.Add("apellidos");     
            cbxcampo.Items.Add("direccion");      
                   
            cbxcampo.Items.Add("fecha_creacion");  // u.fecha_creacion

           
            cbxcampo.Items.Add("NombreSucursal");  // s.nombre AS NombreSucursal
            cbxcampo.Items.Add("TipoUsuario");     // tu.descripcion AS TipoUsuario
                      // sx.descripcion AS Sexo

            // ✅ Campo por defecto
            cbxcampo.SelectedItem = "usuario";
            cbxcampo.Items.Add("Sexo");
        }

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            string campoSeleccionado = cbxcampo.SelectedItem?.ToString();
            string valorBusqueda = txtbuscar.Text.Trim();

            if (string.IsNullOrEmpty(campoSeleccionado) || string.IsNullOrEmpty(valorBusqueda))
            {
                // Si no hay filtro, recarga todos los datos
                CargarDatos();
                return;
            }

            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand())
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    connection.Open();
                    command.Connection = connection;

                    string queryBase = @"
                        SELECT 
                            u.*,
                            s.nombre AS NombreSucursal, 
                            tu.descripcion AS TipoUsuario, 
                            sx.descripcion AS Sexo
                        FROM usuarios u
                        INNER JOIN sucursal s ON u.idsucursal = s.idsucursal
                        INNER JOIN tipo_usuario tu ON u.idtipo = tu.idtipo
                        INNER JOIN sexo sx ON u.idsexo = sx.idsexo
                        WHERE ";

                    string filtro = "";

                    // Campos numéricos → búsqueda exacta
                    if (campoSeleccionado == "idusuario" || campoSeleccionado == "idsucursal" ||
                        campoSeleccionado == "idtipo" || campoSeleccionado == "idsexo")
                    {
                        filtro = $"u.{campoSeleccionado} = @valor";
                        if (!int.TryParse(valorBusqueda, out int valorInt))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        command.Parameters.AddWithValue("@valor", valorInt);
                    }
                    // Campos de tablas relacionadas
                    else if (campoSeleccionado == "NombreSucursal")
                    {
                        filtro = "s.nombre LIKE @valor";
                        command.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "TipoUsuario")
                    {
                        filtro = "tu.descripcion LIKE @valor";
                        command.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "Sexo")
                    {
                        filtro = "sx.descripcion LIKE @valor";
                        command.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    // Campos de texto de usuarios
                    else
                    {
                        filtro = $"u.{campoSeleccionado} LIKE @valor";
                        command.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }

                    command.CommandText = queryBase + filtro;
                    adapter.Fill(dt);
                }

                // BindingSource
                BindingSource usuarioBindingSource = new BindingSource();
                usuarioBindingSource.DataSource = dt;
                dataGridView1.DataSource = usuarioBindingSource;

                // Ocultar columnas sensibles
                string[] columnasOcultas = { "idusuario", "idsexo", "idtipo", "idsucursal", "contrasena" };
                foreach (var col in columnasOcultas)
                {
                    if (dataGridView1.Columns.Contains(col))
                        dataGridView1.Columns[col].Visible = false;
                }

                // Convertir columna estatus a checkbox
                if (dataGridView1.Columns.Contains("estatus"))
                {
                    int colIndex = dataGridView1.Columns["estatus"].Index;
                    DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn
                    {
                        Name = "Estatus",
                        HeaderText = "Estado",
                        DataPropertyName = "estatus",
                        TrueValue = true,
                        FalseValue = false
                    };
                    dataGridView1.Columns.RemoveAt(colIndex);
                    dataGridView1.Columns.Insert(colIndex, chkCol);
                }

                // Activar filtros de columna
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }

                // Ajustes visuales
                dataGridView1.ClearSelection();
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.ScrollBars = ScrollBars.Both;
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la búsqueda:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    u.*, 
                    s.nombre AS NombreSucursal, 
                    tu.descripcion AS TipoUsuario, 
                    sx.descripcion AS Sexo
                FROM usuarios u
                INNER JOIN sucursal s ON u.idsucursal = s.idsucursal
                INNER JOIN tipo_usuario tu ON u.idtipo = tu.idtipo
                INNER JOIN sexo sx ON u.idsexo = sx.idsexo
                WHERE ";

                        string filtro = "";
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;

                        // 🔹 Campos numéricos exactos
                        if (campoSeleccionado == "idusuario" || campoSeleccionado == "idsucursal" ||
                            campoSeleccionado == "idtipo" || campoSeleccionado == "idsexo")
                        {
                            filtro = $"u.{campoSeleccionado} = @valor";
                            if (!int.TryParse(valorBusqueda, out int valorInt))
                            {
                                dataGridView1.DataSource = null;
                                return;
                            }
                            cmd.Parameters.AddWithValue("@valor", valorInt);
                        }
                        // 🔹 Campos de tablas relacionadas (alias)
                        else if (campoSeleccionado == "nombresucursal")
                        {
                            filtro = "s.nombre LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else if (campoSeleccionado == "tipousuario")
                        {
                            filtro = "tu.descripcion LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else if (campoSeleccionado == "sexo")
                        {
                            filtro = "sx.descripcion LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        // 🔹 Cualquier otro campo texto de usuarios
                        else
                        {
                            filtro = $"u.{campoSeleccionado} LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }

                        cmd.CommandText = queryBase + filtro;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;

                        // Ocultar columnas internas
                        string[] columnasOcultar = { "idusuario", "idsexo", "idtipo", "idsucursal", "contrasena" };
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
    }
 }

