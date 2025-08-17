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
    public partial class ConsultaClientes : Form
    {



        string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

        // Variables para identificar cliente seleccionado
        public int? ClienteIdSeleccionado { get; private set; }
        public string NumDocumentoSeleccionado { get; private set; }
        public string NombreSeleccionado { get; private set; }

        public ConsultaClientes()
        {
            InitializeComponent();
            CargarDatos();

            cbxcampo.SelectedIndex = -1;
        }

        //metodo que carga los datros al datagriview desde las base de datos
        private void CargarDatos()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
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

                    BindingSource bs = new BindingSource();
                    bs.DataSource = dt;
                    dataGridView1.DataSource = bs;

                    // Ocultar columnas ID internas
                    dataGridView1.Columns["IdCliente"].Visible = false;
                    dataGridView1.Columns["IdTipoCliente"].Visible = false;
                    dataGridView1.Columns["IdTipoDocumento"].Visible = false;
                    dataGridView1.Columns["IdProvincia"].Visible = false;
                    

                    // Ajustar ancho de columna TipoClienteNombre si existe
                    if (dataGridView1.Columns.Contains("TipoClienteNombre"))
                        dataGridView1.Columns["TipoClienteNombre"].Width = 150;

                    // Activar filtro en encabezados (usando DataGridViewAutoFilter)
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los clientes: " + ex.Message);
                }
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnmostrar_Click(object sender, EventArgs e)
        {
            // Limpia la búsqueda
            txtbuscar.Text = "";
            // Opcional: resetear selección en combo
            cbxcampo.SelectedIndex = 0;
            // Recarga los datos
            CargarDatos();
            cbxcampo.SelectedIndex = -1;
            dataGridView1.ClearSelection();
        }

        private void ConsultaClientes_Load(object sender, EventArgs e)
        {
            
            typeof(DataGridView).InvokeMember("DoubleBuffered",
        System.Reflection.BindingFlags.NonPublic |
        System.Reflection.BindingFlags.Instance |
        System.Reflection.BindingFlags.SetProperty,
        null, dataGridView1, new object[] { true });

           
            //aqui se cargan los datos al combobox de el datagriview para el filtrado

            cbxcampo.Items.Clear();

            cbxcampo.Items.Add("tipoclientenombre");
            cbxcampo.Items.Add("tipodocumentonombre");
            cbxcampo.Items.Add("nodocumento");
            cbxcampo.Items.Add("nombre");
            cbxcampo.Items.Add("razonsocial");
            cbxcampo.Items.Add("girocliente");
            cbxcampo.Items.Add("telefono");
            cbxcampo.Items.Add("correo");
            cbxcampo.Items.Add("provincianombre");
            cbxcampo.Items.Add("direccion");
            cbxcampo.Items.Add("limitecredito");
            cbxcampo.Items.Add("fecha_creacion");

            cbxcampo.SelectedItem = "nombre";
             // No seleccionar nada al inicio
            dataGridView1.ClearSelection();
        }

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            //campo de el texbox para buscar los datos

            string campoSeleccionado = cbxcampo.SelectedItem?.ToString().ToLower();
            string valorBusqueda = txtbuscar.Text.Trim();

            if (string.IsNullOrEmpty(campoSeleccionado) || string.IsNullOrEmpty(valorBusqueda))
            {
                // Si no hay filtro, cargamos todos
                CargarDatos();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string queryBase = @"
        SELECT 
            c.idcliente AS idcliente, 
            c.idtipocliente AS idtipocliente, 
            c.idtipodocumento AS idtipodocumento, 
            c.nodocumento AS nodocumento, 
            c.nombre AS nombre,
            c.estado AS estado,
            c.razonsocial AS razonsocial, 
            c.girocliente AS girocliente, 
            c.telefono AS telefono, 
            c.correo AS correo, 
            c.idprovincia AS idprovincia,
            c.direccion AS direccion, 
            c.limitecredito AS limitecredito,
            c.fecha_creacion AS fecha_creacion,
            tc.nombre AS tipoclientenombre,
            td.descripcion AS tipodocumentonombre,
            p.nombre AS provincianombre
        FROM clientes c
        INNER JOIN tipo_cliente tc ON c.idtipocliente = tc.idtipo
        INNER JOIN tipo_documento td ON c.idtipodocumento = td.idtipo
        INNER JOIN provincia p ON c.idprovincia = p.idprovincia
        WHERE c.estado = 1 AND ";

                    string filtro = "";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (campoSeleccionado == "idcliente")
                    {
                        filtro = "c.idcliente = @valor";
                        if (!int.TryParse(valorBusqueda, out int idCliente))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        cmd.Parameters.AddWithValue("@valor", idCliente);
                    }
                    else if (campoSeleccionado == "fecha_creacion")
                    {
                        filtro = "CONVERT(VARCHAR(10), c.fecha_creacion, 120) LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "estado")
                    {
                        filtro = "c.estado = @valor";
                        if (!int.TryParse(valorBusqueda, out int estadoVal))
                        {
                            dataGridView1.DataSource = null;
                            return;
                        }
                        cmd.Parameters.AddWithValue("@valor", estadoVal);
                    }
                    else if (campoSeleccionado == "tipoclientenombre")
                    {
                        filtro = "tc.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "tipodocumentonombre")
                    {
                        filtro = "td.descripcion LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "provincianombre")
                    {
                        filtro = "p.nombre LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else if (campoSeleccionado == "nodocumento")
                    {
                        filtro = "LTRIM(RTRIM(c.nodocumento)) LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }
                    else
                    {
                        filtro = $"c.{campoSeleccionado} LIKE @valor";
                        cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                    }

                    cmd.CommandText = queryBase + filtro;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable tabla = new DataTable();
                    adapter.Fill(tabla);

                    BindingSource bs = new BindingSource();
                    bs.DataSource = tabla;
                    dataGridView1.DataSource = bs;

                    // Limpiar selección para evitar selección errónea
                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;

                    // Ocultar columnas internas
                    dataGridView1.Columns["idcliente"].Visible = false;
                    dataGridView1.Columns["idtipocliente"].Visible = false;
                    dataGridView1.Columns["idtipodocumento"].Visible = false;
                    dataGridView1.Columns["idprovincia"].Visible = false;

                    // Ajustar ancho columna TipoClienteNombre si existe
                    if (dataGridView1.Columns.Contains("tipoclientenombre"))
                        dataGridView1.Columns["tipoclientenombre"].Width = 150;

                    // Activar filtros en encabezados
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                        col.HeaderCell = new DataGridViewAutoFilter.DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);

                    // Asegurar que al terminar de enlazar datos no quede selección
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error en la búsqueda: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            ClienteIdSeleccionado = row.Cells["IdCliente"].Value != DBNull.Value
                ? Convert.ToInt32(row.Cells["IdCliente"].Value)
                : (int?)null;

            NumDocumentoSeleccionado = row.Cells["NoDocumento"].Value?.ToString() ?? "";
            NombreSeleccionado = row.Cells["Nombre"].Value?.ToString() ?? "";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtbuscar_KeyDown(object sender, KeyEventArgs e)
        {
            //esto deja un mensaje cuando se da enter y se deja vacio algun campo
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
                        c.idcliente AS idcliente, 
                        c.idtipocliente AS idtipocliente, 
                        c.idtipodocumento AS idtipodocumento, 
                        c.nodocumento AS nodocumento, 
                        c.nombre AS nombre,
                        c.estado AS estado,
                        c.razonsocial AS razonsocial, 
                        c.girocliente AS girocliente, 
                        c.telefono AS telefono, 
                        c.correo AS correo, 
                        c.idprovincia AS idprovincia,
                        c.direccion AS direccion, 
                        c.limitecredito AS limitecredito,
                        c.fecha_creacion AS fecha_creacion,
                        tc.nombre AS tipoclientenombre,
                        td.descripcion AS tipodocumentonombre,
                        p.nombre AS provincianombre
                    FROM clientes c
                    INNER JOIN tipo_cliente tc ON c.idtipocliente = tc.idtipo
                    INNER JOIN tipo_documento td ON c.idtipodocumento = td.idtipo
                    INNER JOIN provincia p ON c.idprovincia = p.idprovincia
                    WHERE c.estado = 1 AND ";

                        string filtro = "";
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;

                        if (campoSeleccionado == "idcliente")
                        {
                            filtro = "c.idcliente = @valor";
                            if (!int.TryParse(valorBusqueda, out int idCliente))
                            {
                                dataGridView1.DataSource = null;
                                return;
                            }
                            cmd.Parameters.AddWithValue("@valor", idCliente);
                        }
                        else if (campoSeleccionado == "fecha_creacion")
                        {
                            filtro = "CONVERT(VARCHAR(10), c.fecha_creacion, 120) LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else if (campoSeleccionado == "tipoclientenombre")
                        {
                            filtro = "tc.nombre LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else if (campoSeleccionado == "tipodocumentonombre")
                        {
                            filtro = "td.descripcion LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else if (campoSeleccionado == "provincianombre")
                        {
                            filtro = "p.nombre LIKE @valor";
                            cmd.Parameters.AddWithValue("@valor", "%" + valorBusqueda + "%");
                        }
                        else
                        {
                            filtro = $"c.{campoSeleccionado} LIKE @valor";
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
                        dataGridView1.Columns["idcliente"].Visible = false;
                        dataGridView1.Columns["idtipocliente"].Visible = false;
                        dataGridView1.Columns["idtipodocumento"].Visible = false;
                        dataGridView1.Columns["idprovincia"].Visible = false;

                        // Ajustar ancho para nombre tipo cliente
                        if (dataGridView1.Columns.Contains("tipoclientenombre"))
                            dataGridView1.Columns["tipoclientenombre"].Width = 150;

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

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //esto es para cargar el orden de los datos en el datagriview
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
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
    }
}

