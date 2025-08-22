using Restaurante.Views.Ventanas;
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
    public partial class MantenimientoMesas : Form
    {
        public MantenimientoMesas()
        {
            InitializeComponent();
            CargarDatosMesas();
            CargarSalas();
            btnborrar.Enabled = false;

            EventosGlobales.PedidoEntregado += () =>
            {
                CargarDatosMesas();         
                ActualizarColoresMesas();  
            };
        }

       
        public void CargarDatosMesas()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT m.idmesa, m.nombre, m.idsala, s.nombre AS nombre_sala, 
                   m.cantpersonas, m.disponible, m.estado
            FROM mesas m
            INNER JOIN salas s ON m.idsala = s.idsala
            WHERE m.estado = 1";

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

                    if (dataGridView1.Columns.Contains("idmesa"))
                        dataGridView1.Columns["idmesa"].Visible = false;

                    if (dataGridView1.Columns.Contains("idsala"))
                        dataGridView1.Columns["idsala"].Visible = false;

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

        public void ActualizarColoresMesas()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["disponible"].Value != null)
                {
                    bool libre = Convert.ToInt32(row.Cells["disponible"].Value) == 1;
                    row.DefaultCellStyle.BackColor = libre ? Color.LightGreen : Color.LightCoral;
                }
            }
        }

        private void AbrirPedidosPendientes()
        {
            var formPedidosPendientes = new PedidosPendientes();

          
            formPedidosPendientes.PedidoEntregado += () => CargarDatosMesas();

            formPedidosPendientes.Show();
        }

       
        private void btnVerPedidosPendientes_Click(object sender, EventArgs e)
        {
            AbrirPedidosPendientes();
        }



        private void cbxmesa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxmesa.SelectedIndex != -1)
            {
                int idsala = Convert.ToInt32(cbxmesa.SelectedValue);
                CargarMesasPorSala(idsala);
            }
        }

        private void CargarMesasPorSala(int idsala)
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT m.idmesa, m.nombre, m.idsala, s.nombre AS nombre_sala, 
                   m.cantpersonas, m.disponible, m.estado
            FROM mesas m
            INNER JOIN salas s ON m.idsala = s.idsala
            WHERE m.estado = 1 AND m.idsala = @IdSala";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdSala", idsala);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;

                  
                    if (dataGridView1.Columns.Contains("estado"))
                        dataGridView1.Columns["estado"].Visible = false;
                    if (dataGridView1.Columns.Contains("idmesa"))
                        dataGridView1.Columns["idmesa"].Visible = false;
                    if (dataGridView1.Columns.Contains("idsala"))
                        dataGridView1.Columns["idsala"].Visible = false;
                }
            }
        }


        private void ConfigurarColumnasDataGridView()
        {
            dataGridView1.Columns.Clear();

           
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Sala", HeaderText = "Sala" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "nombre", HeaderText = "Nombre" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "CantidadPersona", HeaderText = "Cantidad Persona" });

            
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void EliminarMesa(int idMesa)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(@"
            UPDATE mesas
            SET estado = 0
            WHERE idmesa = @IdMesa", connection))
                {
                    command.Parameters.AddWithValue("@IdMesa", idMesa);

                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Mesa eliminada correctamente (estado = 0).", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la mesa o ya estaba eliminada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }



        private void CargarSalas()
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();
                    string query = "SELECT idsala, nombre FROM salas WHERE estado = 1";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    var salasDict = new Dictionary<int, string>();

                    while (reader.Read())
                    {
                        int idSala = (int)reader["idsala"];
                        string nombre = reader["nombre"].ToString();
                        salasDict.Add(idSala, nombre);
                    }

                    cbxmesa.DataSource = new BindingSource(salasDict, null);
                    cbxmesa.DisplayMember = "Value";  
                    cbxmesa.ValueMember = "Key";     
                    cbxmesa.SelectedIndex = -1;      
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar salas: " + ex.Message);
            }
        }
        private bool estaEditando = false;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["idmesa"].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                estaEditando = true;

                txtmesa.Text = row.Cells["nombre"].Value.ToString();
                numericUpDown1.Value = Convert.ToDecimal(row.Cells["cantpersonas"].Value);

               
                if (cbxmesa.Items.Count > 0)
                {
                
                    cbxmesa.SelectedValue = Convert.ToInt32(row.Cells["idsala"].Value);
                }

             
                txtmesa.Tag = row.Cells["idmesa"].Value;
                btnborrar.Enabled = true;
            }
        }


        private void LimpiarCampos()
        {
            
            txtmesa.Clear();
            numericUpDown1.Value = numericUpDown1.Minimum;
            cbxmesa.SelectedIndex = -1;
            estaEditando = false;

        }
        private void btnborrar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["idmesa"].Value);
                string nombre = dataGridView1.CurrentRow.Cells["nombre"].Value.ToString();

                DialogResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas desactivar esta mesa: {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    EliminarMesa(id);  
                    CargarDatosMesas();      
                    LimpiarCampos();  
                    btnborrar.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Seleccione una mesa primero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {

            txtmesa.Clear();
            numericUpDown1.Value = numericUpDown1.Minimum;
            cbxmesa.SelectedIndex = -1;
            btnborrar.Enabled = false;

            dataGridView1.ClearSelection();
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtmesa.Text))
            {
                MessageBox.Show("Por favor, ingresa un nombre de mesa.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbxmesa.SelectedIndex == -1 || cbxmesa.SelectedValue == null)
            {
                MessageBox.Show("Por favor, selecciona una sala antes de guardar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numericUpDown1.Value == 0)
            {
                MessageBox.Show("Por favor, ingresa una cantidad válida de personas.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int? idMesa = txtmesa.Tag as int?;
                bool esActualizacion = estaEditando && idMesa.HasValue;

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();
                    SqlCommand cmd;

                    int idSala = Convert.ToInt32(cbxmesa.SelectedValue);

                    if (esActualizacion)
                    {
                        SqlCommand checkUpdateCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM mesas 
                    WHERE nombre = @Nombre AND idsala = @IdSala AND idmesa <> @IdMesa", con);
                        checkUpdateCmd.Parameters.AddWithValue("@Nombre", txtmesa.Text.Trim());
                        checkUpdateCmd.Parameters.AddWithValue("@IdSala", idSala);
                        checkUpdateCmd.Parameters.AddWithValue("@IdMesa", idMesa.Value);

                        int duplicadoUpdate = (int)checkUpdateCmd.ExecuteScalar();
                        if (duplicadoUpdate > 0)
                        {
                            MessageBox.Show("Ya existe otra mesa con ese nombre en esta sala.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                       
                        cmd = new SqlCommand(@"
                    UPDATE mesas
                    SET nombre = @Nombre, idsala = @IdSala, cantpersonas = @CantPersonas
                    WHERE idmesa = @IdMesa", con);
                        cmd.Parameters.AddWithValue("@IdMesa", idMesa.Value);
                    }
                    else
                    {
                        SqlCommand checkInsertCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM mesas 
                    WHERE nombre = @Nombre AND idsala = @IdSala", con);
                        checkInsertCmd.Parameters.AddWithValue("@Nombre", txtmesa.Text.Trim());
                        checkInsertCmd.Parameters.AddWithValue("@IdSala", idSala);

                        int duplicado = (int)checkInsertCmd.ExecuteScalar();
                        if (duplicado > 0)
                        {
                            MessageBox.Show("Ya existe una mesa con ese nombre en esta sala.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                      
                        cmd = new SqlCommand(@"
                    INSERT INTO mesas (nombre, idsala, cantpersonas)
                    VALUES (@Nombre, @IdSala, @CantPersonas)", con);
                    }

                  
                    cmd.Parameters.AddWithValue("@Nombre", txtmesa.Text.Trim());
                    cmd.Parameters.AddWithValue("@IdSala", idSala);
                    cmd.Parameters.AddWithValue("@CantPersonas", (int)numericUpDown1.Value);

                    cmd.ExecuteNonQuery();
                }

                CargarDatosMesas();
                LimpiarCampos();

                MessageBox.Show(esActualizacion ? "Mesa actualizada correctamente" : "Mesa guardada correctamente",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("unique_sala_mesa"))
                {
                    MessageBox.Show("El nombre de la mesa ya existe en esta sala.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Se produjo una violación de restricción de clave única. Por favor, verifica tus datos e intenta de nuevo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar o actualizar la mesa:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MantenimientoMesas_Load(object sender, EventArgs e)
        {

        }
    }
}
