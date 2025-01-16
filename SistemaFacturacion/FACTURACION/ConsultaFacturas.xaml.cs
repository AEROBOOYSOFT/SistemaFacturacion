using SistemaFacturacion.CLASES_CRUD;
using SistemaFacturacion.Clases;
using System;
using System.Windows;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Documents;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;

using Paragraph = iTextSharp.text.Paragraph;
using System.Drawing;

namespace SistemaFacturacion.FACTURACION
{
    /// <summary>
    /// Lógica de interacción para ConsultaFacturas.xaml
    /// </summary>
    public partial class ConsultaFacturas : Window
    {
        public ConsultaFacturas()
        {
            InitializeComponent();
            CargarFacturas();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarFacturas();
        }

        private void CargarFacturas()
        {
            var facturas = consultafacturacrud.ObtenerFacturas();
            dgFacturas.ItemsSource = facturas;
        }

        private void VerDetalles_Click(object sender, RoutedEventArgs e)
        {
            if (dgFacturas.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una factura para ver los detalles.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var facturaSeleccionada = (Factura)dgFacturas.SelectedItem;

            // Llamar a ObtenerFacturaConCliente para obtener la factura con el cliente asociado
            var facturaConCliente = Facturacrud.ObtenerFacturaConCliente(facturaSeleccionada.IdFactura);

            if (facturaConCliente != null)
            {
                // Pasar la factura con el cliente a la ventana de detalles
                var ventanaDetalles = new DetalleFacturaVentana(facturaConCliente);
                ventanaDetalles.ShowDialog();
            }
            else
            {
                MessageBox.Show("No se pudo obtener la factura con el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AnularFactura_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si hay una factura seleccionada
            if (dgFacturas.SelectedItem is Factura facturaSeleccionada)
            {
                // Confirmar anulación
                var resultado = MessageBox.Show("¿Estás seguro de que deseas anular esta factura?",
                                                "Confirmar Anulación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    // Llamar al método AnularFactura del CRUD
                    consultafacturacrud.AnularFactura(facturaSeleccionada.IdFactura);

                    // Refrescar la lista de facturas
                    CargarFacturas();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una factura para anular.", "Anular Factura", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportarReporte_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar un cuadro de diálogo para seleccionar el formato de exportación
            var resultado = MessageBox.Show("Seleccione el formato de exportación:\n\nSí: PDF\nNo: Excel\nCancelar: Imprimir",
                                            "Exportar Reporte",
                                            MessageBoxButton.YesNoCancel,
                                            MessageBoxImage.Question);

            switch (resultado)
            {
                case MessageBoxResult.Yes:
                    // Exportar a PDF
                    ExportarAPDF();
                    break;

                case MessageBoxResult.No:
                    // Exportar a Excel
                    ExportarAExcel();
                    break;

                case MessageBoxResult.Cancel:
                    // Imprimir reporte
                    ImprimirReporte();
                    break;
            }
        }
        private void ExportarAPDF()
        {
            if (dgFacturas.SelectedItem is Factura facturaSeleccionada)
            {
                // Configurar el cuadro de diálogo para guardar el archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Archivo PDF|*.pdf",
                    Title = "Guardar Reporte de Factura",
                    FileName = $"Factura_{facturaSeleccionada.IdFactura}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string rutaArchivo = saveFileDialog.FileName;

                    // Crear el documento PDF
                    Document documento = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter.GetInstance(documento, new FileStream(rutaArchivo, FileMode.Create));

                    documento.Open();

                    // Encabezado
                    documento.Add(new Paragraph("Reporte de Factura"));
                    documento.Add(new Paragraph($"Factura ID: {facturaSeleccionada.IdFactura}"));
                    documento.Add(new Paragraph($"Cliente: {facturaSeleccionada.Cliente.Nombre}"));
                    documento.Add(new Paragraph($"Fecha: {facturaSeleccionada.Fecha:dd/MM/yyyy}"));
                    documento.Add(new Paragraph($"Total: {facturaSeleccionada.Total:C}"));
                    documento.Add(new Paragraph("---------------------------------------------------"));

                    // Detalle de la factura
                    PdfPTable tabla = new PdfPTable(4);
                    tabla.AddCell("Producto");
                    tabla.AddCell("Cantidad");
                    tabla.AddCell("Precio Unitario");
                    tabla.AddCell("Subtotal");

                    foreach (var detalle in facturaSeleccionada.Detalles)
                    {
                        tabla.AddCell(detalle.Producto.Nombre);
                        tabla.AddCell(detalle.Cantidad.ToString());
                        tabla.AddCell(detalle.PrecioUnitario.ToString("C"));
                        tabla.AddCell(detalle.Subtotal.ToString("C"));
                    }

                    documento.Add(tabla);

                    documento.Close();

                    MessageBox.Show("Reporte PDF generado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una factura para exportar.", "Exportar PDF", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExportarAExcel()
        {
            if (dgFacturas.SelectedItem is Factura facturaSeleccionada)
            {
                try
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Archivo Excel|*.xlsx",
                        Title = "Guardar Reporte de Factura",
                        FileName = $"Factura_{facturaSeleccionada.IdFactura}.xlsx"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string rutaArchivo = saveFileDialog.FileName;

                        // Crear una nueva aplicación de Excel
                        var excelApp = new Microsoft.Office.Interop.Excel.Application();
                        var workbook = excelApp.Workbooks.Add();
                        var worksheet = workbook.Sheets[1] as Microsoft.Office.Interop.Excel.Worksheet;

                        // Encabezados
                        worksheet.Cells[1, 1] = "Reporte de Factura";
                        worksheet.Cells[2, 1] = $"Factura ID: {facturaSeleccionada.IdFactura}";
                        worksheet.Cells[3, 1] = $"Cliente: {facturaSeleccionada.Cliente.Nombre}";
                        worksheet.Cells[4, 1] = $"Fecha: {facturaSeleccionada.Fecha:dd/MM/yyyy}";
                        worksheet.Cells[5, 1] = $"Total: {facturaSeleccionada.Total:C}";

                        // Detalle de la factura
                        worksheet.Cells[7, 1] = "Producto";
                        worksheet.Cells[7, 2] = "Cantidad";
                        worksheet.Cells[7, 3] = "Precio Unitario";
                        worksheet.Cells[7, 4] = "Subtotal";

                        int fila = 8;
                        foreach (var detalle in facturaSeleccionada.Detalles)
                        {
                            worksheet.Cells[fila, 1] = detalle.Producto.Nombre;
                            worksheet.Cells[fila, 2] = detalle.Cantidad;
                            worksheet.Cells[fila, 3] = detalle.PrecioUnitario;
                            worksheet.Cells[fila, 4] = detalle.Subtotal;
                            fila++;
                        }

                        workbook.SaveAs(rutaArchivo);
                        workbook.Close();
                        excelApp.Quit();

                        MessageBox.Show("Reporte Excel generado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al exportar a Excel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una factura para exportar.", "Exportar Excel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ImprimirReporte()
        {
            if (dgFacturas.SelectedItem is Factura facturaSeleccionada)
            {
                // Configuración de la impresión
                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += (sender, e) => PrintPage(sender, e, facturaSeleccionada);

                // Mostrar cuadro de diálogo de impresión
                System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDocument.Print();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una factura para imprimir.", "Imprimir", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    private void PrintPage(object sender, PrintPageEventArgs e, Factura facturaSeleccionada) 
        {
    float yPos = 20;
    float leftMargin = e.MarginBounds.Left;
    System.Drawing.Font font = new System.Drawing.Font("Arial", 10);
    
    // Encabezado
    e.Graphics.DrawString("Reporte de Factura", font, Brushes.Black, leftMargin, yPos);
    yPos += 20;
    e.Graphics.DrawString($"Factura ID: {facturaSeleccionada.IdFactura}", font, Brushes.Black, leftMargin, yPos);
    yPos += 20;
    e.Graphics.DrawString($"Cliente: {facturaSeleccionada.Cliente.Nombre}", font, Brushes.Black, leftMargin, yPos);
    yPos += 20;
    e.Graphics.DrawString($"Fecha: {facturaSeleccionada.Fecha:dd/MM/yyyy}", font, Brushes.Black, leftMargin, yPos);
    yPos += 20;
    e.Graphics.DrawString($"Total: {facturaSeleccionada.Total:C}", font, Brushes.Black, leftMargin, yPos);
    yPos += 20;

    // Detalles de la factura (tabla)
    e.Graphics.DrawString("---------------------------------------------------", font, Brushes.Black, leftMargin, yPos);
    yPos += 20;

    // Títulos de las columnas
    e.Graphics.DrawString("Producto", font, Brushes.Black, leftMargin, yPos);
    e.Graphics.DrawString("Cantidad", font, Brushes.Black, leftMargin + 200, yPos);
    e.Graphics.DrawString("Precio Unitario", font, Brushes.Black, leftMargin + 300, yPos);
    e.Graphics.DrawString("Subtotal", font, Brushes.Black, leftMargin + 400, yPos);
    yPos += 20;

    // Detalles de los productos
    foreach (var detalle in facturaSeleccionada.Detalles)
    {
        e.Graphics.DrawString(detalle.Producto.Nombre, font, Brushes.Black, leftMargin, yPos);
        e.Graphics.DrawString(detalle.Cantidad.ToString(), font, Brushes.Black, leftMargin + 200, yPos);
        e.Graphics.DrawString(detalle.PrecioUnitario.ToString("C"), font, Brushes.Black, leftMargin + 300, yPos);
        e.Graphics.DrawString(detalle.Subtotal.ToString("C"), font, Brushes.Black, leftMargin + 400, yPos);
        yPos += 20;
    }

    // Marcar fin de la página
    e.HasMorePages = false;
    }

    }
}
