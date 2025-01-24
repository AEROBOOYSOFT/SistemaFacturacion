using SistemaFacturacion.Clases;
using System;
using System.Windows;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

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

                    // Definir las fuentes
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont italicFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

                    // Encabezado del recibo
                    document.Add(new Paragraph("RECIBO DE PAGO")
                        .SetFont(boldFont)
                        .SetFontSize(18)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    document.Add(new Paragraph($"Empresa: {empresa}\nRUC: {ruc}\nDirección: {direccion}")
                        .SetFontSize(12));

                    document.Add(new Paragraph($"Fecha del recibo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .SetFontSize(10));

                    document.Add(new Paragraph($"Factura ID: {factura.IdFactura}")
                        .SetFont(boldFont)
                        .SetFontSize(12));

                    document.Add(new Paragraph($"Cliente: {factura.Cliente.Nombre}\nTotal Factura: {factura.Total:C}")
                        .SetFontSize(12));

                    document.Add(new Paragraph("Detalle del Pago:")
                        .SetFont(boldFont)
                        .SetFontSize(14));

                    Table table = new Table(2).UseAllAvailableWidth();
                    table.AddCell("Fecha de Pago:");
                    table.AddCell(pago.FechaPago.ToString("dd/MM/yyyy"));
                    table.AddCell("Método de Pago:");
                    table.AddCell(pago.MetodoPago);
                    table.AddCell("Monto Pagado:");
                    table.AddCell(pago.Monto.ToString("C"));
                    document.Add(table);

                    document.Add(new Paragraph("\n\nGracias por su pago.")
                        .SetFont(italicFont)
                        .SetFontSize(10));

                    document.Close();
                }
            }

            MessageBox.Show($"Recibo generado exitosamente en:\n{rutaRecibo}", "Recibo Generado", MessageBoxButton.OK, MessageBoxImage.Information);

            // Abrir el PDF automáticamente después de generarlo
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(rutaRecibo) { UseShellExecute = true });
        }
    }
}
