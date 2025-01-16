using System;
using System.Data;
using System.Windows;
using SistemaFacturacion.Clases_crud;

namespace SistemaFacturacion.PRODUCTOS
{
    public partial class ConsultaProductos : Window
    {
        private readonly Productocrud _productService;

        public ConsultaProductos()
        {
            InitializeComponent();
            _productService = new Productocrud();  // Inicialización de ProductService
            CargarProductos();
        }

        // Método para cargar los productos en el DataGrid
        private void CargarProductos()
        {
            DataTable productos = _productService.ObtenerProductos();
            dgProductos.ItemsSource = productos.DefaultView;
        }

        // Evento para el botón Agregar
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            ProductoFormulario agregarProducto = new ProductoFormulario();
            if (agregarProducto.ShowDialog() == true)
            {
                CargarProductos();
            }
        }

        // Evento para el botón Editar
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem != null)
            {
                DataRowView selectedRow = dgProductos.SelectedItem as DataRowView;
                int productoId = (int)selectedRow["ProductoID"];

                ProductoFormulario editarProducto = new ProductoFormulario(productoId);
                if (editarProducto.ShowDialog() == true)
                {
                    CargarProductos();
                }
            }
            else
            {
                MessageBox.Show("Selecciona un producto para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Evento para el botón Eliminar
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem != null)
            {
                DataRowView selectedRow = dgProductos.SelectedItem as DataRowView;
                int productoId = (int)selectedRow["ProductoID"];

                MessageBoxResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar este producto?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _productService.EliminarProducto(productoId);
                    CargarProductos();
                }
            }
            else
            {
                MessageBox.Show("Selecciona un producto para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

