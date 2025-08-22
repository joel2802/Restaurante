using Restaurante.Views.Ventanas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restaurante.Movimientos
{
    public partial class MovimientoPOS : Form
    {

        private string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";
        public MovimientoPOS()
        {
            InitializeComponent();


        }


        private void CargarCategorias()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT IdCategoria, Nombre FROM Categorias WHERE Estado = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

               
                DataRow dr = dt.NewRow();
                dr["IdCategoria"] = 0;         
                dr["Nombre"] = "Todas";
                dt.Rows.InsertAt(dr, 0);      

                cmbcategoria.DataSource = dt;
                cmbcategoria.DisplayMember = "Nombre";
                cmbcategoria.ValueMember = "IdCategoria";
                cmbcategoria.SelectedIndex = 0; 
            }
        }

        private void CargarSalas()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT IdSala, Nombre FROM Salas WHERE Estado = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                
                DataRow dr = dt.NewRow();
                dr["IdSala"] = 0;
                dr["Nombre"] = "Todas";
                dt.Rows.InsertAt(dr, 0);

                cmbSalas.DataSource = dt;
                cmbSalas.DisplayMember = "Nombre";
                cmbSalas.ValueMember = "IdSala";
                cmbSalas.SelectedIndex = 0; 
            }
        }


     

        public void CargarMesas(int? idSala = null)
        {
            panelMesas.Controls.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
        SELECT m.idmesa, m.nombre, m.cantpersonas, s.nombre AS nombre_sala, m.disponible
        FROM mesas m
        INNER JOIN salas s ON m.idsala = s.idsala
        WHERE m.estado = 1";

                if (idSala != null)
                    query += " AND m.idsala = @IdSala";

                SqlCommand cmd = new SqlCommand(query, con);
                if (idSala != null)
                    cmd.Parameters.AddWithValue("@IdSala", idSala);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Panel panelMesa = new Panel();
                    panelMesa.Width = 120;
                    panelMesa.Height = 100;
                    panelMesa.Margin = new Padding(10);
                    panelMesa.BorderStyle = BorderStyle.FixedSingle;

                    int idMesa = (int)reader["idmesa"];
                    bool disponible = (bool)reader["disponible"];

                    
                    panelMesa.Tag = new { IdMesa = idMesa, Disponible = disponible };

                    
                    panelMesa.BackColor = disponible ? Color.Green : Color.Red;

                    Label lblNombre = new Label();
                    lblNombre.Text = reader["nombre"].ToString();
                    lblNombre.Dock = DockStyle.Top;
                    lblNombre.TextAlign = ContentAlignment.MiddleCenter;
                    lblNombre.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    lblNombre.Height = 30;
                    lblNombre.ForeColor = Color.White;

                    Panel panelPersonas = new Panel();
                    panelPersonas.Dock = DockStyle.Bottom;
                    panelPersonas.Height = 30;
                    panelPersonas.BackColor = Color.White;

                    Label lblPersonas = new Label();
                    lblPersonas.Text = reader["cantpersonas"].ToString() + " Personas";
                    lblPersonas.Dock = DockStyle.Fill;
                    lblPersonas.TextAlign = ContentAlignment.MiddleCenter;
                    lblPersonas.Font = new Font("Segoe UI", 9);
                    lblPersonas.ForeColor = Color.Black;

                    panelPersonas.Controls.Add(lblPersonas);
                    panelMesa.Controls.Add(panelPersonas);
                    panelMesa.Controls.Add(lblNombre);

                    panelMesa.Click += (s, ev) =>
                    {
                        dynamic datos = ((Panel)s).Tag;
                        if (!datos.Disponible) return; 

                        mesaSeleccionada = datos.IdMesa;

                        foreach (Panel p in panelMesas.Controls)
                        {
                            dynamic d = p.Tag;
                            p.BackColor = d.Disponible ? Color.Green : Color.Red;
                        }

             
                ((Panel)s).BackColor = Color.Blue;
                    };

                    panelMesas.Controls.Add(panelMesa);
                }
            }
        }


        private void CargarProductos(int? idCategoria = null)
        {
            panel1Productos.Controls.Clear();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
            SELECT IdProducto, Nombre, PrecioVenta, Existencia AS Stock, RutaFoto
            FROM Productos
            WHERE Estado = 1";

                if (idCategoria != null && idCategoria != 0)
                    query += " AND IdCategoria = @IdCategoria";

                SqlCommand cmd = new SqlCommand(query, con);
                if (idCategoria != null && idCategoria != 0)
                    cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int idProducto = (int)reader["IdProducto"];
                    string nombre = reader["Nombre"].ToString();
                    decimal precio = Convert.ToDecimal(reader["PrecioVenta"]);
                    int stock = Convert.ToInt32(reader["Stock"]);
                    string ruta = reader["RutaFoto"].ToString();

                 
                    Panel panelProducto = new Panel();
                    panelProducto.Width = 120;
                    panelProducto.Height = 160;
                    panelProducto.Margin = new Padding(10);
                    panelProducto.Tag = idProducto;

                  
                    PictureBox pbProducto = new PictureBox();
                    pbProducto.Dock = DockStyle.Top;
                    pbProducto.Height = 100;
                    pbProducto.SizeMode = PictureBoxSizeMode.Zoom;
                    if (!string.IsNullOrEmpty(ruta) && File.Exists(ruta))
                        pbProducto.Image = Image.FromFile(ruta);

                    Label lblNombre = new Label();
                    lblNombre.Text = nombre;
                    lblNombre.Dock = DockStyle.Top;
                    lblNombre.TextAlign = ContentAlignment.MiddleCenter;
                    lblNombre.Height = 25;
                    lblNombre.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                 
                    Label lblPrecio = new Label();
                    lblPrecio.Text = "RD$ " + precio.ToString("N2");
                    lblPrecio.Dock = DockStyle.Top;
                    lblPrecio.TextAlign = ContentAlignment.MiddleCenter;
                    lblPrecio.Height = 20;
                    lblPrecio.Font = new Font("Segoe UI", 9, FontStyle.Regular);

                  
                    panelProducto.Controls.Add(lblPrecio);
                    panelProducto.Controls.Add(lblNombre);
                    panelProducto.Controls.Add(pbProducto);

                 
                    void AgregarAlCarrito(object sender, EventArgs e)
                    {
                        AgregarProductoAlCarrito(idProducto, nombre, precio, stock);
                    }

                  
                    panelProducto.Click += AgregarAlCarrito;
                    pbProducto.Click += AgregarAlCarrito;
                    lblNombre.Click += AgregarAlCarrito;
                    lblPrecio.Click += AgregarAlCarrito;

                    panel1Productos.Controls.Add(panelProducto);
                }
            }
        }

        private void CargarProductosFiltrados(string filtroNombre = "", int? idCategoria = null)
        {
            panel1Productos.Controls.Clear(); 

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
            SELECT IdProducto, Nombre, PrecioVenta, RutaFoto
            FROM Productos
            WHERE Estado = 1";

                
                if (idCategoria != null)
                    query += " AND IdCategoria = @IdCategoria";

                if (!string.IsNullOrEmpty(filtroNombre))
                    query += " AND Nombre LIKE @FiltroNombre";

                SqlCommand cmd = new SqlCommand(query, con);
                if (idCategoria != null)
                    cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);

                if (!string.IsNullOrEmpty(filtroNombre))
                    cmd.Parameters.AddWithValue("@FiltroNombre", "%" + filtroNombre + "%");

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Panel panelProducto = new Panel();
                    panelProducto.Width = 120;
                    panelProducto.Height = 160;
                    panelProducto.Margin = new Padding(10);
                    panelProducto.Tag = reader["IdProducto"];

                    PictureBox pbProducto = new PictureBox();
                    pbProducto.Dock = DockStyle.Top;
                    pbProducto.Height = 100;
                    pbProducto.SizeMode = PictureBoxSizeMode.Zoom;
                    string ruta = reader["RutaFoto"].ToString();
                    if (!string.IsNullOrEmpty(ruta) && File.Exists(ruta))
                        pbProducto.Image = Image.FromFile(ruta);

                    Label lblNombre = new Label();
                    lblNombre.Text = reader["Nombre"].ToString();
                    lblNombre.Dock = DockStyle.Top;
                    lblNombre.TextAlign = ContentAlignment.MiddleCenter;
                    lblNombre.Height = 25;
                    lblNombre.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                    Label lblPrecio = new Label();
                    lblPrecio.Text = "RD$ " + Convert.ToDecimal(reader["PrecioVenta"]).ToString("N2");
                    lblPrecio.Dock = DockStyle.Top;
                    lblPrecio.TextAlign = ContentAlignment.MiddleCenter;
                    lblPrecio.Height = 20;
                    lblPrecio.Font = new Font("Segoe UI", 9, FontStyle.Regular);

                    panelProducto.Controls.Add(lblPrecio);
                    panelProducto.Controls.Add(lblNombre);
                    panelProducto.Controls.Add(pbProducto);

                    panelProducto.Click += (s, ev) =>
                    {
                        int idProducto = (int)((Panel)s).Tag;
                        MessageBox.Show("Producto seleccionado: " + idProducto);
                    };

                    panel1Productos.Controls.Add(panelProducto);
                }
            }
        }
        private void MovimientoPOS_Load(object sender, EventArgs e)
        {
            CargarCategorias();
            CargarSalas();
            CargarMesas();
            CargarProductos();
            ConfigurarDataGridView();
            CargarDatos();
            EventosGlobales.DispararPedidoEntregado();

        }

        private void cmbSalas_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbSalas.SelectedValue == null)
                return;

            if (cmbSalas.SelectedValue is DataRowView)
                return;

            int idSala = Convert.ToInt32(cmbSalas.SelectedValue);

            if (idSala == 0)
                CargarMesas(); 
            else
                CargarMesas(idSala);
        }

        private void cmbcategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbcategoria.SelectedValue == null)
                return;

        
            if (cmbcategoria.SelectedValue is DataRowView)
                return;

            int idCategoria = Convert.ToInt32(cmbcategoria.SelectedValue);

            if (idCategoria == 0)
                CargarProductos(); 
            else
                CargarProductos(idCategoria); 
            FiltrarProductos();
        }


        private void FiltrarProductos()
        {
            string filtroNombre = txtbuscar.Text.Trim();
            int? idCategoria = null;

            if (cmbcategoria.SelectedValue != null && !(cmbcategoria.SelectedValue is DataRowView))
            {
                int cat = Convert.ToInt32(cmbcategoria.SelectedValue);
                if (cat != 0) idCategoria = cat;
            }

            CargarProductosFiltrados(filtroNombre, idCategoria);
        }

        private void ConfigurarDataGridView()
        {
          
            dgvProductos.Columns.Clear();

           
            DataGridViewTextBoxColumn colCodigo = new DataGridViewTextBoxColumn();
            colCodigo.HeaderText = "Código";
            colCodigo.Name = "Codigo";
            colCodigo.Width = 80;
            dgvProductos.Columns.Add(colCodigo);

            
            DataGridViewTextBoxColumn colProducto = new DataGridViewTextBoxColumn();
            colProducto.HeaderText = "Producto";
            colProducto.Name = "Producto";
            colProducto.Width = 180;
            dgvProductos.Columns.Add(colProducto);

            DataGridViewTextBoxColumn colPrecio = new DataGridViewTextBoxColumn();
            colPrecio.HeaderText = "P. Unitario";
            colPrecio.Name = "PrecioUnitario";
            colPrecio.Width = 100;
            dgvProductos.Columns.Add(colPrecio);

           
            DataGridViewTextBoxColumn colCantidad = new DataGridViewTextBoxColumn();
            colCantidad.HeaderText = "Cantidad";
            colCantidad.Name = "Cantidad";
            colCantidad.Width = 80;
            dgvProductos.Columns.Add(colCantidad);

            DataGridViewTextBoxColumn colSubtotal = new DataGridViewTextBoxColumn();
            colSubtotal.HeaderText = "Subtotal";
            colSubtotal.Name = "Subtotal";
            colSubtotal.Width = 100;
            colSubtotal.ReadOnly = true;
            dgvProductos.Columns.Add(colSubtotal);

            DataGridViewTextBoxColumn colTotal = new DataGridViewTextBoxColumn();
            colTotal.HeaderText = "Total";
            colTotal.Name = "Total";
            colTotal.Width = 100;
            colTotal.ReadOnly = true;
            dgvProductos.Columns.Add(colTotal);

            
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        private const decimal ITBIS = 0.18m; 

        private void AgregarProductoAlCarrito(int idProducto, string nombre, decimal precio, int stock)
        {
            
            DataGridViewRow rowExistente = null;
            foreach (DataGridViewRow row in dgvProductos.Rows)
            {
                if (Convert.ToInt32(row.Cells["Codigo"].Value) == idProducto)
                {
                    rowExistente = row;
                    break;
                }
            }

            if (rowExistente != null)
            {
                int cantidadActual = Convert.ToInt32(rowExistente.Cells["Cantidad"].Value);

                if (cantidadActual + 1 > stock)
                {
                    MessageBox.Show("No hay suficiente stock disponible.", "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                rowExistente.Cells["Cantidad"].Value = cantidadActual + 1;
                decimal subtotal = (cantidadActual + 1) * precio;
                rowExistente.Cells["Subtotal"].Value = subtotal;
                rowExistente.Cells["Total"].Value = subtotal; 
            }
            else
            {
                // Nuevo producto
                if (stock <= 0)
                {
                    MessageBox.Show("Producto sin stock disponible.", "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int nuevaCantidad = 1;
                decimal subtotal = nuevaCantidad * precio;

                dgvProductos.Rows.Add(idProducto, nombre, precio, nuevaCantidad, subtotal, subtotal);
            }

            // Recalcular el total general
            CalcularTotales();
        }

        private void CalcularTotales()
        {
            decimal sumaSubtotales = 0;

            foreach (DataGridViewRow row in dgvProductos.Rows)
            {
                if (row.Cells["Subtotal"].Value != null)
                    sumaSubtotales += Convert.ToDecimal(row.Cells["Subtotal"].Value);
            }

            subtotalPedido = sumaSubtotales;
            impuestoPedido = subtotalPedido * ITBIS;
            totalPedido = subtotalPedido + impuestoPedido;

            // Mostrar en labels (en pesos dominicanos)
            lblSubtotal.Text = $"Subtotal: RD${subtotalPedido:N2}";
            lblItbis.Text = $"ITBIS (18%): RD${impuestoPedido:N2}";
            lblTotalGeneral.Text = $"Total: RD${totalPedido:N2}";
        }

        private decimal subtotalPedido = 0;
        private decimal impuestoPedido = 0;
        private decimal totalPedido = 0;


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FiltrarProductos();
        }



        class MesaInfo
        {
            public int IdMesa { get; set; }
            public bool Disponible { get; set; }
        }

       
        private int mesaSeleccionada = 0;


        private void btnsolicitar_Click(object sender, EventArgs e)
        {

            if (dgvProductos.Rows.Count == 0)
            {
                MessageBox.Show("No hay productos en el pedido.");
                return;
            }

            if (mesaSeleccionada == 0)
            {
                MessageBox.Show("Debe seleccionar una mesa.");
                return;
            }

            if (!clienteSeleccionado.HasValue)
            {
                MessageBox.Show("Debe seleccionar un cliente antes de guardar el pedido.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // -------------------------------
                    // 1️⃣ Insertar pedido
                    // -------------------------------
                    string queryPedido = @"
                INSERT INTO pedidos (idusuario, idcliente, cliente, idmesa, total, subtotal, impuesto, estado)
                VALUES (@IdUsuario, @IdCliente, @Cliente, @IdMesa, @Total, @SubTotal, @Impuesto, 0);
                SELECT SCOPE_IDENTITY();
                UPDATE mesas SET disponible = 0 WHERE idmesa = @IdMesa;
            ";

                    decimal total = totalPedido;
                    decimal subtotal = subtotalPedido;
                    decimal impuesto = impuestoPedido;

                    SqlCommand cmdPedido = new SqlCommand(queryPedido, con, transaction);
                    cmdPedido.Parameters.AddWithValue("@IdUsuario", 1); // Usuario por defecto

                    if (clienteSeleccionado.HasValue)
                    {
                        cmdPedido.Parameters.AddWithValue("@IdCliente", clienteSeleccionado.Value);
                        cmdPedido.Parameters.AddWithValue("@Cliente", txtCliente.Text);
                    }
                    else
                    {
                        cmdPedido.Parameters.AddWithValue("@IdCliente", DBNull.Value);
                        cmdPedido.Parameters.AddWithValue("@Cliente", DBNull.Value);
                    }

                    cmdPedido.Parameters.AddWithValue("@IdMesa", mesaSeleccionada);
                    cmdPedido.Parameters.AddWithValue("@Total", total);
                    cmdPedido.Parameters.AddWithValue("@SubTotal", subtotal);
                    cmdPedido.Parameters.AddWithValue("@Impuesto", impuesto);

                    int idPedido = Convert.ToInt32(cmdPedido.ExecuteScalar());

                    // -------------------------------
                    // 2️⃣ Insertar detalles del pedido
                    // -------------------------------
                    foreach (DataGridViewRow row in dgvProductos.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int idProducto = Convert.ToInt32(row.Cells["Codigo"].Value);
                        int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                        decimal precioUnitario = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value);
                        decimal subtotalProducto = precioUnitario * cantidad;

                        string queryDetalle = @"
                    INSERT INTO pedidoproducto (idpedido, idproducto, cantidad, preciounitario, preciototal)
                    VALUES (@IdPedido, @IdProducto, @Cantidad, @PrecioUnitario, @PrecioTotal)
                ";

                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, transaction);
                        cmdDetalle.Parameters.AddWithValue("@IdPedido", idPedido);
                        cmdDetalle.Parameters.AddWithValue("@IdProducto", idProducto);
                        cmdDetalle.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", precioUnitario);
                        cmdDetalle.Parameters.AddWithValue("@PrecioTotal", subtotalProducto);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    // -------------------------------
                    // 3️⃣ Insertar factura
                    // -------------------------------
                    string queryFactura = @"
                INSERT INTO facturas (idpedido, total, subtotal, impuesto, estado, fecha)
                VALUES (@IdPedido, @Total, @SubTotal, @Impuesto, 0, GETDATE());
                SELECT SCOPE_IDENTITY();
            ";

                    SqlCommand cmdFactura = new SqlCommand(queryFactura, con, transaction);
                    cmdFactura.Parameters.AddWithValue("@IdPedido", idPedido);
                    cmdFactura.Parameters.AddWithValue("@Total", total);
                    cmdFactura.Parameters.AddWithValue("@SubTotal", subtotal);
                    cmdFactura.Parameters.AddWithValue("@Impuesto", impuesto);

                    int idFactura = Convert.ToInt32(cmdFactura.ExecuteScalar());

                    // -------------------------------
                    // 4️⃣ Commit y actualizar UI
                    // -------------------------------
                    transaction.Commit();

                    // Cambiar color de la mesa
                    foreach (Control ctrl in panelMesas.Controls)
                    {
                        if (ctrl is Panel panel && panel.Tag != null)
                        {
                            dynamic datos = panel.Tag;
                            if (datos.IdMesa == mesaSeleccionada)
                            {
                                panel.BackColor = Color.Red;
                                ToolTip tt = new ToolTip();
                                tt.SetToolTip(panel, "Pedido registrado");
                                panel.Tag = new { IdMesa = datos.IdMesa, Disponible = false };
                            }
                        }
                    }

                    MessageBox.Show("Pedido y factura registrados correctamente.");

                    // Limpiar formulario
                    dgvProductos.Rows.Clear();
                    lblTotalGeneral.Text = "0.00";
                    lblSubtotal.Text = "0.00";
                    lblItbis.Text = "0.00";
                    mesaSeleccionada = 0;
                    clienteSeleccionado = null;
                    txtCliente.Text = "";

                    // -------------------------------
                    // 5️⃣ Actualizar historial de facturas si está abierto
                    // -------------------------------
                    var historial = Application.OpenForms.OfType<ControlFacturas>().FirstOrDefault();
                    if (historial != null)
                    {
                        historial.RefrescarHistorialFacturas();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error al registrar pedido y factura: " + ex.Message);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            txtCliente.Text = "";
            txtbuscar.Text = "";

            
            clienteSeleccionado = null;

           
            CargarDatos();
            dataGridView1.ClearSelection();

           
            txtCliente.Focus();
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            
            CargarDatos();

           
            dataGridView1.Visible = true;

           
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            txtCliente.Text = "";
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


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                txtCliente.Text = fila.Cells["Nombre"].Value.ToString();

                clienteSeleccionado = Convert.ToInt32(fila.Cells["IdCliente"].Value);
                txtCliente.Enabled = false;
                
                
                txtCliente.TabStop = false;   
              
                txtCliente.BackColor = Color.White; 

                
                dataGridView1.Visible = false;
            }
        }

        private int? clienteSeleccionado = null;

        private void button1_Click(object sender, EventArgs e)
        {
            PedidosPendientes ver = new PedidosPendientes();
            ver.Show();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           ControlFacturas ver = new ControlFacturas();
            ver.Show();
        }
    }

}
