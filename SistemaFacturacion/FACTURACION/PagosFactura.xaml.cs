using SistemaFacturacion.Clases;
using SistemaFacturacion.CLASES;
using SistemaFacturacion.CLASES_CRUD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SistemaFacturacion.FACTURACION
{
    /// <summary>
    /// Lógica de interacción para PagosFactura.xaml
    /// </summary>
    public partial class PagosFactura : Window
    {
        private Factura facturaSeleccionada;
        public PagosFactura()
        {
            InitializeComponent();
        }
        // Buscar factura por ID
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtFacturaID.Text, out int facturaId))
            {
                facturaSeleccionada = Facturacrud.ObtenerFacturaConCliente(facturaId);

                if (facturaSeleccionada != null)
                {
                    txtCliente.Text = facturaSeleccionada.Cliente.Nombre;
                    txtTotalFactura.Text = facturaSeleccionada.Total.ToString("F2");

                    // Cargar pagos de la factura
                    var pagos = PagoCRUD.ObtenerPagosPorFactura(facturaId);
                    dgPagos.ItemsSource = pagos;

                    // Calcular total pagado y saldo pendiente
                    decimal totalPagado = pagos.Sum(p => p.Monto);
                    decimal saldoPendiente = facturaSeleccionada.Total - totalPagado;

                    txtTotalPagado.Text = totalPagado.ToString("F2");
                    txtSaldoPendiente.Text = saldoPendiente.ToString("F2");
                }
                else
                {
                    MessageBox.Show("Factura no encontrada.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Ingrese un ID de factura válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Registrar un nuevo pago
        private void btnRegistrarPago_Click(object sender, RoutedEventArgs e)
        {
            if (facturaSeleccionada == null)
            {
                MessageBox.Show("Debe buscar una factura primero.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (decimal.TryParse(txtMontoPago.Text, out decimal monto) && monto > 0)
            {
                string metodoPago = cmbMetodoPago.Text;

                if (string.IsNullOrEmpty(metodoPago))
                {
                    MessageBox.Show("Seleccione un método de pago.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Pago nuevoPago = new Pago
                {
                    FacturaID = facturaSeleccionada.IdFactura,
                    FechaPago = DateTime.Now,
                    Monto = monto,
                    MetodoPago = metodoPago
                   
                };

                PagoCRUD.InsertarPago(nuevoPago);
                GenerarReciboPDF(nuevoPago);

                MessageBox.Show("Pago registrado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Actualizar la lista de pagos
                btnBuscar_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Ingrese un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            // Validar que el monto ingresado no exceda el saldo pendiente
            if (monto > decimal.Parse(txtSaldoPendiente.Text))
            {
                MessageBox.Show("El monto no puede ser mayor que el saldo pendiente.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar si la factura ya está pagada completamente
            if (decimal.Parse(txtSaldoPendiente.Text) == 0)
            {
                MessageBox.Show("La factura ya ha sido pagada en su totalidad.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

        }
        private void GenerarReciboPDF(Pago pago)
        {
            // Definir el nombre y la ruta del archivo PDF
            string filePath = ObtenerRutaRecibo(pago);

            // Crear el archivo PDF y escribir en él
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                var writer = new iText.Kernel.Pdf.PdfWriter(fs);
                var pdf = new iText.Layout.Document(new iText.Kernel.Pdf.PdfDocument(writer));

                // Agregar título
                AgregarTitulo(pdf);

                // Agregar detalles del pago
                AgregarDetallesPago(pdf, pago);

                // Cerrar el documento PDF
                pdf.Close();
            }

            // Mostrar mensaje de éxito
            MostrarMensajeExito(filePath);
        }

        private string ObtenerRutaRecibo(Pago pago)
        {
            return $"Recibo_Pago_{pago.FacturaID}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        }

        private void AgregarTitulo(iText.Layout.Document pdf)
        {
            var titulo = new iText.Layout.Element.Paragraph("RECIBO DE PAGO")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(16)
                .SetBold();
            pdf.Add(titulo);
        }

        private void AgregarDetallesPago(iText.Layout.Document pdf, Pago pago)
        {
            pdf.Add(new iText.Layout.Element.Paragraph($"Factura ID: {pago.FacturaID}"));
            pdf.Add(new iText.Layout.Element.Paragraph($"Fecha de Pago: {pago.FechaPago.ToString("dd/MM/yyyy")}"));
            pdf.Add(new iText.Layout.Element.Paragraph($"Monto Pagado: {pago.Monto:C}"));
            pdf.Add(new iText.Layout.Element.Paragraph($"Método de Pago: {pago.MetodoPago}"));
        }

        private void MostrarMensajeExito(string filePath)
        {
            MessageBox.Show($"Recibo generado: {filePath}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void btnEliminarPago_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagos.SelectedItem is Pago pagoSeleccionado)
            {
                if (MessageBox.Show("¿Está seguro de eliminar este pago?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    PagoCRUD.EliminarPago(pagoSeleccionado.PagoID);
                    MessageBox.Show("Pago eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnBuscar_Click(sender, e); // Actualizar la lista de pagos
                }
            }
            else
            {
                MessageBox.Show("Seleccione un pago para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
