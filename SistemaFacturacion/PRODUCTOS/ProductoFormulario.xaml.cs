using System;
using System.Data;
using System.Windows;
using SistemaFacturacion.Clases;
using SistemaFacturacion.Clases_crud;

namespace SistemaFacturacion.PRODUCTOS
{
    public partial class ProductoFormulario : Window
    {
        private readonly Productocrud _productService;
        private int _productoId;

        // Propiedad para almacenar el producto seleccionado
        private Producto ProductoSeleccionado { get; set; }

        // Constructor para editar un producto
        public ProductoFormulario(int productoId)
        {
            InitializeComponent();
            _productService = new Productocrud();
            _productoId = productoId;

            ProductoSeleccionado = new Producto();  // Crear la instancia de Producto
            this.DataContext = ProductoSeleccionado; // Establecer el DataContext antes de cargar los datos

            // Cargar el producto desde el servicio
            CargarProducto();
        }

        // Constructor para agregar un producto
        public ProductoFormulario()
        {
            InitializeComponent();
            _productService = new Productocrud();  // Inicialización de ProductService
            ProductoSeleccionado = new Producto();  // Crear una nueva instancia de Producto
            this.DataContext = ProductoSeleccionado; // Establecer el DataContext
        }

        // Método para cargar el producto en el formulario
        private void CargarProducto()
        {
            // Obtener los datos del producto desde el servicio
            DataRow producto = _productService.ObtenerProductoPorId(_productoId);

            if (producto != null)
            {
                // Crear una nueva instancia de Producto y asignar los valores
                ProductoSeleccionado = new Producto
                {
                    IdProducto = _productoId,
                    Nombre = producto["Nombre"].ToString(),
                    Descripcion = producto["Descripcion"].ToString(),
                    Precio = Convert.ToDecimal(producto["Precio"]),
                    Stock = Convert.ToInt32(producto["Stock"]),
                    // Asignar directamente el valor de Estado (nullable)
                    Estado = producto["Estado"] as bool?
                };

                // Establecer el DataContext a la instancia de Producto
                this.DataContext = ProductoSeleccionado;
            }
            else
            {
                // Si no se encuentra el producto, mostrar un mensaje de error y cerrar la ventana
                MessageBox.Show("Producto no encontrado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        // Evento para guardar el producto editado o creado
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validar los campos
            if (string.IsNullOrWhiteSpace(ProductoSeleccionado.Nombre) || string.IsNullOrWhiteSpace(ProductoSeleccionado.Descripcion))
            {
                MessageBox.Show("Los campos Nombre y Descripción son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ProductoSeleccionado.Precio <= 0)
            {
                MessageBox.Show("Precio inválido. Asegúrese de que es un número válido mayor a cero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ProductoSeleccionado.Stock < 0)
            {
                MessageBox.Show("Stock inválido. Asegúrese de que es un número válido mayor o igual a cero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Si el producto tiene un IdProducto, es una edición; si no, es una creación
                if (_productoId == 0)  // Si el IdProducto es 0, es una creación
                {
                    _productService.CrearProducto(
                        ProductoSeleccionado.Nombre,
                        ProductoSeleccionado.Descripcion,
                        ProductoSeleccionado.Precio,
                        ProductoSeleccionado.Stock
                    );

                    MessageBox.Show("Producto creado con éxito", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else  // Si el IdProducto ya tiene valor, es una actualización
                {
                    _productService.ActualizarProducto(
                        ProductoSeleccionado.IdProducto,
                        ProductoSeleccionado.Nombre,
                        ProductoSeleccionado.Descripcion,
                        ProductoSeleccionado.Precio,
                        ProductoSeleccionado.Stock,
                        ProductoSeleccionado.Estado ?? false  // Si el estado es null, asignar false por defecto
                    );

                    MessageBox.Show("Producto actualizado con éxito", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;  // Cerrar la ventana con éxito
            }
            catch (Exception ex)
            {
                // Si ocurre un error al guardar los cambios, mostrar un mensaje
                MessageBox.Show($"Error al guardar el producto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Evento para cancelar la edición y cerrar la ventana sin guardar cambios
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;  // Cerrar la ventana sin guardar cambios
        }
    }
}
