using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restaurante.Movimientos
{


    public partial class ReporteFacturas : Form
    {
        public int IdPedido { get; set; }
        public string Usuario { get; set; }
        public string Cliente { get; set; }
        public string NCF { get; set; } = "001-001-0000001234"; // Valor por defecto
        public decimal Subtotal { get; set; }
        public float Impuesto { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now; // Nueva propiedad para usar en el reporte

        private DataTable _dataSource;
        public ReporteFacturas(DataTable dataSource, string usuario, string tipo, byte mes, int año, decimal total, decimal subtotal, float impuesto)
        {
            InitializeComponent();


            Usuario = usuario;
            Subtotal = subtotal;
            Impuesto = impuesto;
            Total = total;

            _dataSource = dataSource;

            this.Load += ReporteFacturas_Load;
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            reportViewer1.RefreshReport();


            DataTable dt = new DataTable();
            dt.Columns.Add("Codigo", typeof(string));     // Código del producto
            dt.Columns.Add("Producto", typeof(string));   // Nombre del producto
            dt.Columns.Add("Cant", typeof(int));          // Cantidad
            dt.Columns.Add("Precio", typeof(decimal));    // Precio unitario
            dt.Columns.Add("Total", typeof(decimal));     // Total por línea
            dt.Columns.Add("Impuesto", typeof(decimal));  // Impuesto por línea

            // Filas de ejemplo
            dt.Rows.Add("P001", "Arroz", 2, 50.00m, 100.00m, 18.00m);
            dt.Rows.Add("P002", "Pollo", 1, 120.00m, 120.00m, 21.60m);
            dt.Rows.Add("P003", "Refresco", 3, 30.00m, 90.00m, 16.20m);
        }

        private void ReporteFacturas_Load(object sender, EventArgs e)
        {

            if (this.reportViewer1 == null)
            {
                throw new ArgumentNullException(nameof(this.reportViewer1), "ReportViewer control is null");
            }



            // Crea los parámetros del reporte
            ReportParameter[] nada = new ReportParameter[]
            {
        new ReportParameter("rDate", Fecha.ToString("dd/MM/yyyy")),
    new ReportParameter("rCliente", Cliente ?? "Cliente no definido"),
    new ReportParameter("rUsuario", Usuario ?? "Usuario no definido"),
    new ReportParameter("rSubtotal", Subtotal.ToString("N2")),
    new ReportParameter("rNCF", NCF ?? "001-001-0000001234"), // Puedes parametrizar esto si lo necesitas
    new ReportParameter("rTotal", Total.ToString("N2")),
    new ReportParameter("rImpuesto", Impuesto.ToString("N2")),
    new ReportParameter("rHora", Fecha.ToString("hh:mm tt")),
            };



            // Asigna la lista directamente como la fuente de datos del reporte
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", _dataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "Restaurante.Movimientos.FacturaReport.rdlc";
            // Configura el ReportViewer
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.SetParameters(nada);

            reportViewer1.ZoomMode = ZoomMode.PageWidth;

            reportViewer1.RefreshReport();

            reportViewer1.ZoomMode = ZoomMode.PageWidth;

            this.reportViewer1.RefreshReport();
        }
    }
}
