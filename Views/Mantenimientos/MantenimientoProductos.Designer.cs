namespace Restaurante.Views.Mantenimientos
{
    partial class MantenimientoProductos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.dtproducto = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.txtnombreproducto = new System.Windows.Forms.TextBox();
            this.txtcodigobarra = new System.Windows.Forms.TextBox();
            this.txtcodigoproducto = new System.Windows.Forms.TextBox();
            this.txtstockminimo = new System.Windows.Forms.TextBox();
            this.txtstockmaximo = new System.Windows.Forms.TextBox();
            this.txtpreciocompra = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtnolote = new System.Windows.Forms.TextBox();
            this.txtprecioventa = new System.Windows.Forms.TextBox();
            this.fechaelaboracion = new System.Windows.Forms.DateTimePicker();
            this.fechavencimiento = new System.Windows.Forms.DateTimePicker();
            this.cmbproveedor = new System.Windows.Forms.ComboBox();
            this.cmbimpuesto = new System.Windows.Forms.ComboBox();
            this.cmbcategoria = new System.Windows.Forms.ComboBox();
            this.txtexistenciaprod = new System.Windows.Forms.TextBox();
            this.txtdescuento = new System.Windows.Forms.TextBox();
            this.pbfoto = new System.Windows.Forms.PictureBox();
            this.btnguardar = new System.Windows.Forms.Button();
            this.btnlimpiar = new System.Windows.Forms.Button();
            this.btnborrar = new System.Windows.Forms.Button();
            this.btnabrir = new System.Windows.Forms.Button();
            this.btnfoto = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtproducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbfoto)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnfoto);
            this.panel1.Controls.Add(this.pbfoto);
            this.panel1.Controls.Add(this.dtproducto);
            this.panel1.Location = new System.Drawing.Point(535, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(562, 545);
            this.panel1.TabIndex = 15;
            // 
            // dtproducto
            // 
            this.dtproducto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtproducto.Location = new System.Drawing.Point(3, 279);
            this.dtproducto.Name = "dtproducto";
            this.dtproducto.RowHeadersWidth = 51;
            this.dtproducto.RowTemplate.Height = 24;
            this.dtproducto.Size = new System.Drawing.Size(556, 263);
            this.dtproducto.TabIndex = 0;
            this.dtproducto.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtproducto_CellClick);
            this.dtproducto.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtproducto_CellContentClick_1);
            this.dtproducto.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dtproducto_CellFormatting);
            this.dtproducto.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dtproducto_DataBindingComplete);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 22);
            this.label1.TabIndex = 62;
            this.label1.Text = "Nombre de Producto:";
            // 
            // txtnombreproducto
            // 
            this.txtnombreproducto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtnombreproducto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtnombreproducto.Location = new System.Drawing.Point(19, 52);
            this.txtnombreproducto.Multiline = true;
            this.txtnombreproducto.Name = "txtnombreproducto";
            this.txtnombreproducto.Size = new System.Drawing.Size(220, 40);
            this.txtnombreproducto.TabIndex = 63;
            // 
            // txtcodigobarra
            // 
            this.txtcodigobarra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtcodigobarra.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtcodigobarra.Location = new System.Drawing.Point(19, 678);
            this.txtcodigobarra.Multiline = true;
            this.txtcodigobarra.Name = "txtcodigobarra";
            this.txtcodigobarra.Size = new System.Drawing.Size(220, 40);
            this.txtcodigobarra.TabIndex = 64;
            // 
            // txtcodigoproducto
            // 
            this.txtcodigoproducto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtcodigoproducto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtcodigoproducto.Location = new System.Drawing.Point(19, 588);
            this.txtcodigoproducto.Multiline = true;
            this.txtcodigoproducto.Name = "txtcodigoproducto";
            this.txtcodigoproducto.Size = new System.Drawing.Size(220, 40);
            this.txtcodigoproducto.TabIndex = 65;
            // 
            // txtstockminimo
            // 
            this.txtstockminimo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtstockminimo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtstockminimo.Location = new System.Drawing.Point(19, 502);
            this.txtstockminimo.Multiline = true;
            this.txtstockminimo.Name = "txtstockminimo";
            this.txtstockminimo.Size = new System.Drawing.Size(220, 40);
            this.txtstockminimo.TabIndex = 66;
            // 
            // txtstockmaximo
            // 
            this.txtstockmaximo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtstockmaximo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtstockmaximo.Location = new System.Drawing.Point(19, 413);
            this.txtstockmaximo.Multiline = true;
            this.txtstockmaximo.Name = "txtstockmaximo";
            this.txtstockmaximo.Size = new System.Drawing.Size(220, 40);
            this.txtstockmaximo.TabIndex = 67;
            // 
            // txtpreciocompra
            // 
            this.txtpreciocompra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtpreciocompra.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpreciocompra.Location = new System.Drawing.Point(19, 141);
            this.txtpreciocompra.Multiline = true;
            this.txtpreciocompra.Name = "txtpreciocompra";
            this.txtpreciocompra.Size = new System.Drawing.Size(220, 40);
            this.txtpreciocompra.TabIndex = 70;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(15, 116);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(176, 22);
            this.label16.TabIndex = 71;
            this.label16.Text = "Precio de Compra:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(222, 22);
            this.label2.TabIndex = 72;
            this.label2.Text = "Existencia de Producto:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 388);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 22);
            this.label3.TabIndex = 73;
            this.label3.Text = "Stock Máximo:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 297);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 22);
            this.label5.TabIndex = 75;
            this.label5.Text = "Descuento:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 477);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 22);
            this.label4.TabIndex = 76;
            this.label4.Text = "Stock Minimo:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 563);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(193, 22);
            this.label6.TabIndex = 84;
            this.label6.Text = "Codigo de Producto:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(15, 653);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(162, 22);
            this.label7.TabIndex = 85;
            this.label7.Text = "Codigo de Barra:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(292, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 22);
            this.label8.TabIndex = 96;
            this.label8.Text = "Proveedor:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(292, 116);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 22);
            this.label9.TabIndex = 97;
            this.label9.Text = "Categoría:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(292, 208);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(159, 22);
            this.label17.TabIndex = 140;
            this.label17.Text = "Precio de Venta:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(292, 297);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 22);
            this.label10.TabIndex = 141;
            this.label10.Text = "Impuesto:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(292, 388);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 22);
            this.label12.TabIndex = 142;
            this.label12.Text = "No. de Lote:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(292, 477);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(183, 22);
            this.label13.TabIndex = 143;
            this.label13.Text = "Fecha Elaboración:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(292, 563);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(171, 22);
            this.label14.TabIndex = 144;
            this.label14.Text = "Fecha Expiración:";
            // 
            // txtnolote
            // 
            this.txtnolote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtnolote.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtnolote.Location = new System.Drawing.Point(296, 413);
            this.txtnolote.Multiline = true;
            this.txtnolote.Name = "txtnolote";
            this.txtnolote.Size = new System.Drawing.Size(220, 40);
            this.txtnolote.TabIndex = 145;
            // 
            // txtprecioventa
            // 
            this.txtprecioventa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtprecioventa.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtprecioventa.Location = new System.Drawing.Point(296, 233);
            this.txtprecioventa.Multiline = true;
            this.txtprecioventa.Name = "txtprecioventa";
            this.txtprecioventa.Size = new System.Drawing.Size(220, 40);
            this.txtprecioventa.TabIndex = 146;
            // 
            // fechaelaboracion
            // 
            this.fechaelaboracion.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fechaelaboracion.Location = new System.Drawing.Point(298, 511);
            this.fechaelaboracion.Name = "fechaelaboracion";
            this.fechaelaboracion.Size = new System.Drawing.Size(218, 22);
            this.fechaelaboracion.TabIndex = 147;
            // 
            // fechavencimiento
            // 
            this.fechavencimiento.Location = new System.Drawing.Point(296, 588);
            this.fechavencimiento.Name = "fechavencimiento";
            this.fechavencimiento.Size = new System.Drawing.Size(218, 22);
            this.fechavencimiento.TabIndex = 148;
            // 
            // cmbproveedor
            // 
            this.cmbproveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbproveedor.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbproveedor.FormattingEnabled = true;
            this.cmbproveedor.Location = new System.Drawing.Point(296, 55);
            this.cmbproveedor.Name = "cmbproveedor";
            this.cmbproveedor.Size = new System.Drawing.Size(218, 34);
            this.cmbproveedor.TabIndex = 149;
            // 
            // cmbimpuesto
            // 
            this.cmbimpuesto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbimpuesto.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbimpuesto.FormattingEnabled = true;
            this.cmbimpuesto.Location = new System.Drawing.Point(296, 331);
            this.cmbimpuesto.Name = "cmbimpuesto";
            this.cmbimpuesto.Size = new System.Drawing.Size(218, 34);
            this.cmbimpuesto.TabIndex = 150;
            // 
            // cmbcategoria
            // 
            this.cmbcategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbcategoria.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbcategoria.FormattingEnabled = true;
            this.cmbcategoria.Location = new System.Drawing.Point(296, 153);
            this.cmbcategoria.Name = "cmbcategoria";
            this.cmbcategoria.Size = new System.Drawing.Size(218, 34);
            this.cmbcategoria.TabIndex = 151;
            // 
            // txtexistenciaprod
            // 
            this.txtexistenciaprod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtexistenciaprod.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtexistenciaprod.Location = new System.Drawing.Point(19, 232);
            this.txtexistenciaprod.Multiline = true;
            this.txtexistenciaprod.Name = "txtexistenciaprod";
            this.txtexistenciaprod.Size = new System.Drawing.Size(218, 41);
            this.txtexistenciaprod.TabIndex = 152;
            // 
            // txtdescuento
            // 
            this.txtdescuento.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtdescuento.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtdescuento.Location = new System.Drawing.Point(19, 322);
            this.txtdescuento.Multiline = true;
            this.txtdescuento.Name = "txtdescuento";
            this.txtdescuento.Size = new System.Drawing.Size(218, 41);
            this.txtdescuento.TabIndex = 153;
            // 
            // pbfoto
            // 
            this.pbfoto.Location = new System.Drawing.Point(182, 3);
            this.pbfoto.Name = "pbfoto";
            this.pbfoto.Size = new System.Drawing.Size(204, 221);
            this.pbfoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbfoto.TabIndex = 1;
            this.pbfoto.TabStop = false;
            this.pbfoto.Click += new System.EventHandler(this.pbfoto_Click);
            // 
            // btnguardar
            // 
            this.btnguardar.Font = new System.Drawing.Font("Rockwell", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnguardar.Location = new System.Drawing.Point(19, 739);
            this.btnguardar.Name = "btnguardar";
            this.btnguardar.Size = new System.Drawing.Size(135, 66);
            this.btnguardar.TabIndex = 154;
            this.btnguardar.Text = "Guardar";
            this.btnguardar.UseVisualStyleBackColor = true;
            this.btnguardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnlimpiar
            // 
            this.btnlimpiar.Font = new System.Drawing.Font("Rockwell", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnlimpiar.Location = new System.Drawing.Point(215, 743);
            this.btnlimpiar.Name = "btnlimpiar";
            this.btnlimpiar.Size = new System.Drawing.Size(140, 64);
            this.btnlimpiar.TabIndex = 155;
            this.btnlimpiar.Text = "Limpiar";
            this.btnlimpiar.UseVisualStyleBackColor = true;
            this.btnlimpiar.Click += new System.EventHandler(this.btnlimpiar_Click);
            // 
            // btnborrar
            // 
            this.btnborrar.Location = new System.Drawing.Point(616, 763);
            this.btnborrar.Name = "btnborrar";
            this.btnborrar.Size = new System.Drawing.Size(14, 23);
            this.btnborrar.TabIndex = 156;
            this.btnborrar.Text = "button3";
            this.btnborrar.UseVisualStyleBackColor = true;
            this.btnborrar.Click += new System.EventHandler(this.btnborrar_Click_1);
            // 
            // btnabrir
            // 
            this.btnabrir.Font = new System.Drawing.Font("Rockwell", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnabrir.Location = new System.Drawing.Point(406, 743);
            this.btnabrir.Name = "btnabrir";
            this.btnabrir.Size = new System.Drawing.Size(138, 66);
            this.btnabrir.TabIndex = 157;
            this.btnabrir.Text = "Buscar";
            this.btnabrir.UseVisualStyleBackColor = true;
            this.btnabrir.Click += new System.EventHandler(this.btnAbrir_Click);
            // 
            // btnfoto
            // 
            this.btnfoto.Font = new System.Drawing.Font("Rockwell", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnfoto.Location = new System.Drawing.Point(22, 49);
            this.btnfoto.Name = "btnfoto";
            this.btnfoto.Size = new System.Drawing.Size(120, 80);
            this.btnfoto.TabIndex = 2;
            this.btnfoto.Text = "Buscar la Foto";
            this.btnfoto.UseVisualStyleBackColor = true;
            this.btnfoto.Click += new System.EventHandler(this.btnfoto_Click);
            // 
            // MantenimientoProductos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 840);
            this.Controls.Add(this.btnabrir);
            this.Controls.Add(this.btnborrar);
            this.Controls.Add(this.btnlimpiar);
            this.Controls.Add(this.btnguardar);
            this.Controls.Add(this.txtdescuento);
            this.Controls.Add(this.txtexistenciaprod);
            this.Controls.Add(this.cmbcategoria);
            this.Controls.Add(this.cmbimpuesto);
            this.Controls.Add(this.cmbproveedor);
            this.Controls.Add(this.fechavencimiento);
            this.Controls.Add(this.fechaelaboracion);
            this.Controls.Add(this.txtprecioventa);
            this.Controls.Add(this.txtnolote);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txtpreciocompra);
            this.Controls.Add(this.txtstockmaximo);
            this.Controls.Add(this.txtstockminimo);
            this.Controls.Add(this.txtcodigoproducto);
            this.Controls.Add(this.txtcodigobarra);
            this.Controls.Add(this.txtnombreproducto);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "MantenimientoProductos";
            this.Text = "MantenimientoProductos";
            this.Load += new System.EventHandler(this.MantenimientoProductos_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtproducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbfoto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbfoto;
        private System.Windows.Forms.DataGridView dtproducto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtnombreproducto;
        private System.Windows.Forms.TextBox txtcodigobarra;
        private System.Windows.Forms.TextBox txtcodigoproducto;
        private System.Windows.Forms.TextBox txtstockminimo;
        private System.Windows.Forms.TextBox txtstockmaximo;
        private System.Windows.Forms.TextBox txtpreciocompra;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtnolote;
        private System.Windows.Forms.TextBox txtprecioventa;
        private System.Windows.Forms.DateTimePicker fechaelaboracion;
        private System.Windows.Forms.DateTimePicker fechavencimiento;
        private System.Windows.Forms.ComboBox cmbproveedor;
        private System.Windows.Forms.ComboBox cmbimpuesto;
        private System.Windows.Forms.ComboBox cmbcategoria;
        private System.Windows.Forms.TextBox txtexistenciaprod;
        private System.Windows.Forms.TextBox txtdescuento;
        private System.Windows.Forms.Button btnfoto;
        private System.Windows.Forms.Button btnguardar;
        private System.Windows.Forms.Button btnlimpiar;
        private System.Windows.Forms.Button btnborrar;
        private System.Windows.Forms.Button btnabrir;
    }
}