using SistemaFacturacion.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;

namespace SistemaFacturacion.CLASES
{
    public static class GeneradorRecibos
    {
        public static void GenerarReciboPDF(Pago pago, Factura factura, string empresa, string ruc, string direccion)
        {
            string rutaRecibo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                             $"Recibo_{pago.PagoID}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            using (PdfWriter writer = new PdfWriter(rutaRecibo))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);

                    // Encabezado del recibo
                    document.Add(new Paragraph("RECIBO DE PAGO")
                        .SetFontSize(18)
                        .SetBold()
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Add(new Paragraph($"Empresa: {empresa}\nRUC: {ruc}\nDirección: {direccion}")
                        .SetFontSize(12));

                    document.Add(new Paragraph($"Fecha del recibo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .SetFontSize(10));

                    document.Add(new Paragraph($"Factura ID: {factura.IdFactura}")
                        .SetFontSize(12)
                        .SetBold());

                    document.Add(new Paragraph($"Cliente: {factura.Cliente.Nombre}\nTotal Factura: {factura.Total:C}")
                        .SetFontSize(12));

                    document.Add(new Paragraph("Detalle del Pago:")
                        .SetFontSize(14)
                        .SetBold());

                    Table table = new Table(2).UseAllAvailableWidth();
                    table.AddCell("Fecha de Pago:");
                    table.AddCell(pago.FechaPago.ToString("dd/MM/yyyy"));
                    table.AddCell("Método de Pago:");
                    table.AddCell(pago.MetodoPago);
                    table.AddCell("Monto Pagado:");
                    table.AddCell(pago.Monto.ToString("C"));
                    document.Add(table);

                    document.Add(new Paragraph("\n\nGracias por su pago.")
                        .SetFontSize(10)
                        .SetItalic());

                    document.Close();
                }
            }

            MessageBox.Show($"Recibo generado exitosamente en:\n{rutaRecibo}", "Recibo Generado", MessageBoxButton.OK, MessageBoxImage.Information);

            // Abrir el PDF automáticamente después de generarlo
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(rutaRecibo) { UseShellExecute = true });
        }
    }
}
