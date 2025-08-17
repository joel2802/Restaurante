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
    public partial class ConsultaCategorias : Form
    {

        string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
        public int? CategoriaIdSeleccionada { get; private set; }
        public string NombreCategoriaSeleccionada { get; private set; }
        public ConsultaCategorias()
        {
            InitializeComponent();
            CargarDatos();

           
        }

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
                    idcategoria AS IdCategoria, 
                    nombre AS Nombre,
                    estado AS Estado
                FROM categorias
                WHERE estado = 1";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bs = new BindingSource();
                    bs.DataSource = dt;
                    dataGridView1.DataSource = bs;
                    

                    // Ocultar columnas que no se mostrarán
                    if (dataGridView1.Columns.Contains("IdCategoria"))
                        dataGridView1.Columns["IdCategoria"].Visible = false;

                    if (dataGridView1.Columns.Contains("Estado"))
                        dataGridView1.Columns["Estado"].Visible = false;

                    // Ajustar tamaño de columnas
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Agregar filtro en encabezados si tienes DataGridViewAutoFilter
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar las categorías: " + ex.Message);
                }
            }
        }
        private void ConsultaCategorias_Load(object sender, EventArgs e)
        {
            cbxcampo.Items.Clear();

            cbxcampo.Items.Add("nombre");
            // cbxcampo.Items.Add("estado"); // opcional, si quieres filtrar por estado

            cbxcampo.SelectedItem = "nombre";  // No seleccionar nada al inicio
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Evita errores al hacer clic en encabezados

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            CategoriaIdSeleccionada = row.Cells["IdCategoria"].Value != DBNull.Value
                ? Convert.ToInt32(row.Cells["IdCategoria"].Value)
                : (int?)null;

            NombreCategoriaSeleccionada = row.Cells["Nombre"].Value?.ToString() ?? "";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            string campoSeleccionado = cbxcampo.SelectedItem?.ToString();
            string valorBusqueda = txtbuscar.Text.Trim();

            if (string.IsNullOrEmpty(campoSeleccionado) || string.IsNullOrEmpty(valorBusqueda))
            {
                CargarDatos();
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string queryBase = @"
                SELECT 
                    idcategoria AS IdCategoria, 
                    nombre AS Nombre,
                    estado AS Estado
                FROM categorias
                WHERE estado = 1 AND ";

                    string filtro;
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    if (campoSeleccionado == "IdCategoria" || campoSeleccionado == "Estado")
                    {
                        filtro = campoSeleccionado + " = @valor";
                        if (!int.TryParse(valorBusqueda, out int valorInt))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        cmd.Parameters.AddWithValue("@valor", valorInt);
                    }
                    else // Texto (Nombre)
                    {
                        filtro = campoSeleccionado + " LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }

                    cmd.CommandText = queryBase + filtro;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bs = new BindingSource();
                    bs.DataSource = dt;
                    dataGridView1.DataSource = bs;

                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;

                    // Ocultar columnas
                    if (dataGridView1.Columns.Contains("IdCategoria"))
                        dataGridView1.Columns["IdCategoria"].Visible = false;
                    if (dataGridView1.Columns.Contains("Estado"))
                        dataGridView1.Columns["Estado"].Visible = false;

                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;
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
                    idcategoria AS IdCategoria, 
                    nombre AS Nombre,
                    estado AS Estado
                FROM categorias
                WHERE estado = 1 AND ";

                        string filtro = "";
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;

                        if (campoSeleccionado == "idcategoria")
                        {
                            filtro = "idcategoria = @valor";
                            if (!int.TryParse(valorBusqueda, out int idCategoria))
                            {
                                dataGridView1.DataSource = null;
                                return;
                            }
                            cmd.Parameters.AddWithValue("@valor", idCategoria);
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
                        dataGridView1.ClearSelection();

                        // Ocultar columnas no necesarias
                        if (dataGridView1.Columns.Contains("IdCategoria"))
                            dataGridView1.Columns["IdCategoria"].Visible = false;
                        if (dataGridView1.Columns.Contains("Estado"))
                            dataGridView1.Columns["Estado"].Visible = false;

                        // Ajustar tamaño columnas
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // Activar filtros
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

        private void btnmostrar_Click(object sender, EventArgs e)
        {
            txtbuscar.Text = "";
            cbxcampo.SelectedIndex = 0;  // Temporal para forzar refresco
            CargarDatos();
            cbxcampo.SelectedIndex = -1; // Ninguno seleccionado
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
        }
    }
}
