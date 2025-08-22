using Restaurante.model;
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
using Restaurante.Views;


namespace Restaurante.Views.Ventanas
{
    public partial class VentanaUsuario : Form
    {
        internal UsuarioModel UsuarioSeleccionado { get; private set; }

        public VentanaUsuario()
        {
            InitializeComponent();
            CargarDatos();
        }


        private void CargarDatos()
        {
            {
                string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

                var usuarioList = new List<UsuarioModel>();

                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = @"
        SELECT u.*, s.nombre AS NombreSucursal, tu.descripcion AS TipoUsuario, sx.descripcion AS Sexo
        FROM usuarios u
        INNER JOIN sucursal s ON u.idsucursal = s.idsucursal
        INNER JOIN tipo_usuario tu ON u.idtipo = tu.idtipo
        INNER JOIN sexo sx ON u.idsexo = sx.idsexo";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuario = new UsuarioModel();
                            usuario.IdUsuario = (int)reader["idusuario"];
                            usuario.NoDocumento = reader["no_documento"].ToString();
                            usuario.Usuario = reader["usuario"].ToString();
                            usuario.Contrasena = reader["contrasena"].ToString();
                            usuario.Telefono = reader["telefono"].ToString();
                            usuario.IdTipo = (int)reader["idtipo"];
                            usuario.Nombre = reader["nombre"].ToString();
                            usuario.Correo = reader["correo"].ToString();
                            usuario.IdSexo = (int)reader["idsexo"];
                            usuario.Direccion = reader["direccion"].ToString();
                            usuario.IdSucursal = (int)reader["idsucursal"];
                            usuario.Comision = reader["comision"] != DBNull.Value ? (decimal)reader["comision"] : 0;
                            usuario.Estatus = (bool)reader["estatus"];
                            usuario.FechaCreacion = (DateTime)reader["fecha_creacion"];
                            usuario.Sucursal = reader["NombreSucursal"].ToString();
                            usuario.TipoUsuario = reader["TipoUsuario"].ToString();
                            usuario.Sexo = reader["Sexo"].ToString();

                           
                            usuarioList.Add(usuario);
                        }
                    }
                }

                
                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = usuarioList;
                dataGridView1.DataSource = bindingSource;


                if (dataGridView1.Columns.Contains("IdUsuario"))
                    dataGridView1.Columns["IdUsuario"].Visible = false;

                if (dataGridView1.Columns.Contains("Contrasena"))
                    dataGridView1.Columns["Contrasena"].Visible = false;

                if (dataGridView1.Columns.Contains("Estatus"))
                {
                    int colIndex = dataGridView1.Columns["Estatus"].Index;
                    DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
                    chkCol.Name = "Estatus";
                    chkCol.HeaderText = "Estado";
                    chkCol.DataPropertyName = "Estatus";
                    chkCol.TrueValue = true;
                    chkCol.FalseValue = false;

                    dataGridView1.Columns.RemoveAt(colIndex);
                    dataGridView1.Columns.Insert(colIndex, chkCol);
                }



                dataGridView1.Refresh();

               
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 
                dataGridView1.ScrollBars = ScrollBars.Both;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

               
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }
            }
        }
        private void VentanaUsuario_Load(object sender, EventArgs e)
        {
           

          
            if (dataGridView1.Columns.Contains("Status"))
                dataGridView1.Columns["Status"].Visible = false;

            if (dataGridView1.Columns.Contains("IdNivelAcceso"))
                dataGridView1.Columns["IdNivelAcceso"].Visible = false;

            if (dataGridView1.Columns.Contains("NombreUsuario"))
                dataGridView1.Columns["NombreUsuario"].Visible = false;

            if (dataGridView1.Columns.Contains("Comisiones"))
                dataGridView1.Columns["Comisiones"].Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                int idUsuario = dataGridView1.Columns.Contains("IdUsuario") && fila.Cells["IdUsuario"].Value != DBNull.Value
                    ? Convert.ToInt32(fila.Cells["IdUsuario"].Value)
                    : 0;

                string noDocumento = dataGridView1.Columns.Contains("NoDocumento") ? fila.Cells["NoDocumento"].Value?.ToString() : null;
                string nombre = dataGridView1.Columns.Contains("Nombre") ? fila.Cells["Nombre"].Value?.ToString() : null;
                string sexo = dataGridView1.Columns.Contains("Sexo") ? fila.Cells["Sexo"].Value?.ToString() : null;
                string direccion = dataGridView1.Columns.Contains("Direccion") ? fila.Cells["Direccion"].Value?.ToString() : null;
                string telefono = dataGridView1.Columns.Contains("Telefono") ? fila.Cells["Telefono"].Value?.ToString() : null;
                string correo = dataGridView1.Columns.Contains("Correo") ? fila.Cells["Correo"].Value?.ToString() : null;
                string usuarioSistema = dataGridView1.Columns.Contains("usuario") ? fila.Cells["usuario"].Value?.ToString() : null;
                string contrasena = dataGridView1.Columns.Contains("Contrasena") ? fila.Cells["Contrasena"].Value?.ToString() : null;

                int? idNivelAcceso = null;
                if (dataGridView1.Columns.Contains("IdNivelAcceso") && fila.Cells["IdNivelAcceso"].Value != DBNull.Value)
                    idNivelAcceso = Convert.ToInt32(fila.Cells["IdNivelAcceso"].Value);

                bool? estatus = null;
                if (dataGridView1.Columns.Contains("Status") && fila.Cells["Status"].Value != DBNull.Value)
                    estatus = Convert.ToBoolean(fila.Cells["Status"].Value);

                decimal? comision = null;
                if (dataGridView1.Columns.Contains("Comisiones") && fila.Cells["Comisiones"].Value != DBNull.Value)
                    comision = Convert.ToDecimal(fila.Cells["Comisiones"].Value);

                int? idSucursal = null;
                if (dataGridView1.Columns.Contains("IdSucursal") && fila.Cells["IdSucursal"].Value != DBNull.Value)
                    idSucursal = Convert.ToInt32(fila.Cells["IdSucursal"].Value);

                int? idTipo = null;
                if (dataGridView1.Columns.Contains("IdTipo") && fila.Cells["IdTipo"].Value != DBNull.Value)
                    idTipo = Convert.ToInt32(fila.Cells["IdTipo"].Value);

                UsuarioModel usuario = new UsuarioModel
                {
                    IdUsuario = idUsuario,
                    NoDocumento = noDocumento,
                    Nombre = nombre,
                    Sexo = sexo,
                    Direccion = direccion,
                    Telefono = telefono,
                    Correo = correo,
                    Usuario = usuarioSistema,
                    Contrasena = contrasena,
                    IdNivelAcceso = idNivelAcceso,
                    Estatus = estatus,
                    Comision = comision,
                    IdSucursal = idSucursal,
                    IdTipo = idTipo
                };

                UsuarioSeleccionado = usuario;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
    
}
