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
    public partial class MantenimientoMedida : Form
    {
        public MantenimientoMedida()
        {
            InitializeComponent();
            CargarDatosMedidas();
            btnborrar.Enabled = false;
        }

        private void LimpiarCampos()
        {
            txtnombre.Clear();
            txtmedida.Clear();
            estaEditando = false;


        }

      
        private bool estaEditando = false;
        private void CargarDatosMedidas()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"SELECT * FROM medidas WHERE estado = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = dt;

                    dataGridView1.DataSource = bindingSource;

                   
                    if (dataGridView1.Columns.Contains("estado"))
                        dataGridView1.Columns["estado"].Visible = false;

                    if (dataGridView1.Columns.Contains("idmedida"))
                        dataGridView1.Columns["idmedida"].Visible = false;

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

        private void EliminarMedida(int id)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            UPDATE medidas
            SET estado = 0
            WHERE idmedida = @IdMedida", connection))
                {
                    command.Parameters.AddWithValue("@IdMedida", id);
                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Medida eliminada (estado = 0)", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la medida o ya estaba eliminada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        private void MantenimientoMedida_Load(object sender, EventArgs e)
        {

        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            txtnombre.Clear();
            txtmedida.Clear();
            btnborrar.Enabled = false;
            LimpiarCampos();
            dataGridView1.ClearSelection();
        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["IdMedida"].Value);
                string nombre = dataGridView1.CurrentRow.Cells["Nombre"].Value.ToString();

                DialogResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas desactivar esta medida: {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    EliminarMedida(id);
                    CargarDatosMedidas();       
                    LimpiarCampos();     
                    btnborrar.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Seleccione una medida primero.");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["IdMedida"].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                estaEditando = true;

                txtnombre.Text = row.Cells["Nombre"].Value.ToString();
                txtmedida.Text = row.Cells["Sigla"].Value.ToString();

                txtnombre.Tag = row.Cells["IdMedida"].Value; 
                btnborrar.Enabled = true;
            }
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtnombre.Text) || string.IsNullOrWhiteSpace(txtmedida.Text))
            {
                MessageBox.Show("Por favor, ingresa nombre y sigla.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int? idMedida = txtnombre.Tag as int?; 
                bool esActualizacion = estaEditando && idMedida.HasValue;

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();
                    SqlCommand cmd;

                    if (esActualizacion)
                    {
                      
                        SqlCommand checkUpdateCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM medidas 
                    WHERE nombre = @Nombre AND sigla = @Sigla AND idmedida <> @IdMedida", con);
                        checkUpdateCmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());
                        checkUpdateCmd.Parameters.AddWithValue("@Sigla", txtmedida.Text.Trim());
                        checkUpdateCmd.Parameters.AddWithValue("@IdMedida", idMedida.Value);

                        int duplicadoUpdate = (int)checkUpdateCmd.ExecuteScalar();
                        if (duplicadoUpdate > 0)
                        {
                            MessageBox.Show("Ya existe otra medida con ese nombre y sigla.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                     
                        cmd = new SqlCommand(@"
                    UPDATE medidas
                    SET nombre = @Nombre, sigla = @Sigla
                    WHERE idmedida = @IdMedida", con);
                        cmd.Parameters.AddWithValue("@IdMedida", idMedida.Value);
                    }
                    else
                    {
                       
                        SqlCommand checkInsertCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM medidas 
                    WHERE nombre = @Nombre AND sigla = @Sigla", con);
                        checkInsertCmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());
                        checkInsertCmd.Parameters.AddWithValue("@Sigla", txtmedida.Text.Trim());

                        int duplicado = (int)checkInsertCmd.ExecuteScalar();
                        if (duplicado > 0)
                        {
                            MessageBox.Show("Ya existe una medida con ese nombre y sigla.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                     
                        cmd = new SqlCommand(@"
                    INSERT INTO medidas (nombre, sigla)
                    VALUES (@Nombre, @Sigla)", con);
                    }

                    cmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Sigla", txtmedida.Text.Trim());
                    cmd.ExecuteNonQuery();
                }

                CargarDatosMedidas();     
                LimpiarCampos();    
                estaEditando = false;

                MessageBox.Show(esActualizacion ? "Medida actualizada correctamente" : "Medida guardada correctamente",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar o actualizar la medida:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
