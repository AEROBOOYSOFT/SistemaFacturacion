using SistemaFacturacion.Clases;
using SistemaFacturacion.CLASES_CRUD;
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
    /// Lógica de interacción para RegistroFacturas.xaml
    /// </summary>
    public partial class RegistroFacturas : Window
    {
        private List<DetalleFactura> detalleFactura;
        private decimal total;
        public RegistroFacturas()
        {
            InitializeComponent();
            detalleFactura = new List<DetalleFactura>();
            total = 0;
            CargarClientes();
            CargarProductos();

        }
        private void CargarClientes()
        {
            var clientes = Facturacrud.ObtenerClientes();
            cmbClientes.ItemsSource = clientes;
            cmbClientes.DisplayMemberPath = "Nombre";  // esel nombre del cliente
            cmbClientes.SelectedValuePath = "IdCliente"; // Usar el IdCliente como valor seleccionado
        }

        private void CargarProductos()
        {
            var productos = Facturacrud.ObtenerProductos();
            cmbProductos.ItemsSource = productos;
            cmbProductos.DisplayMemberPath = "Nombre";  // es el nombre del producto
            cmbProductos.SelectedValuePath = "IdProducto"; // Usar el IdProducto como valor seleccionado
        }

        private void btnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProductos.SelectedItem == null || string.IsNullOrEmpty(txtCantidad.Text))
            {
                MessageBox.Show("Debe seleccionar un producto y una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var producto = (Producto)cmbProductos.SelectedItem;
            int cantidad = int.Parse(txtCantidad.Text);

            if (cantidad <= 0 || cantidad > producto.Stock)
            {
                MessageBox.Show("La cantidad debe ser mayor a 0 y menor o igual al stock disponible.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal subtotal = producto.Precio * cantidad;

            detalleFactura.Add(new DetalleFactura
            {
                IdProducto = producto.IdProducto,
                Producto = producto,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            });

            dgDetalleFactura.ItemsSource = null;
            dgDetalleFactura.ItemsSource = detalleFactura;

            total += subtotal;
            txtTotal.Text = total.ToString("F2");
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClientes.SelectedItem == null || detalleFactura.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un cliente y agregar al menos un producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Obtener el cliente seleccionado y su nombre
            var clienteSeleccionado = (Cliente)cmbClientes.SelectedItem;
            string nombreCliente = clienteSeleccionado.Nombre;

            var factura = new Factura
            {
                IdCliente = (int)cmbClientes.SelectedValue,
                Fecha = DateTime.Now,
                Total = total,
                Detalles = detalleFactura,
                Cliente = clienteSeleccionado  // Asignar el objeto cliente completo
            };

            try
            {
                // Guardar la factura, incluyendo el nombre del cliente
                Facturacrud.GuardarFactura(factura);
                MessageBox.Show("Factura guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la factura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            cmbClientes.SelectedItem = null;
            cmbProductos.SelectedItem = null;
            txtCantidad.Text = string.Empty;
            detalleFactura.Clear();
            dgDetalleFactura.ItemsSource = null;
            txtTotal.Text = "0.00";
            total = 0;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}