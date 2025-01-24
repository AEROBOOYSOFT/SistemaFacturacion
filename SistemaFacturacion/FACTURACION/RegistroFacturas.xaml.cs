using SistemaFacturacion.Clases;
using SistemaFacturacion.CLASES;
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
        private Factura facturaActual;
        private List<DetalleFactura> detalleFactura;
        private decimal total;
        public RegistroFacturas()
        {
            InitializeComponent();
            detalleFactura = new List<DetalleFactura>();
            total = 0;
            CargarClientes();
            CargarProductos();
            facturaActual = new Factura
            {
                Fecha = DateTime.Now,
                Estado = true
            };

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
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0 || cantidad > producto.Stock)
            {
                MessageBox.Show("La cantidad debe ser mayor a 0 y menor o igual al stock disponible.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            detalleFactura.Add(new DetalleFactura
            {
                IdProducto = producto.IdProducto,
                Producto = producto,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio

            });

            dgDetalleFactura.ItemsSource = null;
            dgDetalleFactura.ItemsSource = detalleFactura;

            CalcularTotales();
        }

        private void CalcularTotales()
        {
            decimal subtotal = detalleFactura.Sum(d => d.Cantidad * d.PrecioUnitario);

            decimal tasaImpuesto = ConfiguracionCRUD.ObtenerImpuestoITBIS(); // Asegúrate de que este método funcione correctamente
            decimal impuestos = subtotal * tasaImpuesto / 100; // Convertir tasa a porcentaje
            total = subtotal + impuestos;  // Asegúrate de actualizar la variable 'total'

            // Mostrar en la interfaz
            txtSubtotal.Text = subtotal.ToString("F2");
            txtImpuestos.Text = impuestos.ToString("F2");
            txtTotal.Text = total.ToString("F2");

            // Guardar en la factura actual
            facturaActual.Subtotal = subtotal;
            facturaActual.Impuestos = impuestos;
            facturaActual.Total = total;  // Asegúrate de que facturaActual.Total también se actualice
        }


        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClientes.SelectedItem == null || detalleFactura.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un cliente y agregar al menos un producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar el stock de todos los productos antes de continuar
            foreach (var detalle in detalleFactura)
            {
                if (detalle.Producto.Stock < detalle.Cantidad)
                {
                    MessageBox.Show($"Stock insuficiente para el producto {detalle.Producto.Nombre}. Disponible: {detalle.Producto.Stock}, requerido: {detalle.Cantidad}.",
                                    "Error de Stock", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var clienteSeleccionado = (Cliente)cmbClientes.SelectedItem;

            var factura = new Factura
            {
                IdCliente = (int)cmbClientes.SelectedValue,
                Fecha = DateTime.Now,
                Total = total,  // Aquí ya debería tener el valor correcto
                Detalles = detalleFactura,
                Cliente = clienteSeleccionado,
                Estado = true // Asignar un estado válido (por ejemplo, 1 para activa)
            };
        

            try
            {
                // Guardar la factura
                Facturacrud.GuardarFactura(factura);

                // Actualizar el stock y registrar movimientos de inventario
                foreach (var detalle in detalleFactura)
                {
                    // Actualizar stock del producto
                    detalle.Producto.Stock -= detalle.Cantidad;
                    Facturacrud.ActualizarStock(detalle.Producto.IdProducto, detalle.Producto.Stock);

                    // Registrar movimiento de inventario
                    RegistrarMovimientoInventario(detalle.Producto.IdProducto, detalle.Cantidad, "Salida", $"Venta en factura {factura.IdFactura}");
                }

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
            txtSubtotal.Text = "0";
            txtImpuestos.Text = "0";
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        private void RegistrarMovimientoInventario(int productoId, int cantidad, string tipoMovimiento, string descripcion)
        {
            var movimiento = new MovimientoInventario
            {
                ProductoID = productoId,
                Cantidad = cantidad,
                TipoMovimiento = tipoMovimiento,
                Descripcion = descripcion
            };

            Facturacrud.RegistrarMovimiento(movimiento);
        }

    }
}