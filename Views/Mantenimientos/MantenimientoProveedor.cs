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
    public partial class MantenimientoProveedor : Form
    {
        SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;");

        public MantenimientoProveedor()
        {
            InitializeComponent();
            CargarTiposDocumento();
            CargarProvincias();
            CargarDepartamentos();
            CargarProveedores();
            btnborrar.Enabled = false;
        }


        private void EliminarProveedor(int idProveedor)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(@"
            UPDATE proveedor
            SET estado = 0
            WHERE idproveedor = @IdProveedor", connection))
                {
                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);

                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Proveedor eliminado correctamente (estado = 0).", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el proveedor o ya estaba eliminado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void CargarTiposDocumento()
        {
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
            {
                string query = "SELECT idtipo,descripcion from tipo_documento";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxdocumento.DataSource = dt;
                cbxdocumento.DisplayMember = "descripcion";   // Lo que se muestra
                cbxdocumento.ValueMember = "idtipo"; // El valor real
                cbxdocumento.SelectedIndex = -1; // Deja el combo vacío al cargar
            }
        }


        private void CargarProvincias()
        {
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
            {
                string query = "SELECT idprovincia, nombre from provincia";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cbxprovincia.DataSource = dt;
                cbxprovincia.DisplayMember = "nombre";
                cbxprovincia.ValueMember = "idprovincia";
                cbxprovincia.SelectedIndex = -1;
            }
        }

        private void CargarDepartamentos()
        {
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
            {
                String query = "Select iddepartamento, nombre from departamentos";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxdepartamentos.DataSource = dt;
                cbxdepartamentos.DisplayMember = "nombre";
                cbxdepartamentos.ValueMember = "iddepartamento";
                cbxdepartamentos.SelectedIndex = -1;
            }
        }

        private void CargarProveedores()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

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


                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;

                    // Ocultar columnas de ID y estado
                    if (dataGridView1.Columns.Contains("idproveedor"))
                        dataGridView1.Columns["idproveedor"].Visible = false;
                    if (dataGridView1.Columns.Contains("idtipodocumento"))
                        dataGridView1.Columns["idtipodocumento"].Visible = false;
                    if (dataGridView1.Columns.Contains("idprovincia"))
                        dataGridView1.Columns["idprovincia"].Visible = false;
                    if (dataGridView1.Columns.Contains("iddepartamento"))
                        dataGridView1.Columns["iddepartamento"].Visible = false;
                    if (dataGridView1.Columns.Contains("estado"))
                        dataGridView1.Columns["estado"].Visible = false;
                }
            }
        }

        private void CargarProveedorParaEditar(int idProveedor)
        {
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM proveedor WHERE idproveedor = @IdProveedor", con);
                cmd.Parameters.AddWithValue("@IdProveedor", idProveedor);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtndocumento.Text = reader["nodocumento"].ToString();
                        txtndocumento.Tag = idProveedor;  // Aquí guardamos el id real en el Tag
                        cbxdocumento.SelectedValue = Convert.ToInt32(reader["idtipodocumento"]);
                        txtnombre.Text = reader["nombre"].ToString();
                        txtntelefono.Text = reader["telefono"].ToString();
                        cbxprovincia.SelectedValue = Convert.ToInt32(reader["idprovincia"]);
                        cbxdepartamentos.SelectedValue = Convert.ToInt32(reader["iddepartamento"]);
                        txtdireccion.Text = reader["direccion"].ToString();
                        txtcorreo.Text = reader["correo"].ToString();
                        txtvendedor.Text = reader["nombre_vendedor"].ToString();

                        estaEditando = true; // Marca que estás en modo edición
                    }
                }
            }
        }


        private void MantenimientoProveedor_Load(object sender, EventArgs e)
        {

        }

        private bool estaEditando = false;
        private void btnguardar_Click(object sender, EventArgs e)
        {
            // Validaciones de campos obligatorios
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtndocumento.Text))
            {
                MessageBox.Show("Por favor, ingresa el número de documento.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbxdocumento.SelectedIndex == -1 || cbxdocumento.SelectedValue == null)
            {
                MessageBox.Show("Por favor, selecciona un tipo de documento.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtnombre.Text))
            {
                MessageBox.Show("Por favor, ingresa el nombre del proveedor.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtntelefono.Text))
            {
                MessageBox.Show("Por favor, ingresa el número de teléfono.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbxprovincia.SelectedIndex == -1 || cbxprovincia.SelectedValue == null)
            {
                MessageBox.Show("Por favor, selecciona una provincia.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbxdepartamentos.SelectedIndex == -1 || cbxdepartamentos.SelectedValue == null)
            {
                MessageBox.Show("Por favor, selecciona un departamento.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtdireccion.Text))
            {
                MessageBox.Show("Por favor, ingresa la dirección.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtcorreo.Text))
            {
                MessageBox.Show("Por favor, ingresa el correo electrónico.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtvendedor.Text))
            {
                MessageBox.Show("Por favor, ingresa el nombre del vendedor.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int? idProveedor = txtndocumento.Tag as int?;
                bool esActualizacion = estaEditando && idProveedor.HasValue;

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();
                    SqlCommand cmd;

                    if (esActualizacion)
                    {
                        // Verificar duplicado excluyendo el actual
                        SqlCommand checkUpdateCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM proveedor 
                    WHERE nodocumento = @NoDocumento AND idproveedor <> @IdProveedor", con);
                        checkUpdateCmd.Parameters.AddWithValue("@NoDocumento", txtndocumento.Text.Trim());
                        checkUpdateCmd.Parameters.AddWithValue("@IdProveedor", idProveedor.Value);

                        int duplicadoUpdate = (int)checkUpdateCmd.ExecuteScalar();
                        if (duplicadoUpdate > 0)
                        {
                            MessageBox.Show("Ya existe otro proveedor con ese número de documento.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Actualizar
                        cmd = new SqlCommand(@"
                    UPDATE proveedor
                    SET nodocumento = @NoDocumento,
                        idtipodocumento = @IdTipoDocumento,
                        nombre = @Nombre,
                        telefono = @Telefono,
                        idprovincia = @IdProvincia,
                        iddepartamento = @IdDepartamento,
                        direccion = @Direccion,
                        correo = @Correo,
                        nombre_vendedor = @NombreVendedor
                    WHERE idproveedor = @IdProveedor", con);
                        cmd.Parameters.AddWithValue("@IdProveedor", idProveedor.Value);
                    }
                    else
                    {
                        // Verificar duplicado en inserción
                        SqlCommand checkInsertCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM proveedor 
                    WHERE nodocumento = @NoDocumento", con);
                        checkInsertCmd.Parameters.AddWithValue("@NoDocumento", txtndocumento.Text.Trim());

                        int duplicado = (int)checkInsertCmd.ExecuteScalar();
                        if (duplicado > 0)
                        {
                            MessageBox.Show("Ya existe un proveedor con ese número de documento.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Insertar
                        cmd = new SqlCommand(@"
                    INSERT INTO proveedor 
                    (nodocumento, idtipodocumento, nombre, telefono, idprovincia, iddepartamento, direccion, correo, nombre_vendedor)
                    VALUES 
                    (@NoDocumento, @IdTipoDocumento, @Nombre, @Telefono, @IdProvincia, @IdDepartamento, @Direccion, @Correo, @NombreVendedor)", con);
                    }

                    // Parámetros comunes
                    cmd.Parameters.AddWithValue("@NoDocumento", txtndocumento.Text.Trim());
                    cmd.Parameters.AddWithValue("@IdTipoDocumento", Convert.ToInt32(cbxdocumento.SelectedValue));
                    cmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefono", txtntelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@IdProvincia", Convert.ToInt32(cbxprovincia.SelectedValue));
                    cmd.Parameters.AddWithValue("@IdDepartamento", Convert.ToInt32(cbxdepartamentos.SelectedValue));
                    cmd.Parameters.AddWithValue("@Direccion", txtdireccion.Text.Trim());
                    cmd.Parameters.AddWithValue("@Correo", txtcorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@NombreVendedor", txtvendedor.Text.Trim());

                    cmd.ExecuteNonQuery();
                }

                CargarProveedores();
                LimpiarCampos();
                estaEditando = false;
                txtndocumento.Tag = null; // Limpias el Tag para no mantener id viejo


                MessageBox.Show(esActualizacion ? "Proveedor actualizado correctamente" : "Proveedor guardado correctamente",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("unique_no_documento_proveedor"))
                {
                    MessageBox.Show("El número de documento ya existe.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Se produjo un error de base de datos. Verifica los datos e intenta de nuevo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar o actualizar el proveedor:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            cbxdocumento.SelectedIndex = -1;
            txtndocumento.Clear();
            txtnombre.Clear();
            txtntelefono.Clear();
            cbxprovincia.SelectedIndex = -1;
            cbxdepartamentos.SelectedIndex = -1;
            txtdireccion.Clear();
            txtcorreo.Clear();
            txtvendedor.Clear();

            // Si quieres poner el cursor en el primer campo
            cbxdocumento.Focus();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["idproveedor"].Value != null)
            {


                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                estaEditando = true;

                int idProveedor = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["idproveedor"].Value);
                CargarProveedorParaEditar(idProveedor);  

                    cbxdocumento.SelectedValue = Convert.ToInt32(row.Cells["idtipodocumento"].Value);
                txtndocumento.Text = row.Cells["nodocumento"].Value.ToString();
                txtnombre.Text = row.Cells["nombre"].Value.ToString();
                txtntelefono.Text = row.Cells["telefono"].Value.ToString();
                cbxprovincia.SelectedValue = Convert.ToInt32(row.Cells["idprovincia"].Value);
                cbxdepartamentos.SelectedValue = Convert.ToInt32(row.Cells["iddepartamento"].Value);
                txtdireccion.Text = row.Cells["direccion"].Value.ToString();
                txtcorreo.Text = row.Cells["correo"].Value.ToString();
                txtvendedor.Text = row.Cells["nombre_vendedor"].Value.ToString();

                // Guardar el ID en Tag para usarlo en la edición (aquí debe ser txtndocumento)
                txtndocumento.Tag = Convert.ToInt32(row.Cells["idproveedor"].Value);

                btnborrar.Enabled = true;
            }
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            cbxdocumento.SelectedIndex = -1;
            txtndocumento.Clear();
            txtnombre.Clear();
            txtntelefono.Clear();
            cbxprovincia.SelectedIndex = -1;
            cbxdepartamentos.SelectedIndex = -1;
            txtdireccion.Clear();
            txtcorreo.Clear();
            txtvendedor.Clear();

            // Si quieres poner el cursor en el primer campo
            cbxdocumento.Focus();
            estaEditando = false;
            dataGridView1.ClearSelection();
            btnborrar.Enabled = false;
        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["idproveedor"].Value);
                string nombre = dataGridView1.CurrentRow.Cells["nombre"].Value.ToString();

                DialogResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas desactivar este proveedor: {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    EliminarProveedor(id);  // Marca como estado = 0
                    CargarProveedores();    // Recarga el DataGridView con los proveedores
                    LimpiarCampos();        // Limpia los campos del formulario
                    btnborrar.Enabled = false;
                    estaEditando = false;
                    txtndocumento.Tag = null; // Limpia el tag para evitar confusiones futuras
                }
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor primero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
