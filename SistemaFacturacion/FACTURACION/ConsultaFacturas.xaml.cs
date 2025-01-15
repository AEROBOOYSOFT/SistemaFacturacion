using SistemaFacturacion.CLASES_CRUD;
using SistemaFacturacion.Clases;
using System;
using System.Windows;

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
            MessageBox.Show("Funcionalidad de exportación a PDF en desarrollo.", "Exportar a PDF", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportarAExcel()
        {
            MessageBox.Show("Funcionalidad de exportación a Excel en desarrollo.", "Exportar a Excel", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ImprimirReporte()
        {
            MessageBox.Show("Funcionalidad de impresión en desarrollo.", "Imprimir", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
