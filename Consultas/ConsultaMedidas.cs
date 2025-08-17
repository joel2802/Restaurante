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
    public partial class ConsultaMedidas : Form
    {
        string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
        public ConsultaMedidas()
        {
            InitializeComponent();
            CargarDatos();
        }
        public int? MedidaIdSeleccionada { get; private set; }
        public string NombreMedidaSeleccionada { get; private set; }
        public string AbreviaturaSeleccionada { get; private set; }

        private void CargarDatos()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = @"
                SELECT 
                    m.idmedida,
                    m.nombre,
                    m.sigla,
                    m.estado
                FROM medidas m
                WHERE m.estado = 1";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        BindingSource bs = new BindingSource();
                        bs.DataSource = dt;
                        dataGridView1.DataSource = bs;

                        // 🔹 Ocultar columnas internas
                        if (dataGridView1.Columns.Contains("estado"))
                            dataGridView1.Columns["estado"].Visible = false;

                        if (dataGridView1.Columns.Contains("idmedida"))
                            dataGridView1.Columns["idmedida"].Visible = false;

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

                    // 🔹 Configurar ComboBox de campos
                    cbxcampo.Items.Clear();
                    cbxcampo.Items.Add("nombre");
                    cbxcampo.Items.Add("sigla");
                    cbxcampo.SelectedItem = "nombre";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar medidas: " + ex.Message);
                }
            }
        }
        private void ConsultaMedidas_Load(object sender, EventArgs e)
        {
            cbxcampo.Items.Clear();
            cbxcampo.Items.Add("nombre");
            cbxcampo.Items.Add("sigla");

            // 🔹 Seleccionar por defecto "nombre"
            cbxcampo.SelectedItem = "nombre";

            // 🔹 Quitar selección automática en DataGridView
            dataGridView1.ClearSelection();
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
            idmedida,
            nombre,
            sigla,
            estado
        FROM medidas
        WHERE estado = 1 AND ";

                    string filtro = "";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    // 🔹 Filtros especiales
                    if (campoSeleccionado == "idmedida")
                    {
                        filtro = "idmedida = @valor";
                        if (!int.TryParse(valorBusqueda, out int idMedida))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        cmd.Parameters.AddWithValue("@valor", idMedida);
                    }
                    else if (campoSeleccionado == "estado")
                    {
                        filtro = "estado = @valor";
                        if (!int.TryParse(valorBusqueda, out int estadoVal))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        cmd.Parameters.AddWithValue("@valor", estadoVal);
                    }
                    else
                    {
                        // 🔹 Filtro genérico con LIKE insensible a mayúsculas/minúsculas y acentos
                        filtro = $"{campoSeleccionado} COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }

                    cmd.CommandText = queryBase + filtro;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable tabla = new DataTable();
                    adapter.Fill(tabla);

                    BindingSource bs = new BindingSource();
                    bs.DataSource = tabla;
                    dataGridView1.DataSource = bs;

                    // 🔹 Ocultar columnas internas
                    if (dataGridView1.Columns.Contains("estado"))
                        dataGridView1.Columns["estado"].Visible = false;

                    if (dataGridView1.Columns.Contains("idmedida"))
                        dataGridView1.Columns["idmedida"].Visible = false;

                    // 🔹 Limpiar selección
                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;

                    // 🔹 Encabezados con filtro
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

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        string queryBase = @"
                    SELECT 
                        idmedida,
                        nombre,
                        abreviatura,
                        estado
                    FROM medidas
                    WHERE estado = 1 AND ";

                        string filtro = "";
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;

                        if (campoSeleccionado == "idmedida")
                        {
                            filtro = "idmedida = @valor";
                            if (!int.TryParse(valorBusqueda, out int idMedida))
                            {
                                dataGridView1.DataSource = null;
                                return;
                            }
                            cmd.Parameters.AddWithValue("@valor", idMedida);
                        }
                        else if (campoSeleccionado == "estado")
                        {
                            filtro = "estado = @valor";
                            if (!int.TryParse(valorBusqueda, out int estadoVal))
                            {
                                dataGridView1.DataSource = null;
                                return;
                            }
                            cmd.Parameters.AddWithValue("@valor", estadoVal);
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

                        if (dataGridView1.Columns.Contains("idmedida"))
                            dataGridView1.Columns["idmedida"].Visible = false;

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            // Asigna el ID seleccionado
            MedidaIdSeleccionada = row.Cells["IdMedida"].Value != DBNull.Value
                ? Convert.ToInt32(row.Cells["IdMedida"].Value)
                : (int?)null;

            // Asigna nombre y abreviatura
            NombreMedidaSeleccionada = row.Cells["Nombre"].Value?.ToString() ?? "";
            AbreviaturaSeleccionada = row.Cells["Abreviatura"].Value?.ToString() ?? "";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
