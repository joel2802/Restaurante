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
    public partial class ConsultaMesas : Form
    {

        string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

        public int? MesaIdSeleccionada { get; private set; }
        public string NombreMesaSeleccionada { get; private set; }
        public ConsultaMesas()
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
                    m.idmesa,
                    m.nombre,
                    m.idsala,
                    s.nombre AS nombre_sala, 
                    m.cantpersonas,
                    m.disponible,
                    m.estado
                FROM mesas m
                INNER JOIN salas s ON m.idsala = s.idsala
                WHERE m.estado = 1";

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
                        if (dataGridView1.Columns.Contains("estado"))
                            dataGridView1.Columns["estado"].Visible = false;

                        if (dataGridView1.Columns.Contains("idmesa"))
                            dataGridView1.Columns["idmesa"].Visible = false;

                        if (dataGridView1.Columns.Contains("idsala"))
                            dataGridView1.Columns["idsala"].Visible = false;

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
                    MessageBox.Show("Error al cargar las mesas: " + ex.Message);
                }
            }
        }
        private void ConsultaMesas_Load(object sender, EventArgs e)
        {
            cbxcampo.Items.Clear();

            cbxcampo.Items.Add("nombre");        // Nombre de la mesa
            cbxcampo.Items.Add("nombre_sala");   // Nombre de la sala
            cbxcampo.Items.Add("cantpersonas");  // Cantidad de personas

            cbxcampo.SelectedItem = "nombre";    // Seleccionamos "nombre" por defecto

            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            MesaIdSeleccionada = row.Cells["idmesa"].Value != DBNull.Value
                ? Convert.ToInt32(row.Cells["idmesa"].Value)
                : (int?)null;

            NombreMesaSeleccionada = row.Cells["nombre"].Value?.ToString() ?? "";

            this.DialogResult = DialogResult.OK;
            this.Close();
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
                    m.idmesa, 
                    m.nombre, 
                    m.idsala, 
                    s.nombre AS nombre_sala, 
                    m.cantpersonas, 
                    m.disponible, 
                    m.estado
                FROM mesas m
                INNER JOIN salas s ON m.idsala = s.idsala
                WHERE m.estado = 1 AND ";

                    string filtro = "";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    // Para filtrar números exactos (ejemplo cantpersonas)
                    if (campoSeleccionado == "idmesa" || campoSeleccionado == "cantpersonas")
                    {
                        filtro = $"m.{campoSeleccionado} = @valor";
                        if (!int.TryParse(valorBusqueda, out int valorInt))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        cmd.Parameters.AddWithValue("@valor", valorInt);
                    }
                    else if (campoSeleccionado == "nombre_sala")
                    {
                        filtro = "s.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "nombre")
                    {
                        filtro = "m.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else
                    {
                        // En caso de que haya otro campo (por si acaso)
                        filtro = $"{campoSeleccionado} LIKE @valor";
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
                    if (dataGridView1.Columns.Contains("estado"))
                        dataGridView1.Columns["estado"].Visible = false;

                    if (dataGridView1.Columns.Contains("idmesa"))
                        dataGridView1.Columns["idmesa"].Visible = false;

                    if (dataGridView1.Columns.Contains("idsala"))
                        dataGridView1.Columns["idsala"].Visible = false;

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
                        m.idmesa, 
                        m.nombre, 
                        m.idsala, 
                        s.nombre AS nombre_sala, 
                        m.cantpersonas, 
                        m.disponible, 
                        m.estado
                    FROM mesas m
                    INNER JOIN salas s ON m.idsala = s.idsala
                    WHERE m.estado = 1 AND ";

                        string filtro = "";
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;

                        if (campoSeleccionado == "idmesa" || campoSeleccionado == "cantpersonas")
                        {
                            filtro = $"m.{campoSeleccionado} = @valor";
                            if (!int.TryParse(valorBusqueda, out int valorInt))
                            {
                                dataGridView1.DataSource = null;
                                return;
                            }
                            cmd.Parameters.AddWithValue("@valor", valorInt);
                        }
                        else if (campoSeleccionado == "nombre_sala")
                        {
                            filtro = "s.nombre LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else if (campoSeleccionado == "nombre")
                        {
                            filtro = "m.nombre LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else
                        {
                            filtro = $"{campoSeleccionado} LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }

                        cmd.CommandText = queryBase + filtro;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        BindingSource bs = new BindingSource();
                        bs.DataSource = dt;
                        dataGridView1.DataSource = bs;

                        // Ocultar columnas internas
                        if (dataGridView1.Columns.Contains("estado"))
                            dataGridView1.Columns["estado"].Visible = false;

                        if (dataGridView1.Columns.Contains("idmesa"))
                            dataGridView1.Columns["idmesa"].Visible = false;

                        if (dataGridView1.Columns.Contains("idsala"))
                            dataGridView1.Columns["idsala"].Visible = false;

                        // Ajustar ancho columnas si quieres, ejemplo:
                        if (dataGridView1.Columns.Contains("nombre_sala"))
                            dataGridView1.Columns["nombre_sala"].Width = 150;

                        // Activar filtros en encabezados
                        foreach (DataGridViewColumn col in dataGridView1.Columns)
                            col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
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
