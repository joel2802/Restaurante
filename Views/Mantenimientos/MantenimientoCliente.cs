using Restaurante.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Restaurante.Views.Mantenimientos
{
    public partial class MantenimientoCliente : Form
    {
        SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;");
        public MantenimientoCliente()
        {
            
            InitializeComponent();
            CargarClientesEnGrid();
            CargarDatos();
            CargarTipoCliente();
            CargarTipoDocumento();
            CargarProvincias();
            panel1.Visible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;


            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;

            btnborrar.Enabled = false;
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            
            txtndocumento.Clear();
            txtnombre.Clear();
            txtrazon.Clear();
            txtgirocliente.Clear();
            txttelf.Clear();
            txtcorreo.Clear();
            txtdireccion.Clear();
            txtcredito.Clear();

            
            btntipocliente.SelectedIndex = -1;
            btntipodocumento.SelectedIndex = -1;
            cbxprovincia.SelectedIndex = -1;

            
            btntipocliente.Focus();
            dataGridView1.ClearSelection();
            estaEditandoCliente = false;

            btnborrar.Enabled = false;
        }
        private void CargarTipoCliente()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT idtipo, nombre FROM tipo_cliente", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                btntipocliente.DataSource = dt;
                btntipocliente.DisplayMember = "nombre";
                btntipocliente.ValueMember = "idtipo";
                btntipocliente.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tipo de cliente: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void CargarTipoDocumento()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT idtipo, descripcion FROM tipo_documento", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                btntipodocumento.DataSource = dt;
                btntipodocumento.DisplayMember = "descripcion";
                btntipodocumento.ValueMember = "idtipo";
                btntipodocumento.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tipo de documento: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void CargarProvincias()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT idprovincia, nombre FROM provincia", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxprovincia.DataSource = dt;
                cbxprovincia.DisplayMember = "nombre";
                cbxprovincia.ValueMember = "idprovincia";
                cbxprovincia.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar provincias: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }



       
        private void btnguardarr_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposCliente())
                return;

            if (string.IsNullOrWhiteSpace(txtcredito.Text))
                txtcredito.Text = "0";

            try
            {
                int? idCliente = txtndocumento.Tag as int?;
                bool esActualizacion = estaEditandoCliente && idCliente.HasValue;

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();

                    SqlCommand cmd;

                    if (esActualizacion)
                    {
                        
                        SqlCommand checkUpdateCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM clientes 
                    WHERE nodocumento = @NoDocumento AND idcliente <> @IdCliente", con);
                        checkUpdateCmd.Parameters.AddWithValue("@NoDocumento", txtndocumento.Text.Trim());
                        checkUpdateCmd.Parameters.AddWithValue("@IdCliente", idCliente.Value);

                        int duplicadoUpdate = (int)checkUpdateCmd.ExecuteScalar();
                        if (duplicadoUpdate > 0)
                        {
                            MessageBox.Show("Ya existe otro cliente con ese número de documento.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                       
                        cmd = new SqlCommand(@"
                    UPDATE clientes
                    SET 
                        idtipocliente = @IdTipoCliente,
                        idtipodocumento = @IdTipoDocumento,
                        nodocumento = @NoDocumento,
                        nombre = @Nombre,
                        razonsocial = @RazonSocial,
                        girocliente = @GiroCliente,
                        telefono = @Telefono,
                        correo = @Correo,
                        direccion = @Direccion,
                        idprovincia = @IdProvincia,
                        limitecredito = @LimiteCredito
                    WHERE idcliente = @IdCliente", con);

                        cmd.Parameters.AddWithValue("@IdCliente", idCliente.Value);
                    }
                    else
                    {
                        
                        SqlCommand checkInsertCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM clientes 
                    WHERE nodocumento = @NoDocumento", con);
                        checkInsertCmd.Parameters.AddWithValue("@NoDocumento", txtndocumento.Text.Trim());

                        int duplicado = (int)checkInsertCmd.ExecuteScalar();
                        if (duplicado > 0)
                        {
                            MessageBox.Show("Ya existe un cliente con ese número de documento.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        
                        cmd = new SqlCommand(@"
                    INSERT INTO clientes
                    (idtipocliente, idtipodocumento, nodocumento, nombre, razonsocial, girocliente, telefono, correo, direccion, estado, idprovincia, fecha_creacion, limitecredito)
                    VALUES 
                    (@IdTipoCliente, @IdTipoDocumento, @NoDocumento, @Nombre, @RazonSocial, @GiroCliente, @Telefono, @Correo, @Direccion, @Estado, @IdProvincia, @FechaCreacion, @LimiteCredito)", con);

                        cmd.Parameters.AddWithValue("@Estado", 1);
                        cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                    }

                    
                    cmd.Parameters.AddWithValue("@IdTipoCliente", btntipocliente.SelectedValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdTipoDocumento", btntipodocumento.SelectedValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdProvincia", cbxprovincia.SelectedValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoDocumento", txtndocumento.Text.Trim());
                    cmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@RazonSocial", txtrazon.Text.Trim());
                    cmd.Parameters.AddWithValue("@GiroCliente", txtgirocliente.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefono", txttelf.Text.Trim());
                    cmd.Parameters.AddWithValue("@Correo", txtcorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Direccion", txtdireccion.Text.Trim());

                    if (decimal.TryParse(txtcredito.Text, out decimal limite))
                        cmd.Parameters.AddWithValue("@LimiteCredito", limite);
                    else
                        cmd.Parameters.AddWithValue("@LimiteCredito", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }

                CargarDatos();
                LimpiarCampos();

                MessageBox.Show(esActualizacion ? "Cliente actualizado correctamente" : "Cliente guardado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar o actualizar el cliente:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        

        private int? ObtenerIdClientePorDocumento(string noDocumento)
        {
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
            using (SqlCommand cmd = new SqlCommand("SELECT idcliente FROM clientes WHERE nodocumento = @NoDocumento", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@NoDocumento", noDocumento);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : (int?)null;
            }
        }








        private void LimpiarCampos()
        {
           
            txtndocumento.Clear();
            txtnombre.Clear();
            txtrazon.Clear();
            txtgirocliente.Clear();
            txttelf.Clear();
            txtcorreo.Clear();
            txtdireccion.Clear();
            txtcredito.Clear();
            estaEditandoCliente = false;

            
            btntipocliente.SelectedIndex = -1;
            btntipodocumento.SelectedIndex = -1;
            cbxprovincia.SelectedIndex = -1;

           
            txtndocumento.Tag = null;
            btntipodocumento.Focus();
        }



        private void CargarDatos()
        {


         
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT 
                c.idcliente AS IdCliente, 
                c.idtipocliente AS IdTipoCliente, 
                c.idtipodocumento AS IdTipoDocumento, 
                c.nodocumento AS NoDocumento, 
                c.nombre AS Nombre,
                c.estado AS Estado,
                c.razonsocial AS RazonSocial, 
                c.girocliente AS GiroCliente, 
                c.telefono AS Telefono, 
                c.correo AS Correo, 
                c.idprovincia AS IdProvincia,
                c.direccion AS Direccion, 
                c.limitecredito AS LimiteCredito,
                c.fecha_creacion AS FechaCreacion,
                tc.nombre AS TipoClienteNombre,
                td.descripcion AS TipoDocumentoNombre,
                p.nombre AS ProvinciaNombre
            FROM clientes c
            INNER JOIN tipo_cliente tc ON c.idtipocliente = tc.idtipo
            INNER JOIN tipo_documento td ON c.idtipodocumento = td.idtipo
            INNER JOIN provincia p ON c.idprovincia = p.idprovincia
            WHERE c.estado = 1";

                SqlDataAdapter da = new SqlDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                
                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = dt;

               
                dataGridView1.DataSource = bindingSource;

                


               
                dataGridView1.Columns["IdCliente"].Visible = false;
                dataGridView1.Columns["IdTipoCliente"].Visible = false;
                dataGridView1.Columns["IdTipoDocumento"].Visible = false;
                dataGridView1.Columns["IdProvincia"].Visible = false;
                

                dataGridView1.Refresh();

               
               
                     
                if (dataGridView1.Columns.Contains("TipoClienteNombre"))
                {
                    dataGridView1.Columns["TipoClienteNombre"].Width = 150; 
                }


               
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }
            }
        }

        private void CargarClientesEnGrid()
        {
            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM clientes", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

              
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.Visible = true;
                }

                dataGridView1.AutoResizeColumns();
            }
        }


        private void EliminarCliente(int id)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                 UPDATE clientes
                 SET estado = 0
                 WHERE idcliente = @IdCliente", connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", id);
                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Cliente eliminado (estado = 0)", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el cliente o ya estaba eliminado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            string[] columnasOrdenadas = {
                "TipoClienteNombre",
                "TipoDocumentoNombre",
                "NoDocumento",
                "Nombre",
                "RazonSocial",
                "GiroCliente",
                "Telefono",
                "Correo",
                "ProvinciaNombre",
                "Direccion",
                "LimiteCredito",
                "Estado",
                "FechaCreacion"
            };

            for (int i = 0; i < columnasOrdenadas.Length; i++)
            {
                if (dataGridView1.Columns.Contains(columnasOrdenadas[i]))
                {
                    dataGridView1.Columns[columnasOrdenadas[i]].DisplayIndex = i;
                }
            }
        }
        private void MantenimientoCliente_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                 System.Reflection.BindingFlags.NonPublic |
                 System.Reflection.BindingFlags.Instance |
                 System.Reflection.BindingFlags.SetProperty,
                 null, dataGridView1, new object[] { true });


        }

        private void btnagregar_Click(object sender, EventArgs e)
        {
           
        }

        private void btntipocliente_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["IdCliente"].Value);
                string nombre = dataGridView1.CurrentRow.Cells["Nombre"].Value.ToString();

                DialogResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas desactivar este registro del cliente: {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    EliminarCliente(id);
                    CargarDatos();
                    LimpiarCampos();
                   
                    btnborrar.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Seleccione un cliente primero.");
            }
        }

        private bool estaEditandoCliente = false;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
                estaEditandoCliente = true;
                txtndocumento.Text = fila.Cells["NoDocumento"].Value?.ToString();
                txtndocumento.Tag = Convert.ToInt32(fila.Cells["idcliente"].Value);
                txtnombre.Text = fila.Cells["Nombre"].Value?.ToString();
                txtrazon.Text = fila.Cells["RazonSocial"].Value?.ToString();
                txtgirocliente.Text = fila.Cells["GiroCliente"].Value?.ToString();
                txttelf.Text = fila.Cells["Telefono"].Value?.ToString();
                txtcorreo.Text = fila.Cells["Correo"].Value?.ToString();
                txtdireccion.Text = fila.Cells["Direccion"].Value?.ToString();

               
                txtcredito.Text = fila.Cells["LimiteCredito"].Value != DBNull.Value
                    ? fila.Cells["LimiteCredito"].Value.ToString()
                    : "";

              
                if (fila.Cells["IdTipoCliente"].Value != DBNull.Value)
                    btntipocliente.SelectedValue = fila.Cells["IdTipoCliente"].Value;

                if (fila.Cells["IdTipoDocumento"].Value != DBNull.Value)
                    btntipodocumento.SelectedValue = fila.Cells["IdTipoDocumento"].Value;

                if (fila.Cells["IdProvincia"].Value != DBNull.Value)
                    cbxprovincia.SelectedValue = fila.Cells["IdProvincia"].Value;
                btnborrar.Enabled = true;
            }
        }
        private bool ValidarCamposCliente()
        {
           
            if (btntipocliente.SelectedIndex == -1)
            {
                MessageBox.Show("El tipo de cliente es requerido.");
                return false;
            }

           
            if (btntipodocumento.SelectedIndex == -1)
            {
                MessageBox.Show("El tipo de documento es requerido.");
                return false;
            }

            
            string noDocumento = txtndocumento.Text.Trim();
            if (string.IsNullOrWhiteSpace(noDocumento))
            {
                MessageBox.Show("El número de documento es requerido.");
                return false;
            }
            if (noDocumento.Length > 20)
            {
                MessageBox.Show("El número de documento no puede exceder 20 caracteres.");
                return false;
            }
            if (!Regex.IsMatch(noDocumento, @"^\d+$"))
            {
                MessageBox.Show("El número de documento debe contener solo números.");
                return false;
            }

            
            string nombre = txtnombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre es requerido.");
                return false;
            }
            if (nombre.Length > 100)
            {
                MessageBox.Show("El nombre no puede exceder 100 caracteres.");
                return false;
            }

           
            string razon = txtrazon.Text.Trim();
            if (string.IsNullOrWhiteSpace(razon))
            {
                MessageBox.Show("La razón social es requerida.");
                return false;
            }
            if (razon.Length > 100)
            {
                MessageBox.Show("La razón social no puede exceder 100 caracteres.");
                return false;
            }

           
            string giro = txtgirocliente.Text.Trim();
            if (string.IsNullOrWhiteSpace(giro))
            {
                MessageBox.Show("El giro del cliente es requerido.");
                return false;
            }
            if (giro.Length > 100)
            {
                MessageBox.Show("El giro del cliente no puede exceder 100 caracteres.");
                return false;
            }

            
            string telefono = txttelf.Text.Trim();
            if (string.IsNullOrWhiteSpace(telefono))
            {
                MessageBox.Show("El teléfono es requerido.");
                return false;
            }
            if (telefono.Length < 10 || telefono.Length > 20)
            {
                MessageBox.Show("El teléfono debe tener entre 10 y 20 caracteres.");
                return false;
            }

           
            string correo = txtcorreo.Text.Trim();
            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show("El correo electrónico es requerido.");
                return false;
            }
            if (correo.Length > 100)
            {
                MessageBox.Show("El correo electrónico no puede exceder 100 caracteres.");
                return false;
            }
            if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El correo electrónico no es válido.");
                return false;
            }

           
            if (cbxprovincia.SelectedIndex == -1)
            {
                MessageBox.Show("La provincia es requerida.");
                return false;
            }

            
            string direccion = txtdireccion.Text.Trim();
            if (string.IsNullOrWhiteSpace(direccion))
            {
                MessageBox.Show("La dirección es requerida.");
                return false;
            }
            if (direccion.Length > 150)
            {
                MessageBox.Show("La dirección no puede exceder 150 caracteres.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtcredito.Text))
            {
                if (!decimal.TryParse(txtcredito.Text.Trim(), out decimal limite) || limite < 0)
                {
                    MessageBox.Show("El límite de crédito debe ser un número positivo.");
                    return false;
                }
            }

            return true;
        }

        private void txtndocumento_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtndocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txttelf_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }

        private void txtcredito_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }

        private void txtnombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            CargarDatos();
        }
    }
}
