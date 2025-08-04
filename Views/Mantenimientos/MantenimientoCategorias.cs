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
using DataGridViewAutoFilter;

namespace Restaurante.Views.Mantenimientos
{
    public partial class MantenimientoCategorias : Form
    {
        public MantenimientoCategorias()
        {
            InitializeComponent();
            CargarCategoriasEnGrid();
            CargarDatos();
            btnborrar.Enabled = false;
        }


        private void CargarDatos()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
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

                // --- Cambio clave: Usar BindingSource ---
                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = dt; // Enlazar el DataTable al BindingSource
                dataGridView1.DataSource = bindingSource; // Asignar el BindingSource al DataGridView

                // Configuración de columnas (opcional)
                if (dataGridView1.Columns.Contains("IdCategoria"))
                    dataGridView1.Columns["IdCategoria"].Visible = false;

                dataGridView1.Columns["estado"].Visible = false;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // --- Asegúrate de que DataGridViewAutoFilter esté configurado correctamente ---
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }

            }
        }
        private void CargarCategoriasEnGrid()
        {
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM categorias", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Mostrar todas las columnas
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.Visible = true;
                }

                
                dataGridView1.AutoResizeColumns();
            }
        }


        private void MantenimientoCategorias_Load(object sender, EventArgs e)
        {

        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtnombre.Text))
            {
                MessageBox.Show("Por favor, ingresa un nombre de categoría.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int? idCategoria = txtnombre.Tag as int?;
                // Usamos la variable para controlar edición
                bool esActualizacion = estaEditando && idCategoria.HasValue;

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();
                    SqlCommand cmd;

                    if (esActualizacion)
                    {
                        // Validar duplicado en actualización
                        SqlCommand checkUpdateCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM categorias 
                    WHERE nombre = @Nombre AND idcategoria <> @IdCategoria", con);
                        checkUpdateCmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());
                        checkUpdateCmd.Parameters.AddWithValue("@IdCategoria", idCategoria.Value);

                        int duplicadoUpdate = (int)checkUpdateCmd.ExecuteScalar();
                        if (duplicadoUpdate > 0)
                        {
                            MessageBox.Show("Ya existe otra categoría con ese nombre.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Actualizar
                        cmd = new SqlCommand(@"
                    UPDATE categorias
                    SET nombre = @Nombre
                    WHERE idcategoria = @IdCategoria", con);
                        cmd.Parameters.AddWithValue("@IdCategoria", idCategoria.Value);
                    }
                    else
                    {
                        // Validar duplicado al insertar
                        SqlCommand checkInsertCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM categorias 
                    WHERE nombre = @Nombre", con);
                        checkInsertCmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());

                        int duplicado = (int)checkInsertCmd.ExecuteScalar();
                        if (duplicado > 0)
                        {
                            MessageBox.Show("Ya existe una categoría con ese nombre.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Insertar
                        cmd = new SqlCommand(@"
                    INSERT INTO categorias (nombre, estado)
                    VALUES (@Nombre, 1)", con);
                    }

                    // Parámetro común
                    cmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());
                    cmd.ExecuteNonQuery();
                }

                CargarDatos();
                LimpiarCampos();

                MessageBox.Show(esActualizacion ? "Categoría actualizada correctamente" : "Categoría guardada correctamente",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar o actualizar la categoría:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtnombre_TextChanged(object sender, EventArgs e)
        {

        }
        private void LimpiarCampos()
        {
            // Limpiar campos de texto
            txtnombre.Clear();
            estaEditando = false;

        }
        private void btnlimpiar_Click(object sender, EventArgs e)
        {

            txtnombre.Clear();
            btnborrar.Enabled = false;
            LimpiarCampos();
            dataGridView1.ClearSelection(); // 🔁 Deselecciona cualquier fila

        }


        private void EliminarCategoria(int id)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            UPDATE categorias
            SET estado = 0
            WHERE idcategoria = @IdCategoria", connection))
                {
                    command.Parameters.AddWithValue("@IdCategoria", id);
                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Categoría eliminada (estado = 0)", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la categoría o ya estaba eliminada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        private void btnborrar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["IdCategoria"].Value);
                string nombre = dataGridView1.CurrentRow.Cells["Nombre"].Value.ToString();

                DialogResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas desactivar esta categoría: {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    EliminarCategoria(id);
                    CargarDatos();
                    LimpiarCampos();
                    btnborrar.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Seleccione una categoría primero.");
            }
        }
        private bool estaEditando = false;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["IdCategoria"].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                estaEditando = true;
                txtnombre.Text = row.Cells["Nombre"].Value.ToString();
                txtnombre.Tag = row.Cells["IdCategoria"].Value; // Solo si quieres mantener, pero con esta variable es opcional
                btnborrar.Enabled = true;
            }
        }
    }
}
