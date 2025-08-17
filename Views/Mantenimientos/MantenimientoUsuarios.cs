using Restaurante.model;
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
    public partial class MantenimientoUsuarios : Form
    {

        SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;");
        public MantenimientoUsuarios()
        {
            InitializeComponent();
            CargarTipoUsuarios();
            CargarDatos();
            CargarSucursales();


            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;


        }






        private void CargarDatos()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand())
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    connection.Open();
                    command.Connection = connection;

                    // Consulta original usando u.* y joins
                    command.CommandText = @"
                SELECT u.*, s.nombre AS NombreSucursal, tu.descripcion AS TipoUsuario, sx.descripcion AS Sexo
                FROM usuarios u
                INNER JOIN sucursal s ON u.idsucursal = s.idsucursal
                INNER JOIN tipo_usuario tu ON u.idtipo = tu.idtipo
                INNER JOIN sexo sx ON u.idsexo = sx.idsexo";

                    adapter.SelectCommand = command;
                    adapter.Fill(dt);
                }

                // BindingSource local
                BindingSource usuarioBindingSource = new BindingSource();
                usuarioBindingSource.DataSource = dt;
                dataGridView1.DataSource = usuarioBindingSource;

                // Ocultar columnas de ID y contraseña
                string[] columnasOcultas = { "idusuario", "idsexo", "idtipo", "idsucursal", "contrasena" };
                foreach (var col in columnasOcultas)
                {
                    if (dataGridView1.Columns.Contains(col))
                        dataGridView1.Columns[col].Visible = false;
                }

                // Convertir columna estatus a checkbox
                if (dataGridView1.Columns.Contains("estatus"))
                {
                    int colIndex = dataGridView1.Columns["estatus"].Index;
                    DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn
                    {
                        Name = "Estatus",
                        HeaderText = "Estado",
                        DataPropertyName = "estatus",
                        TrueValue = true,
                        FalseValue = false
                    };
                    dataGridView1.Columns.RemoveAt(colIndex);
                    dataGridView1.Columns.Insert(colIndex, chkCol);
                }

                // Activar filtros de columna
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }

                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.ScrollBars = ScrollBars.Both;

                // Enlazar DataBindingComplete para aplicar orden de columnas y ancho fijo
                dataGridView1.DataBindingComplete -= dataGridView1_DataBindingComplete; // evitar duplicados
                dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;

                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MantenimientoUsuarios_Load(object sender, EventArgs e)
        {


            if (dataGridView1.Columns.Contains("Status"))
                dataGridView1.Columns["Status"].Visible = false;

            if (dataGridView1.Columns.Contains("IdNivelAcceso"))
                dataGridView1.Columns["IdNivelAcceso"].Visible = false;

            if (dataGridView1.Columns.Contains("NombreUsuario"))
                dataGridView1.Columns["NombreUsuario"].Visible = false;

            if (dataGridView1.Columns.Contains("Comisiones"))
                dataGridView1.Columns["Comisiones"].Visible = false;


            cbxstatus.DataSource = new List<object>
            {
                new { Text = "Activo",   Value = true },
                new { Text = "Inactivo", Value = false }
            };
            cbxstatus.DisplayMember = "Text";
            cbxstatus.ValueMember = "Value";
            cbxstatus.SelectedIndex = -1;


            cbxsexo.DataSource = new List<dynamic>
             {
                  new { Descripcion = "Masculino", Valor = 1 },
                  new { Descripcion = "Femenino",  Valor = 2 }
             };

            cbxsexo.DisplayMember = "Descripcion";
            cbxsexo.ValueMember = "Valor";
            cbxsexo.SelectedValue = -1;
        }

        public decimal? Comision
        {
            get
            {
                if (txtcomision.Text.Replace(" ", "") == "")
                    return 0.0m;

                if (decimal.TryParse(txtcomision.Text, out decimal result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value == null || value == -1.0m)
                {
                    txtcomision.Text = "";
                }
                else
                {
                    txtcomision.Text = value.ToString();
                }
            }
        }


        public bool? Estatus
        {
            get
            {
                // Si no hay selección, devuelve null
                if (cbxstatus.SelectedIndex == -1)
                    return null;

                // Si el índice es 0, devuelve true; si es 1, devuelve false
                return cbxstatus.SelectedIndex == 0;
            }
            set
            {
                if (value == null)
                {
                    // Deja el combo sin seleccionar
                    cbxstatus.SelectedIndex = -1;
                }
                else
                {
                    // Si el valor es true → índice 0; si es false → índice 1
                    cbxstatus.SelectedIndex = (bool)value ? 0 : 1;
                }
            }
        }

        public int? IdSexo
        {
            get
            {
                if (cbxsexo.SelectedItem?.ToString() == null)
                    return null;

                return cbxsexo.SelectedIndex == 0 ? 1 : 2;

            }
            set { int? selected = value; cbxsexo.SelectedIndex = selected - 1 ?? -1; }
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
        private void CargarTipoUsuarios()
        {
            try
            {
                con.Open(); // asumo que 'con' es tu SqlConnection ya declarada y configurada
                SqlCommand cmd = new SqlCommand("SELECT idtipo, descripcion FROM tipo_usuario", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbxnivelacceso.DataSource = dt;
                cbxnivelacceso.DisplayMember = "descripcion";
                cbxnivelacceso.ValueMember = "idtipo";
                cbxnivelacceso.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tipos de usuario: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }


        private void GuardarSexoUsuario()
        {
            if (cbxsexo.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un sexo antes de continuar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idSexo = (int)cbxsexo.SelectedValue;

            using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
            {
                con.Open();
                string query = "INSERT INTO usuarios (idsexo) VALUES (@idsexo)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@idsexo", idSexo);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Sexo del usuario registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GuardarUsuarioConEstatus()
        {
            if (cbxstatus.SelectedValue != null && cbxstatus.SelectedValue is bool)
            {
                bool estatus = (bool)cbxstatus.SelectedValue;


                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();

                    string query = "INSERT INTO usuarios (estatus) VALUES (@estatus)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@estatus", estatus);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuario guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione un estado válido.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnventana_Click(object sender, EventArgs e)
        {

        }

        private void btnpanel_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            // Guardar el idusuario en Tag para poder actualizar
            txtndocumentos.Tag = row.Cells["idusuario"].Value;

            // Llenar TextBox con los datos de la fila
            txtndocumentos.Text = row.Cells["no_documento"].Value.ToString();
            txtnusuario.Text = row.Cells["usuario"].Value.ToString();
            txtcontrasena.Text = row.Cells["contrasena"].Value.ToString();
            txtnombre.Text = row.Cells["nombre"].Value.ToString();
            txtcorreo.Text = row.Cells["correo"].Value.ToString();
            txtdireccion.Text = row.Cells["direccion"].Value.ToString();
            txtntelf.Text = row.Cells["telefono"].Value.ToString();
            txtcomision.Text = row.Cells["comision"].Value != DBNull.Value ? row.Cells["comision"].Value.ToString() : "";

            cbxsexo.SelectedValue = row.Cells["idsexo"].Value;
            cbxnivelacceso.SelectedValue = row.Cells["idtipo"].Value;
            cbxsucursal.SelectedValue = row.Cells["idsucursal"].Value;
            cbxstatus.SelectedIndex = row.Cells["estatus"].Value != DBNull.Value && (bool)row.Cells["estatus"].Value ? 0 : 1;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            string[] ordenColumnas = {
              "no_documento",
              "nombre",
              "Sexo",
              "direccion",
              "telefono",
              "correo",
              "usuario",
             "TipoUsuario",
             "Estatus",
             "comision",
             "NombreSucursal"
            };

            for (int i = 0; i < ordenColumnas.Length; i++)
            {
                if (dataGridView1.Columns.Contains(ordenColumnas[i]))
                    dataGridView1.Columns[ordenColumnas[i]].DisplayIndex = i;
            }

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Asignar un ancho fijo a todas las columnas
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.Width = 150; // puedes cambiar 150 por el tamaño que quieras
            }
        }

        private void LimpiarCamposUsuario()
        {
            idUsuarioActual = null;
            estaEditandoUsuario = false;

            txtndocumentos.Clear();
            txtnusuario.Clear();
            txtcontrasena.Clear();
            txtntelf.Clear();
            txtnombre.Clear();
            txtcorreo.Clear();
            cbxsexo.SelectedIndex = -1;
            cbxnivelacceso.SelectedIndex = -1;
            txtdireccion.Clear();
            cbxsucursal.SelectedIndex = -1;
            cbxstatus.SelectedIndex = 0;
            txtcomision.Text = "0";
        }


        private int? idUsuarioActual = null;
        private bool estaEditandoUsuario = false;
        private int? idUsuario = null;
        private void btnguardar_Click(object sender, EventArgs e)
        {
           
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
            if (!ValidarCamposCompletos())
                return;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                int idUsuario = txtndocumentos.Tag == null ? 0 : Convert.ToInt32(txtndocumentos.Tag);

                // Verificar duplicado por usuario (ignorando el mismo idUsuario)
                string checkUsuario = "SELECT COUNT(*) FROM usuarios WHERE usuario = @Usuario AND idusuario <> @IdUsuario";
                using (SqlCommand cmdCheckUsuario = new SqlCommand(checkUsuario, con))
                {
                    cmdCheckUsuario.Parameters.AddWithValue("@Usuario", txtnusuario.Text.Trim());
                    cmdCheckUsuario.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    if ((int)cmdCheckUsuario.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("El nombre de usuario ya existe. Por favor, elige otro.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Verificar duplicado por número de documento (ignorando el mismo idUsuario)
                string checkDocumento = "SELECT COUNT(*) FROM usuarios WHERE no_documento = @NoDocumento AND idusuario <> @IdUsuario";
                using (SqlCommand cmdCheckDoc = new SqlCommand(checkDocumento, con))
                {
                    cmdCheckDoc.Parameters.AddWithValue("@NoDocumento", txtndocumentos.Text.Trim());
                    cmdCheckDoc.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    if ((int)cmdCheckDoc.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Ya existe un usuario con ese número de documento.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                SqlCommand cmd;
                if (idUsuario == 0)
                {
                    // INSERTAR
                    cmd = new SqlCommand(@"
                INSERT INTO usuarios 
                (no_documento, usuario, contrasena, telefono, nombre, correo, idsexo, idtipo, direccion, idsucursal, estatus, comision, fecha_creacion)
                VALUES 
                (@NoDocumento, @Usuario, @Contrasena, @Telefono, @Nombre, @Correo, @IdSexo, @IdTipo, @Direccion, @IdSucursal, @Estatus, @Comision, GETDATE())", con);
                }
                else
                {
                    // ACTUALIZAR
                    cmd = new SqlCommand(@"
                UPDATE usuarios
                SET no_documento = @NoDocumento,
                    usuario = @Usuario,
                    contrasena = @Contrasena,
                    telefono = @Telefono,
                    nombre = @Nombre,
                    correo = @Correo,
                    idsexo = @IdSexo,
                    idtipo = @IdTipo,
                    direccion = @Direccion,
                    idsucursal = @IdSucursal,
                    estatus = @Estatus,
                    comision = @Comision
                WHERE idusuario = @IdUsuario", con);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                }

                // Parámetros
                cmd.Parameters.AddWithValue("@NoDocumento", txtndocumentos.Text.Trim());
                cmd.Parameters.AddWithValue("@Usuario", txtnusuario.Text.Trim());
                cmd.Parameters.AddWithValue("@Contrasena", txtcontrasena.Text.Trim());
                cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(txtntelf.Text) ? (object)DBNull.Value : txtntelf.Text.Trim());
                cmd.Parameters.AddWithValue("@Nombre", txtnombre.Text.Trim());
                cmd.Parameters.AddWithValue("@Correo", string.IsNullOrWhiteSpace(txtcorreo.Text) ? (object)DBNull.Value : txtcorreo.Text.Trim());
                cmd.Parameters.AddWithValue("@IdSexo", Convert.ToInt32(cbxsexo.SelectedValue));
                cmd.Parameters.AddWithValue("@IdTipo", Convert.ToInt32(cbxnivelacceso.SelectedValue));
                cmd.Parameters.AddWithValue("@Direccion", string.IsNullOrWhiteSpace(txtdireccion.Text) ? (object)DBNull.Value : txtdireccion.Text.Trim());
                cmd.Parameters.AddWithValue("@IdSucursal", Convert.ToInt32(cbxsucursal.SelectedValue));
                cmd.Parameters.AddWithValue("@Estatus", cbxstatus.SelectedIndex == 0);
                cmd.Parameters.AddWithValue("@Comision", string.IsNullOrWhiteSpace(txtcomision.Text) ? 0m : Convert.ToDecimal(txtcomision.Text));

                cmd.ExecuteNonQuery();

                MessageBox.Show(idUsuario == 0 ? "Usuario guardado correctamente." : "Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar campos y reset Tag
                txtndocumentos.Tag = null;
                txtndocumentos.Clear();
                txtnusuario.Clear();
                txtcontrasena.Clear();
                txtntelf.Clear();
                txtnombre.Clear();
                txtcorreo.Clear();
                txtdireccion.Clear();
                txtcomision.Clear();
                cbxsexo.SelectedIndex = -1;
                cbxnivelacceso.SelectedIndex = -1;
                cbxsucursal.SelectedIndex = -1;
                cbxstatus.SelectedIndex = -1;

              

                // Recargar DataGridView
                CargarDatos();
            }
        }


        private bool ValidarCamposCompletos()
        {
            // Campos obligatorios
            if (string.IsNullOrWhiteSpace(txtndocumentos.Text))
            {
                MessageBox.Show("Debes llenar el número de documento.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtndocumentos.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtnusuario.Text))
            {
                MessageBox.Show("Debes llenar el nombre de usuario.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtnusuario.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtcontrasena.Text))
            {
                MessageBox.Show("Debes llenar la contraseña.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtcontrasena.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtnombre.Text))
            {
                MessageBox.Show("Debes llenar el nombre.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtnombre.Focus();
                return false;
            }

            if (cbxsexo.SelectedIndex < 0)
            {
                MessageBox.Show("Debes seleccionar el sexo.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxsexo.Focus();
                return false;
            }

            if (cbxnivelacceso.SelectedIndex < 0)
            {
                MessageBox.Show("Debes seleccionar el nivel de acceso.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxnivelacceso.Focus();
                return false;
            }

            if (cbxsucursal.SelectedIndex < 0)
            {
                MessageBox.Show("Debes seleccionar la sucursal.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxsucursal.Focus();
                return false;
            }

            // Campos opcionales con validación de formato
            if (!string.IsNullOrWhiteSpace(txtcomision.Text))
            {
                if (!decimal.TryParse(txtcomision.Text.Trim(), out _))
                {
                    MessageBox.Show("La comisión debe ser un valor numérico.", "Formato incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtcomision.Focus();
                    return false;
                }
            }

            return true; // Todos los campos obligatorios y formatos son correctos
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            // Limpiar campos del formulario
            txtndocumentos.Tag = null;
            txtndocumentos.Clear();
            txtnusuario.Clear();
            txtcontrasena.Clear();
            txtntelf.Clear();
            txtnombre.Clear();
            txtcorreo.Clear();
            txtdireccion.Clear();
            txtcomision.Clear();
            cbxsexo.SelectedIndex = -1;
            cbxnivelacceso.SelectedIndex = -1;
            cbxsucursal.SelectedIndex = -1;
            cbxstatus.SelectedIndex = -1;

          

            // Opcional: resetear selección
            dataGridView1.ClearSelection();
        }
    }
}
