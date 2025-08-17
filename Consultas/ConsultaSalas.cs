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
    public partial class ConsultaSalas : Form
    {

        string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
        public int? IdSalaSeleccionada { get; private set; }
        public string NombreSalaSeleccionada { get; private set; }
        public string NombreSucursalSeleccionada { get; private set; }
        public ConsultaSalas()
        {
            InitializeComponent();
            CargarDatos();
        }


        private void CargarDatos()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
            int idsucursal = 1; // Cambiar según necesidad

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = @"
                SELECT 
                    s.*, 
                    sc.nombre AS nombre_sucursal
                FROM salas s
                INNER JOIN sucursal sc ON s.idsucursal = sc.idsucursal 
                WHERE s.estado = 1 AND s.idsucursal = @IdSucursal";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@IdSucursal", SqlDbType.Int).Value = idsucursal;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        BindingSource bs = new BindingSource();
                        bs.DataSource = dt;
                        dataGridView1.DataSource = bs;

                        // 🔹 Ocultar columnas internas
                        if (dataGridView1.Columns.Contains("estado"))
                            dataGridView1.Columns["estado"].Visible = false;

                        if (dataGridView1.Columns.Contains("idsala"))
                            dataGridView1.Columns["idsala"].Visible = false;

                        if (dataGridView1.Columns.Contains("idsucursal"))
                            dataGridView1.Columns["idsucursal"].Visible = false;

                        // 🔹 Ajustes visuales
                        dataGridView1.ScrollBars = ScrollBars.Both;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // 🔹 Activar filtros en encabezados
                        foreach (DataGridViewColumn col in dataGridView1.Columns)
                        {
                            col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                        }

                        // 🔹 Quitar selección automática
                        dataGridView1.ClearSelection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar datos: " + ex.Message);
                }
            }
        }
        private void ConsultaSalas_Load(object sender, EventArgs e)
        {
            cbxcampo.Items.Clear();
            cbxcampo.Items.Add("nombre");            // Nombre de la sala
            cbxcampo.Items.Add("nombre_sucursal");   // Nombre de la sucursal

            // 🔹 Sin selección inicial en el combo
            cbxcampo.SelectedItem = "nombre";

            // 🔹 Cargar datos iniciales desde la BD
            CargarDatos();

            // 🔹 Evitar que el DataGridView seleccione una fila al iniciar
            dataGridView1.ClearSelection();
        }

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            string campoSeleccionado = cbxcampo.SelectedItem?.ToString();
            string valorBusqueda = txtbuscar.Text.Trim();

            // 🔹 Si no hay campo o búsqueda vacía, cargamos todos
            if (string.IsNullOrEmpty(campoSeleccionado) || string.IsNullOrEmpty(valorBusqueda))
            {
                CargarDatos();
                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;
                return;
            }

            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
            int idsucursal = 1; // Cambia según sea necesario

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string queryBase = @"
                SELECT 
                    s.idsala,
                    s.nombre,
                    s.idsucursal,
                    sc.nombre AS nombre_sucursal
                FROM salas s
                INNER JOIN sucursal sc ON s.idsucursal = sc.idsucursal
                WHERE s.estado = 1 AND s.idsucursal = @IdSucursal AND ";

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Parameters.Add("@IdSucursal", SqlDbType.Int).Value = idsucursal;

                    string filtro;
                    if (campoSeleccionado == "nombre")
                    {
                        filtro = "s.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "nombre_sucursal")
                    {
                        filtro = "sc.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else
                    {
                        filtro = "1 = 0"; // Evita resultados inesperados si no hay match
                    }

                    cmd.CommandText = queryBase + filtro;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable tabla = new DataTable();
                    adapter.Fill(tabla);

                    BindingSource bs = new BindingSource();
                    bs.DataSource = tabla;
                    dataGridView1.DataSource = bs;

                    // 🔹 Ocultar columnas internas
                    if (dataGridView1.Columns.Contains("idsala"))
                        dataGridView1.Columns["idsala"].Visible = false;
                    if (dataGridView1.Columns.Contains("idsucursal"))
                        dataGridView1.Columns["idsucursal"].Visible = false;

                    // 🔹 Activar filtros en encabezados
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);

                    // 🔹 Quitar selección automática
                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error en la búsqueda: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            IdSalaSeleccionada = row.Cells["idsala"].Value != DBNull.Value
                ? Convert.ToInt32(row.Cells["idsala"].Value)
                : (int?)null;

            NombreSalaSeleccionada = row.Cells["nombre"].Value?.ToString() ?? "";
            NombreSucursalSeleccionada = row.Cells["nombre_sucursal"].Value?.ToString() ?? "";

            this.DialogResult = DialogResult.OK;
            this.Close();
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

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        string queryBase = @"
                    SELECT 
                        s.idsala,
                        s.nombre,
                        s.idsucursal,
                        sc.nombre AS nombre_sucursal
                    FROM salas s
                    INNER JOIN sucursal sc ON s.idsucursal = sc.idsucursal
                    WHERE s.estado = 1 AND ";

                        string filtro = "";
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;

                        if (campoSeleccionado == "idsala")
                        {
                            filtro = "s.idsala = @valor";
                            if (!int.TryParse(valorBusqueda, out int idSala))
                            {
                                dataGridView1.DataSource = null;
                                return;
                            }
                            cmd.Parameters.AddWithValue("@valor", idSala);
                        }
                        else if (campoSeleccionado == "nombre")
                        {
                            filtro = "s.nombre LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else if (campoSeleccionado == "nombre_sucursal")
                        {
                            filtro = "sc.nombre LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else
                        {
                            filtro = "1 = 0"; // Evita resultados inesperados
                        }

                        cmd.CommandText = queryBase + filtro;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        BindingSource bs = new BindingSource();
                        bs.DataSource = dt;
                        dataGridView1.DataSource = bs;

                        // Ocultar columnas internas
                        if (dataGridView1.Columns.Contains("idsala"))
                            dataGridView1.Columns["idsala"].Visible = false;
                        if (dataGridView1.Columns.Contains("idsucursal"))
                            dataGridView1.Columns["idsucursal"].Visible = false;

                        // Activar filtros en encabezados
                        foreach (DataGridViewColumn col in dataGridView1.Columns)
                            col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);

                        // 🔹 Quitar cualquier selección automática
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error en la búsqueda: " + ex.Message);
                    }
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
        }

        private void btnmostrar_Click(object sender, EventArgs e)
        {
            // Limpia la búsqueda
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
