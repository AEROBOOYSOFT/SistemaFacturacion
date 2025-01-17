using SistemaFacturacion.Clases;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para DetalleFacturaVentana.xaml
    /// </summary>
    public partial class DetalleFacturaVentana : Window
    {
        public Factura FacturaSeleccionada { get; set; }
        public DetalleFacturaVentana(Factura factura)
        {
            InitializeComponent();
            CargarDetalles(factura);

        }
        private void CargarDetalles(Factura factura)
        {
            // Mostrar los detalles de la factura en los controles correspondientes
            lblFacturaId.Content = $"Factura ID: {factura.IdFactura}";
            lblCliente.Content = $"Cliente: {factura.Cliente.Nombre}";
            lblTotal.Content = $"Total: {factura.Total:C}";

            // Si necesitas mostrar más detalles, puedes agregarlos aquí
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}