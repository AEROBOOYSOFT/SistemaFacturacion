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
                    ActualizarEstadoFactura(facturaId);
                    // Calcular total pagado y saldo pendiente
                    decimal totalPagado = pagos.Sum(p => p.Monto);
                    decimal saldoPendiente = facturaSeleccionada.Total - totalPagado;

                    txtTotalPagado.Text = totalPagado.ToString("F2");
                    txtSaldoPendiente.Text = saldoPendiente.ToString("F2");
                    txtEstadoFactura.Text = saldoPendiente > 0 ? "Pendiente" : "Pagada";
                    txtEstadoFactura.Foreground = saldoPendiente > 0 ? Brushes.Red : Brushes.Green;

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
        private Autenticacion autenticacion = new Autenticacion(); // Instancia de autenticación

        private void btnRegistrarPago_Click(object sender, RoutedEventArgs e)
        {
            // Verifica si se ha seleccionado una factura.
            if (facturaSeleccionada == null)
            {
                MessageBox.Show("Debe buscar una factura primero.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verifica que el monto sea válido.
            if (decimal.TryParse(txtMontoPago.Text, out decimal monto) && monto > 0)
            {
                decimal saldoPendiente = decimal.Parse(txtSaldoPendiente.Text);

                // Verifica que el monto no sea mayor al saldo pendiente.
                if (monto > saldoPendiente)
                {
                    MessageBox.Show($"El monto no puede ser mayor al saldo pendiente ({saldoPendiente:F2}).", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string metodoPago = cmbMetodoPago.Text;
                // Verifica que se haya seleccionado un método de pago.
                if (string.IsNullOrEmpty(metodoPago))
                {
                    MessageBox.Show("Seleccione un método de pago.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Verifica si el usuario está autenticado.
                if (autenticacion.UsuarioAutenticado == null)
                {
                    MessageBox.Show("No se ha encontrado un usuario autenticado.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Obtiene el UsuarioID del usuario autenticado.
                int usuarioID = autenticacion.UsuarioAutenticado.UsuarioID; // Aquí accedes al UsuarioID

                // Crea el objeto de pago.
                Pago nuevoPago = new Pago
                {
                    FacturaID = facturaSeleccionada.IdFactura,
                    FechaPago = DateTime.Now,
                    Monto = monto,
                    MetodoPago = metodoPago,
                    UsuarioID = usuarioID // Asigna el UsuarioID del usuario autenticado
                };

                // Inserta el nuevo pago en la base de datos.
                PagoCRUD.InsertarPago(nuevoPago);
                // Obtener datos de configuración para el recibo
                var configuracion = ConfiguracionCRUD.ObtenerConfiguracion();
                GeneradorRecibos.GenerarReciboPDF(nuevoPago, facturaSeleccionada, configuracion.NombreEmpresa, configuracion.Ruc, configuracion.Direccion);
                MessageBox.Show("Pago registrado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Actualiza la lista de pagos o realiza otras acciones necesarias.
                btnBuscar_Click(sender, e);
                ActualizarEstadoFactura(int.Parse(txtFacturaID.Text));
            }
            else
            {
                MessageBox.Show("Ingrese un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            string carpeta = @"C:\Recibos"; // Ruta donde quieres guardar el archivo
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta); // Crear la carpeta si no existe
            }
            return System.IO.Path.Combine(carpeta, $"Recibo_Pago_{pago.FacturaID}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }
    

        private void AgregarTitulo(iText.Layout.Document pdf)
        {
            var titulo = new iText.Layout.Element.Paragraph("RECIBO DE PAGO")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(16);
                
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


        /* metodo 1
         * private void btnEliminarPago_Click(object sender, RoutedEventArgs e)
          {
              if (dgPagos.SelectedItem == null)
              {
                  MessageBox.Show("Seleccione un pago para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                  return;
              }

              var button = sender as Button;
              int idPago = (int)button.Tag;

              if (MessageBox.Show("¿Está seguro de que desea eliminar este pago?", "Confirmar eliminación",
                  MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
              {
                  try
                  {
                      PagoCRUD.EliminarPago(idPago);
                      MessageBox.Show("Pago eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                      // Actualizar la lista de pagos
                      btnBuscar_Click(sender, e);
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show("Error al eliminar el pago: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                  }
              }
          */
        private void btnEliminarPago_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagos.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un pago a eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pagoSeleccionado = (Pago)dgPagos.SelectedItem;

            MessageBoxResult resultado = MessageBox.Show(
                $"¿Está seguro de que desea eliminar el pago de {pagoSeleccionado.Monto:C} realizado el {pagoSeleccionado.FechaPago:dd/MM/yyyy}?",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (resultado == MessageBoxResult.Yes)
            {
                bool eliminado = PagoCRUD.EliminarPago(pagoSeleccionado.PagoID);

                if (eliminado)
                {
                    MessageBox.Show("Pago eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Actualizar la lista de pagos después de eliminar
                    btnBuscar_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el pago. Intente de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            if (facturaSeleccionada == null)
            {
                MessageBox.Show("Debe buscar una factura primero.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pagos = PagoCRUD.ObtenerPagosPorFactura(facturaSeleccionada.IdFactura)
                .Where(p => p.FechaPago >= dpFechaInicio.SelectedDate && p.FechaPago <= dpFechaFin.SelectedDate)
                .ToList();

            dgPagos.ItemsSource = pagos;
        }
        private void btnPagarTotal_Click(object sender, RoutedEventArgs e)
        {
            txtMontoPago.Text = txtSaldoPendiente.Text;
        }

        private void btnBuscarPagos_Click(object sender, RoutedEventArgs e)
        {
            DateTime? fechaInicio = dpBusquedaInicio.SelectedDate;
            DateTime? fechaFin = dpBusquedaFin.SelectedDate;
            string metodoPago = ((ComboBoxItem)cmbMetodoBusqueda.SelectedItem).Content.ToString();

            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio > fechaFin)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor que la fecha de fin.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pagosFiltrados = PagoCRUD.BuscarPagos(fechaInicio, fechaFin, metodoPago);

            if (pagosFiltrados.Count > 0)
            {
                dgPagos.ItemsSource = pagosFiltrados;
            }
            else
            {
                MessageBox.Show("No se encontraron pagos con los criterios especificados.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                dgPagos.ItemsSource = null;
            }
        }

        private void ActualizarEstadoFactura(int facturaID)
        {
            string estado = Facturacrud.ObtenerEstadoFactura(facturaID);
            txtEstadoFactura.Text = estado;

            // Cambiar el color visual según el estado
            if (estado == "Pagada")
            {
                estadoFacturaBorder.Background = new SolidColorBrush(Colors.Green);
                txtEstadoFactura.Text = "Pagada";
            }
            else
            {
                estadoFacturaBorder.Background = new SolidColorBrush(Colors.Orange);
                txtEstadoFactura.Text = "Pendiente";
            }
        }
        private void btnSugerirPagoTotal_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtFacturaID.Text, out int facturaID))
            {
                decimal saldoPendiente = Facturacrud.ObtenerSaldoPendiente(facturaID);

                if (saldoPendiente > 0)
                {
                    txtMontoPago.Text = saldoPendiente.ToString("N2");
                }
                else
                {
                    MessageBox.Show("No hay saldo pendiente en esta factura.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Ingrese un ID de factura válido.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
