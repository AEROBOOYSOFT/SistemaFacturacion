using SistemaFacturacion.Clases;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


using System.Windows.Markup;

namespace SistemaFacturacion.FACTURACION
{
    public partial class DetalleFacturaVentana : Window
    {
        private readonly Factura _facturaSeleccionada;

        public DetalleFacturaVentana(Factura factura)
        {
            InitializeComponent();
            _facturaSeleccionada = factura ?? throw new ArgumentNullException(nameof(factura));
            CargarDetalles();
        }

        private void CargarDetalles()
        {
            try
            {
                // Información básica de la factura
                txtFacturaId.Text = _facturaSeleccionada.IdFactura.ToString();
                txtFecha.Text = _facturaSeleccionada.Fecha.ToString("dd/MM/yyyy HH:mm");
                txtCliente.Text = $"{_facturaSeleccionada.Cliente.Nombre} ({_facturaSeleccionada.Cliente.Cedula})";
                txtTotal.Text = _facturaSeleccionada.Total.ToString("C");

                // Cargar detalles en el DataGrid
                var detallesFactura = new List<DetalleFacturaViewModel>();
                foreach (var detalle in _facturaSeleccionada.Detalles)
                {
                    detallesFactura.Add(new DetalleFacturaViewModel
                    {
                        Producto = detalle.Producto.Nombre,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Subtotal = detalle.Cantidad * detalle.PrecioUnitario
                    });
                }

                dgDetalles.ItemsSource = detallesFactura;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los detalles de la factura: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Configurar el diálogo de impresión
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Crear el documento a imprimir
                    FixedDocument document = CrearDocumentoImpresion();

                    // Imprimir el documento
                    printDialog.PrintDocument(document.DocumentPaginator, "Factura " + _facturaSeleccionada.IdFactura);

                    MessageBox.Show("Documento enviado a imprimir exitosamente.",
                                  "Impresión",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir la factura: {ex.Message}",
                              "Error de Impresión",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private FixedDocument CrearDocumentoImpresion()
        {
            // Crear el documento
            var document = new FixedDocument();
            var pageContent = new PageContent();
            var fixedPage = new FixedPage();

            // Configurar el tamaño de la página (Letter)
            fixedPage.Width = 816; // 8.5 inches * 96 DPI
            fixedPage.Height = 1056; // 11 inches * 96 DPI

            // Añadir el contenido de la factura
            var contenidoFactura = CrearContenidoFactura();
            FixedPage.SetLeft(contenidoFactura, 50);
            FixedPage.SetTop(contenidoFactura, 50);
            fixedPage.Children.Add(contenidoFactura);

            // Ensamblar el documento
            ((IAddChild)pageContent).AddChild(fixedPage);
            document.Pages.Add(pageContent);

            return document;
        }

        private UIElement CrearContenidoFactura()
        {
            // Crear un panel para el contenido de la factura
            var panel = new StackPanel { Margin = new Thickness(20) };

            // Añadir encabezado
            panel.Children.Add(new TextBlock
            {
                Text = "FACTURA",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            });

            // Añadir información básica
            panel.Children.Add(new TextBlock
            {
                Text = $"Factura #: {_facturaSeleccionada.IdFactura}",
                Margin = new Thickness(0, 0, 0, 10)
            });

            panel.Children.Add(new TextBlock
            {
                Text = $"Fecha: {_facturaSeleccionada.Fecha:dd/MM/yyyy HH:mm}",
                Margin = new Thickness(0, 0, 0, 10)
            });

            panel.Children.Add(new TextBlock
            {
                Text = $"Cliente: {_facturaSeleccionada.Cliente.Nombre}",
                Margin = new Thickness(0, 0, 0, 20)
            });

            // Añadir detalles y total
            foreach (var detalle in _facturaSeleccionada.Detalles)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = $"{detalle.Producto.Nombre} x {detalle.Cantidad} = {(detalle.Cantidad * detalle.PrecioUnitario):C}",
                    Margin = new Thickness(0, 0, 0, 5)
                });
            }

            panel.Children.Add(new TextBlock
            {
                Text = $"Total: {_facturaSeleccionada.Total:C}",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 0)
            });

            return panel;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    // ViewModel para los detalles de la factura
    public class DetalleFacturaViewModel
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}