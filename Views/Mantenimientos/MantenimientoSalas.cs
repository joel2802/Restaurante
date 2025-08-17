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

namespace Restaurante.Views.Mantenimientos
{
    public partial class MantenimientoSalas : Form
    {
        public MantenimientoSalas()
        {
            InitializeComponent();
            CargarDatos();
            CargarSucursales();
            btnborrar.Enabled = false;
        }



        private void CargarDatos()
        {

            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
            int idsucursal = 1; // Puedes cambiar este valor o asignarlo dinámicamente

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT s.*, sc.nombre AS nombre_sucursal
            FROM salas s
            INNER JOIN sucursal sc ON s.idsucursal = sc.idsucursal 
            WHERE s.estado = 1 AND s.idsucursal = @IdSucursal";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@IdSucursal", SqlDbType.Int).Value = idsucursal;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = dt;

                    dataGridView1.DataSource = bindingSource;

                    dataGridView1.Columns["estado"].Visible = false;

                    if (dataGridView1.Columns.Contains("idsala"))
                        dataGridView1.Columns["idsala"].Visible = false;

                    if (dataGridView1.Columns.Contains("idsucursal"))
                        dataGridView1.Columns["idsucursal"].Visible = false;

                    dataGridView1.Refresh();

                    
                    dataGridView1.ScrollBars = ScrollBars.Both;
                    
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


                    

                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                    {
                        col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                    }
                }
            }
        }

        private void CargarSucursales()
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();
                    string query = "SELECT idsucursal, nombre FROM sucursal";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cbxsucursal.DataSource = dt;
                    cbxsucursal.DisplayMember = "nombre";       // visible
                    cbxsucursal.ValueMember = "idsucursal";     // el ID (int)
                    cbxsucursal.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar sucursales: " + ex.Message);
            }
        }

        private bool estaEditando = false;
        private void LimpiarCampos()
        {
            // Limpiar campos de texto
            txtsalas.Clear();
            cbxsucursal.SelectedIndex = -1;
            estaEditando = false;

        }
        private void MantenimientoSalas_Load(object sender, EventArgs e)
        {

        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtsalas.Text))
            {
                MessageBox.Show("Por favor, ingresa un nombre de sala.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbxsucursal.SelectedIndex == -1 || cbxsucursal.SelectedValue == null)
            {
                MessageBox.Show("Por favor, selecciona una sucursal antes de guardar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int? idSala = txtsalas.Tag as int?;
                bool esActualizacion = estaEditando && idSala.HasValue;

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();
                    SqlCommand cmd;

                    int idSucursal = Convert.ToInt32(cbxsucursal.SelectedValue);

                    if (esActualizacion)
                    {
                        // Validar duplicado en actualización
                        SqlCommand checkUpdateCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM salas 
                    WHERE nombre = @Nombre AND idsucursal = @IdSucursal AND idsala <> @IdSala", con);
                        checkUpdateCmd.Parameters.AddWithValue("@Nombre", txtsalas.Text.Trim());
                        checkUpdateCmd.Parameters.AddWithValue("@IdSucursal", idSucursal);
                        checkUpdateCmd.Parameters.AddWithValue("@IdSala", idSala.Value);

                        int duplicadoUpdate = (int)checkUpdateCmd.ExecuteScalar();
                        if (duplicadoUpdate > 0)
                        {
                            MessageBox.Show("Ya existe otra sala con ese nombre en esta sucursal.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Actualizar
                        cmd = new SqlCommand(@"
                    UPDATE salas
                    SET nombre = @Nombre, idsucursal = @IdSucursal
                    WHERE idsala = @IdSala", con);
                        cmd.Parameters.AddWithValue("@IdSala", idSala.Value);
                    }
                    else
                    {
                        // Validar duplicado al insertar
                        SqlCommand checkInsertCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM salas 
                    WHERE nombre = @Nombre AND idsucursal = @IdSucursal", con);
                        checkInsertCmd.Parameters.AddWithValue("@Nombre", txtsalas.Text.Trim());
                        checkInsertCmd.Parameters.AddWithValue("@IdSucursal", idSucursal);

                        int duplicado = (int)checkInsertCmd.ExecuteScalar();
                        if (duplicado > 0)
                        {
                            MessageBox.Show("Ya existe una sala con ese nombre en esta sucursal.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Insertar
                        cmd = new SqlCommand(@"
                    INSERT INTO salas (nombre, idsucursal)
                    VALUES (@Nombre, @IdSucursal)", con);
                    }

                    // Parámetros comunes
                    cmd.Parameters.AddWithValue("@Nombre", txtsalas.Text.Trim());
                    cmd.Parameters.AddWithValue("@IdSucursal", idSucursal);

                    cmd.ExecuteNonQuery();
                }

                CargarDatos();
                LimpiarCampos();

                MessageBox.Show(esActualizacion ? "Sala actualizada correctamente" : "Sala guardada correctamente",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar o actualizar la sala:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarSala(int idSala)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(@"
            UPDATE salas
            SET estado = 0
            WHERE idsala = @IdSala", connection))
                {
                    command.Parameters.AddWithValue("@IdSala", idSala);

                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Sala eliminada correctamente (estado = 0).", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la sala o ya estaba eliminada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            txtsalas.Clear();
            btnborrar.Enabled = false;
            LimpiarCampos();
                   
            cbxsucursal.SelectedIndex = -1;  // Limpia la selección del ComboBox
            
            dataGridView1.ClearSelection();
        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["idsala"].Value);
                string nombre = dataGridView1.CurrentRow.Cells["nombre"].Value.ToString();

                DialogResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas desactivar esta sala: {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    EliminarSala(id);
                    CargarDatos();
                    LimpiarCampos();
                    btnborrar.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Seleccione una sala primero.");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            estaEditando = true;
            txtsalas.Text = row.Cells["nombre"].Value.ToString();
            txtsalas.Tag = row.Cells["idsala"].Value;  // Guarda el id para editar
            cbxsucursal.SelectedValue = Convert.ToInt32(row.Cells["idsucursal"].Value); // Cargar sucursal
            btnborrar.Enabled = true;
        }
    }
}
