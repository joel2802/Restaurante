using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restaurante.Views.Mantenimientos
{
    public partial class MantenimientoProductos : Form
    {
        SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;");
        private int idProductoSeleccionado = 0;
        string carpetaFotos = @"C:\Users\joelp\source\repos\Restaurante\FotoProductos";
        string rutaFoto = "";


        public MantenimientoProductos()
        {
            InitializeComponent();
            panel1.Visible = false;
            dtproducto.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dtproducto.ReadOnly = true;
            txtpreciocompra.KeyPress += SoloNumerosDecimales;
            txtprecioventa.KeyPress += SoloNumerosDecimales;
            txtdescuento.KeyPress += SoloNumerosDecimales;

            txtexistenciaprod.KeyPress += SoloNumeros;
            txtstockminimo.KeyPress += SoloNumeros;
            txtstockmaximo.KeyPress += SoloNumeros;

            txtcodigoproducto.KeyPress += SoloNumeros;
            txtnolote.KeyPress += SoloNumeros;
            txtcodigobarra.KeyPress += SoloNumeros;


        }



        private void CargarProductos()
        {
            try
            {
                string query = @"
                SELECT 
                    p.*,
                    c.Nombre AS CategoriaNombre,
                    i.Nombre AS ImpuestoNombre,
                    i.porcentaje AS ImpuestoPorcentaje,
                    pr.Nombre AS ProveedorNombre
                FROM 
                    Productos p
                INNER JOIN 
                    Categorias c ON p.IdCategoria = c.IdCategoria
                INNER JOIN 
                    Impuestos i ON p.IdImpuesto = i.IdImpuesto
                LEFT JOIN 
                    Proveedor pr ON p.IdProveedor = pr.IdProveedor
                WHERE p.estado = 1
                ";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dtproducto.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void LimpiarCampos()
        {

            txtnombreproducto.Clear();
            txtpreciocompra.Clear();
            txtexistenciaprod.Clear();
            txtdescuento.Clear();
            txtstockmaximo.Clear();
            txtstockminimo.Clear();
            txtcodigoproducto.Clear();
            cmbproveedor.SelectedIndex = -1;
            cmbcategoria.SelectedIndex = -1;
            txtprecioventa.Clear();
            cmbimpuesto.SelectedIndex = -1;
            txtnolote.Clear();
            fechaelaboracion.Value = DateTime.Now;
            fechavencimiento.Value = DateTime.Now;
            txtcodigobarra.Clear();
            idProductoSeleccionado = 0;
            pbfoto.Image = null;
            rutaFoto = "";


        }



        private void CargarImagenDesdeBD(string rutaFoto)
        {
            try
            {
            
                if (!string.IsNullOrWhiteSpace(rutaFoto) && File.Exists(rutaFoto))
                {
                    pbfoto.Image = Image.FromFile(rutaFoto);
                }
                else
                {
                    
                    pbfoto.Image = null;
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pbfoto.Image = null;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
           
            if (!ValidarCampos()) 
                return;

            try
            {
                bool esActualizacion = idProductoSeleccionado > 0;

                using (SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;"))
                {
                    con.Open();
                    SqlCommand cmd;

                    if (esActualizacion)
                    {
                        
                        cmd = new SqlCommand(@"
                    UPDATE Productos SET
                        CodigoProducto = @CodigoProducto,
                        Nombre = @Nombre,
                        IdCategoria = @IdCategoria,
                        PrecioVenta = @PrecioVenta,
                        IdImpuesto = @IdImpuesto,
                        IdProveedor = @IdProveedor,
                        PrecioCompra = @PrecioCompra,
                        Descuento = @Descuento,
                        Existencia = @Existencia,
                        NoLote = @NoLote,
                        StockMinimo = @StockMinimo,
                        StockMaximo = @StockMaximo,
                        FechaElaboracion = @FechaElaboracion,
                        FechaExpiracion = @FechaExpiracion,
                        CodigoDeBarra = @CodigoDeBarra,
                        RutaFoto = @RutaFoto,
                        Estado = @Estado,
                        EsProductoFinal = @EsProductoFinal
                    WHERE IdProducto = @IdProducto", con);

                        cmd.Parameters.AddWithValue("@IdProducto", idProductoSeleccionado);
                    }
                    else
                    {
                        
                        cmd = new SqlCommand(@"
                    INSERT INTO Productos
                    (CodigoProducto, Nombre, IdCategoria, PrecioVenta, IdImpuesto, IdProveedor, PrecioCompra, Descuento, Existencia, NoLote, StockMinimo, StockMaximo, FechaElaboracion, FechaExpiracion, CodigoDeBarra, RutaFoto, Estado, EsProductoFinal)
                    VALUES
                    (@CodigoProducto, @Nombre, @IdCategoria, @PrecioVenta, @IdImpuesto, @IdProveedor, @PrecioCompra, @Descuento, @Existencia, @NoLote, @StockMinimo, @StockMaximo, @FechaElaboracion, @FechaExpiracion, @CodigoDeBarra, @RutaFoto, @Estado, @EsProductoFinal)", con);
                    }

                    
                    cmd.Parameters.AddWithValue("@CodigoProducto", txtcodigoproducto.Text.Trim());
                    cmd.Parameters.AddWithValue("@Nombre", txtnombreproducto.Text.Trim());
                    cmd.Parameters.AddWithValue("@IdCategoria", cmbcategoria.SelectedValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrecioVenta", decimal.TryParse(txtprecioventa.Text, out decimal pv) ? pv : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdImpuesto", cmbimpuesto.SelectedValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdProveedor", cmbproveedor.SelectedValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrecioCompra", decimal.TryParse(txtpreciocompra.Text, out decimal pc) ? pc : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descuento", decimal.TryParse(txtdescuento.Text, out decimal desc) ? desc : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Existencia", int.TryParse(txtexistenciaprod.Text, out int ex) ? ex : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoLote", txtnolote.Text.Trim() != "" ? (object)txtnolote.Text.Trim() : DBNull.Value);
                    cmd.Parameters.AddWithValue("@StockMinimo", int.TryParse(txtstockminimo.Text, out int sm) ? sm : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StockMaximo", int.TryParse(txtstockmaximo.Text, out int sM) ? sM : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaElaboracion", fechaelaboracion.Checked ? (object)fechaelaboracion.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaExpiracion", fechavencimiento.Checked ? (object)fechavencimiento.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@CodigoDeBarra", txtcodigobarra.Text.Trim() != "" ? (object)txtcodigobarra.Text.Trim() : DBNull.Value);
                    cmd.Parameters.AddWithValue("@RutaFoto", !string.IsNullOrEmpty(rutaFoto) ? (object)rutaFoto : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", 1); 
                    cmd.Parameters.AddWithValue("@EsProductoFinal", 1); 

                    cmd.ExecuteNonQuery();
                }

                CargarProductos();
                LimpiarCampos();

                MessageBox.Show(esActualizacion ? "Producto actualizado correctamente" : "Producto guardado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar o actualizar el producto:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnBorrar_Click(object sender, EventArgs e)
        {
            
        }



        private void CargarProductosEnGrid()
        {
            try
            {
                string query = @"
        SELECT 
            p.IdProducto,
            p.CodigoProducto,
            p.Nombre,
            p.IdCategoria,
            c.nombre AS Categoria,
            p.PrecioVenta,
            p.IdImpuesto,
            i.nombre AS Impuesto,
            p.IdProveedor,
            pr.nombre AS Proveedor,
            p.PrecioCompra,
            p.descuento,
            p.Existencia,
            p.NoLote,
            p.StockMinimo,
            p.StockMaximo,
            p.FechaElaboracion,
            p.FechaExpiracion,
            p.CodigoDeBarra,
            p.RutaFoto,
            p.Estado,
            p.EsProductoFinal
        FROM Productos p
        LEFT JOIN categorias c ON p.IdCategoria = c.idcategoria
        LEFT JOIN impuestos i ON p.IdImpuesto = i.idimpuesto
        LEFT JOIN proveedor pr ON p.IdProveedor = pr.idproveedor
        WHERE p.estado = 1";

                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                dtproducto.DataSource = dt;

                
                if (dtproducto.Columns.Contains("IdProducto")) dtproducto.Columns["IdProducto"].Visible = false;
                if (dtproducto.Columns.Contains("IdCategoria")) dtproducto.Columns["IdCategoria"].Visible = false;
                if (dtproducto.Columns.Contains("IdImpuesto")) dtproducto.Columns["IdImpuesto"].Visible = false;
                if (dtproducto.Columns.Contains("IdProveedor")) dtproducto.Columns["IdProveedor"].Visible = false;
                if (dtproducto.Columns.Contains("RutaFoto")) dtproducto.Columns["RutaFoto"].Visible = false;

                
                dtproducto.CellFormatting -= dtproducto_CellFormatting; 
                dtproducto.CellFormatting += dtproducto_CellFormatting;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }
        private void CargarProveedor()
        {
            try
            {
                using (SqlConnection c = new SqlConnection(con.ConnectionString))
                {
                    string sql = "SELECT idproveedor, nombre FROM Proveedor";
                    var da = new SqlDataAdapter(sql, c);
                    var dt = new DataTable();
                    da.Fill(dt);
                    cmbproveedor.DataSource = dt;
                    cmbproveedor.DisplayMember = "nombre";
                    cmbproveedor.ValueMember = "idproveedor";
                    cmbproveedor.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error cargando proveedores: " + ex.Message); }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            CargarProductosEnGrid();
        }
        private void CargarImpuesto()
        {
            try
            {
                using (SqlConnection c = new SqlConnection(con.ConnectionString))
                {
                    string sql = "SELECT idimpuesto, nombre FROM impuestos";
                    var da = new SqlDataAdapter(sql, c);
                    var dt = new DataTable();
                    da.Fill(dt);
                    cmbimpuesto.DataSource = dt;
                    cmbimpuesto.DisplayMember = "nombre";
                    cmbimpuesto.ValueMember = "idimpuesto";
                    cmbimpuesto.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error cargando impuestos: " + ex.Message); }
        }
        private void CargarCategoria()
        {
            try
            {
                using (SqlConnection c = new SqlConnection(con.ConnectionString))
                {
                    string sql = "SELECT idcategoria, nombre FROM Categorias WHERE estado = 1"; // 👈 aquí va tu campo real
                    var da = new SqlDataAdapter(sql, c);
                    var dt = new DataTable();
                    da.Fill(dt);

                    cmbcategoria.DataSource = dt;
                    cmbcategoria.DisplayMember = "nombre";
                    cmbcategoria.ValueMember = "idcategoria";
                   cmbcategoria.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando categorías: " + ex.Message);
            }
        }
        private void dtproducto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dtproducto.Rows[e.RowIndex];

            idProductoSeleccionado = Convert.ToInt32(row.Cells["IdProducto"].Value);

            
            txtcodigoproducto.Text = row.Cells["CodigoProducto"].Value?.ToString();
            txtnombreproducto.Text = row.Cells["Nombre"].Value?.ToString();



            if (row.Cells["IdCategoria"].Value != DBNull.Value)
                cmbcategoria.SelectedValue = Convert.ToInt32(row.Cells["IdCategoria"].Value);
            else
                cmbcategoria.SelectedIndex = -1;

            txtprecioventa.Text = row.Cells["PrecioVenta"].Value?.ToString();

            if (row.Cells["IdImpuesto"].Value != DBNull.Value)
                cmbimpuesto.SelectedValue = Convert.ToInt32(row.Cells["IdImpuesto"].Value);
            else
                cmbimpuesto.SelectedIndex = -1;

            if (row.Cells["IdProveedor"].Value != DBNull.Value)
                cmbproveedor.SelectedValue = Convert.ToInt32(row.Cells["IdProveedor"].Value);
            else
                cmbproveedor.SelectedIndex = -1;

            txtpreciocompra.Text = row.Cells["PrecioCompra"].Value?.ToString();
            txtdescuento.Text = row.Cells["Descuento"].Value?.ToString();
            txtexistenciaprod.Text = row.Cells["Existencia"].Value?.ToString();
            txtnolote.Text = row.Cells["NoLote"].Value?.ToString();
            txtstockminimo.Text = row.Cells["StockMinimo"].Value?.ToString();
            txtstockmaximo.Text = row.Cells["StockMaximo"].Value?.ToString();

            if (row.Cells["FechaElaboracion"].Value != DBNull.Value)
                fechaelaboracion.Value = Convert.ToDateTime(row.Cells["FechaElaboracion"].Value);
            else
                fechaelaboracion.Value = DateTime.Today;

            if (row.Cells["FechaExpiracion"].Value != DBNull.Value)
                fechavencimiento.Value = Convert.ToDateTime(row.Cells["FechaExpiracion"].Value);
            else
                fechavencimiento.Value = DateTime.Today;

            txtcodigobarra.Text = row.Cells["CodigoDeBarra"].Value?.ToString();

           
            rutaFoto = row.Cells["RutaFoto"].Value?.ToString();

            
            try
            {
                if (!string.IsNullOrEmpty(rutaFoto) && File.Exists(rutaFoto))
                {
                    using (var fs = new FileStream(rutaFoto, FileMode.Open, FileAccess.Read))
                    {
                        pbfoto.Image = Image.FromStream(fs);
                    }
                }
                else
                {
                    pbfoto.Image = null;
                }
            }
            catch
            {
                pbfoto.Image = null;
            }
        }

        public void EliminarProducto(int idProducto)
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=restaurante;Trusted_Connection=True;";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    command.CommandText = @"
                UPDATE productos 
                SET estado = 0 
                WHERE idproducto = @IdProducto";

                    command.Parameters.Add("@IdProducto", SqlDbType.Int).Value = idProducto;

                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Producto eliminado (estado = 0)", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el producto a eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error eliminando producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MantenimientoProductos_Load(object sender, EventArgs e)
        {
            CargarCategoria();
            CargarImpuesto();
            CargarProveedor();
            CargarProductos();
            dtproducto.DataBindingComplete += dtproducto_DataBindingComplete;
            dtproducto.CellFormatting += dtproducto_CellFormatting;

        }

        private void btnfoto_Click(object sender, EventArgs e)
        {
           
            string carpetaFotos = Path.Combine(
                @"C:\Users",
                Environment.UserName,
                @"source\repos\Restaurante\FotoProductos"
            );

            
            if (!Directory.Exists(carpetaFotos))
                Directory.CreateDirectory(carpetaFotos);

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.webp";
                ofd.InitialDirectory = carpetaFotos; 
                ofd.Title = "Seleccionar imagen";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                     
                        string nombreArchivo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(ofd.FileName);
                        string destino = Path.Combine(carpetaFotos, nombreArchivo);

                       
                        File.Copy(ofd.FileName, destino, true);

                        
                        rutaFoto = destino;

                        
                        byte[] bytes = File.ReadAllBytes(destino);
                        using (var ms = new MemoryStream(bytes))
                        {
                            pbfoto.Image = Image.FromStream(ms);
                        }

                        MessageBox.Show("Imagen seleccionada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SoloNumeros(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void SoloNumerosDecimales(object sender, KeyPressEventArgs e)
        {
       
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
      
            TextBox tb = sender as TextBox;
            if (e.KeyChar == '.' && tb != null && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }
        private bool ValidarCampos()
        {
          
            string nombre = txtnombreproducto.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre del producto es requerido.");
                return false;
            }
            if (nombre.Length > 100)
            {
                MessageBox.Show("El nombre del producto no puede exceder 100 caracteres.");
                return false;
            }

          
            if (cmbproveedor.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un proveedor.");
                return false;
            }

            
            string precioCompra = txtpreciocompra.Text.Trim();
            if (string.IsNullOrWhiteSpace(precioCompra))
            {
                MessageBox.Show("El precio de compra es requerido.");
                return false;
            }
            if (!decimal.TryParse(precioCompra, out decimal compra) || compra < 0)
            {
                MessageBox.Show("El precio de compra debe ser un número positivo.");
                return false;
            }

            if (cmbcategoria.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar una categoría válida.");
                return false;
            }

        
            string existencia = txtexistenciaprod.Text.Trim();
            if (string.IsNullOrWhiteSpace(existencia))
            {
                MessageBox.Show("La existencia del producto es requerida.");
                return false;
            }
            if (!int.TryParse(existencia, out int exist) || exist < 0)
            {
                MessageBox.Show("La existencia debe ser un número entero positivo.");
                return false;
            }

            string precioVenta = txtprecioventa.Text.Trim();
            if (string.IsNullOrWhiteSpace(precioVenta))
            {
                MessageBox.Show("El precio de venta es requerido.");
                return false;
            }
            if (!decimal.TryParse(precioVenta, out decimal venta) || venta < 0)
            {
                MessageBox.Show("El precio de venta debe ser un número positivo.");
                return false;
            }

           
            string descuento = txtdescuento.Text.Trim();
            if (string.IsNullOrWhiteSpace(descuento))
            {
                MessageBox.Show("El descuento es requerido.");
                return false;
            }
            if (!decimal.TryParse(descuento, out decimal desc) || desc < 0 || desc > 100)
            {
                MessageBox.Show("El descuento debe ser un número entre 0 y 100.");
                return false;
            }

          
            if (cmbimpuesto.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un impuesto.");
                return false;
            }

            
            string stockMax = txtstockmaximo.Text.Trim();
            if (string.IsNullOrWhiteSpace(stockMax))
            {
                MessageBox.Show("El stock máximo es requerido.");
                return false;
            }
            if (!int.TryParse(stockMax, out int max) || max < 0)
            {
                MessageBox.Show("El stock máximo debe ser un número entero positivo.");
                return false;
            }

            string stockMin = txtstockminimo.Text.Trim();
            if (string.IsNullOrWhiteSpace(stockMin))
            {
                MessageBox.Show("El stock mínimo es requerido.");
                return false;
            }
            if (!int.TryParse(stockMin, out int min) || min < 0)
            {
                MessageBox.Show("El stock mínimo debe ser un número entero positivo.");
                return false;
            }

            if (min > max)
            {
                MessageBox.Show("El stock mínimo no puede ser mayor que el stock máximo.");
                return false;
            }

            string lote = txtnolote.Text.Trim();
            if (string.IsNullOrWhiteSpace(lote))
            {
                MessageBox.Show("El número de lote es requerido.");
                return false;
            }
            if (lote.Length > 50)
            {
                MessageBox.Show("El número de lote no puede exceder 50 caracteres.");
                return false;
            }

          
            if (fechavencimiento.Value.Date <= fechaelaboracion.Value.Date)
            {
                MessageBox.Show("La fecha de vencimiento debe ser posterior a la fecha de elaboración.");
                return false;
            }

            
            string codigoProducto = txtcodigoproducto.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigoProducto))
            {
                MessageBox.Show("El código de producto es requerido.");
                return false;
            }
            if (codigoProducto.Length > 50)
            {
                MessageBox.Show("El código de producto no puede exceder 50 caracteres.");
                return false;
            }

         
            string codigoBarra = txtcodigobarra.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigoBarra))
            {
                MessageBox.Show("El código de barra es requerido.");
                return false;
            }
            if (!Regex.IsMatch(codigoBarra, @"^\d+$"))
            {
                MessageBox.Show("El código de barra debe contener solo números.");
                return false;
            }
            if (codigoBarra.Length > 20)
            {
                MessageBox.Show("El código de barra no puede exceder 20 caracteres.");
                return false;
            }

            return true;
        }
        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
        private void txtnombreproducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtpreciocompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtexistenciaprod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtdescuento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtstockmaximo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtstockminimo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtcodigoproducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtcodigobarra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void cmbproveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void cmbcategoria_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtprecioventa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void cmbimpuesto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void txtnolote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void pbfoto_Click(object sender, EventArgs e)
        {

        }

        private void dtproducto_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnborrar_Click_1(object sender, EventArgs e)
        {
            if (dtproducto.CurrentRow != null)
            {
                int id = Convert.ToInt32(dtproducto.CurrentRow.Cells["IdProducto"].Value);
                string nombre = dtproducto.CurrentRow.Cells["Nombre"].Value.ToString();

                DialogResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas desactivar este producto: {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    EliminarProducto(id); 
                    CargarProductosEnGrid();         
                    LimpiarCampos();     
                    
                }
            }
            else
            {
                MessageBox.Show("Seleccione un producto primero.");
            }
        }

        private void dtproducto_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dtproducto_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            string[] columnasOrdenadas = {
        "CodigoProducto",
        "Nombre",
        "Categoria",
        "PrecioVenta",
        "Impuesto",
        "Proveedor",
        "PrecioCompra",
        "Descuento",
        "Existencia",
        "NoLote",
        "StockMinimo",
        "StockMaximo",
        "FechaElaboracion",
        "FechaExpiracion",
        "CodigoDeBarra",
        "Estado",
        "EsProductoFinal"
    };

            for (int i = 0; i < columnasOrdenadas.Length; i++)
            {
                if (dtproducto.Columns.Contains(columnasOrdenadas[i]))
                {
                    dtproducto.Columns[columnasOrdenadas[i]].DisplayIndex = i;
                }
            }
        }

        private void dtproducto_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dtproducto.Columns[e.ColumnIndex].Name == "Estado" && e.Value != null)
            {
                int estado = Convert.ToInt32(e.Value);
                e.Value = estado == 1 ? "Activo" : "Inactivo";
                e.FormattingApplied = true;
            }

            if (dtproducto.Columns[e.ColumnIndex].Name == "EsProductoFinal" && e.Value != null)
            {
                int esFinal = Convert.ToInt32(e.Value);
                e.Value = esFinal == 1 ? "Sí" : "No";
                e.FormattingApplied = true;

             
                if (esFinal == 1)
                {
                    dtproducto.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }
    }
}

