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
    public partial class ReporteFinal : Form
    {
        
        private readonly string tipoReporte;
        private readonly byte mes;
        private readonly int año;
        private readonly decimal total, subtotal, impuesto;
        private readonly string usuario;
        private readonly DataTable datosFactura;
        public ReporteFinal(string usuario, string tipo, byte mes, int año, decimal total, decimal subtotal, decimal impuesto, DataTable datosFactura)
        {
            InitializeComponent();


            this.usuario = usuario;
            this.tipoReporte = tipo;
            this.mes = mes;
            this.año = año;
            this.subtotal = subtotal;
            this.total = total;
            this.impuesto = impuesto;
            this.datosFactura = datosFactura;

        }

        private void ReporteFinal_Load(object sender, EventArgs e)
        {
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Restaurante.Movimientos.ReportVentas.rdlc";

            // Asigna el DataSource al ReportViewer
            ReportDataSource rds = new ReportDataSource("DataSet2", datosFactura); // ← Aquí está el cambio
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(rds);

            // Parámetros del reporte
            ReportParameter[] parameters = new ReportParameter[]
            {
        new ReportParameter("rUsuario", this.usuario),
        new ReportParameter("rMes", this.mes.ToString()),
        new ReportParameter("rAno", this.año.ToString()),
        new ReportParameter("rTipo", this.tipoReporte.ToString()),
        new ReportParameter("rTotal", this.total.ToString("N2")),
        new ReportParameter("rSubtotal", this.subtotal.ToString("N2")),
        new ReportParameter("rImpuesto", this.impuesto.ToString("N2"))
            };
            this.reportViewer1.LocalReport.SetParameters(parameters);

            this.reportViewer1.RefreshReport();
        }
    }
}
