using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaFacturacion.Clases;
using SistemaFacturacion.Clases_crud;

using System.Windows;
using System.Windows.Controls;

namespace SistemaFacturacion.CLIENTES
{
    /// <summary>
    /// Lógica de interacción para ConsultaClientes.xaml
    /// </summary>
    public partial class ConsultaClientes : Window
    {
        private Clientecrud _clienteService;

        public ConsultaClientes()
        {
            InitializeComponent();
            _clienteService = new Clientecrud();
            CargarClientes();
        }

        // Método para cargar la lista de clientes en el DataGrid
        private void CargarClientes()
        {
            dgClientes.ItemsSource = _clienteService.ObtenerClientes();
        }

        // Evento de clic para agregar un nuevo cliente
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new ClienteFormulario(); // Ventana de formulario de cliente
            if (ventana.ShowDialog() == true)  // Si la ventana se cierra con éxito
            {
                CargarClientes();  // Recargar la lista de clientes
            }
        }

        // Evento de clic para editar un cliente seleccionado
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el cliente seleccionado
            var clienteSeleccionado = (Cliente)dgClientes.SelectedItem;

            // Verificar que un cliente ha sido seleccionado
            if (clienteSeleccionado == null)
            {
                MessageBox.Show("Seleccione un cliente para editar.");
                return;
            }

            // Crear una nueva instancia del formulario de cliente y pasar el cliente seleccionado
            var ventana = new ClienteFormulario(clienteSeleccionado);

            // Abrir el formulario y esperar a que se cierre
            if (ventana.ShowDialog() == true)
            {
                CargarClientes();  // Recargar los clientes después de editar
            }
        }

        // Evento de clic para eliminar un cliente seleccionado
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var clienteSeleccionado = (Cliente)dgClientes.SelectedItem;  // Obtener el cliente seleccionado
            if (clienteSeleccionado == null)
            {
                MessageBox.Show("Seleccione un cliente para eliminar.");
                return;
            }

            var resultado = MessageBox.Show($"¿Está seguro de que desea eliminar al cliente {clienteSeleccionado.Nombre}?",
                                            "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    _clienteService.EliminarCliente(clienteSeleccionado.IdCliente);  // Eliminar cliente
                    CargarClientes();  // Recargar la lista de clientes
                    MessageBox.Show("Cliente eliminado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error al eliminar el cliente: {ex.Message}");  // Manejo de errores
                }
            }
        }

        private void TxtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Buscar cliente...")
            {
                textBox.Text = string.Empty;
            }
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Buscar cliente...")
            {
                textBox.Text = string.Empty;
            }
        }
    }
}
