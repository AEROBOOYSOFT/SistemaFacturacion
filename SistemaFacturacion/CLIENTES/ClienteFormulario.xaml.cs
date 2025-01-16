using SistemaFacturacion.Clases;
using System;
using System.Windows;
using SistemaFacturacion.Clases_crud; // Asegúrate de tener acceso a ClienteService

namespace SistemaFacturacion
{
    public partial class ClienteFormulario : Window
    {
        private Cliente _cliente;
        private Clientecrud _clienteService;

        // Constructor para crear un nuevo cliente
        public ClienteFormulario()
        {
            InitializeComponent();
            _cliente = new Cliente();  // Crear un nuevo cliente vacío
            DataContext = _cliente;   // Establecer el cliente vacío como contexto de datos
            _clienteService = new Clientecrud();  // Inicializar el servicio
        }

        // Constructor para editar un cliente existente
        public ClienteFormulario(Cliente cliente)
        {
            InitializeComponent();
            _cliente = cliente ?? new Cliente();  // Si el cliente es null, crea uno vacío
            DataContext = _cliente;  // Vincula el cliente al formulario
            _clienteService = new Clientecrud();  // Inicializar el servicio
        }

        // Guardar los datos del cliente (crear o actualizar)
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verifica que el campo Nombre no esté vacío o tenga solo espacios
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El campo 'Nombre' es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Asigna los valores de los TextBox al objeto _cliente
                _cliente.Nombre = txtNombre.Text.Trim();  // Usa Trim() para eliminar posibles espacios en blanco
                _cliente.Cedula = txtCedula.Text.Trim();
                _cliente.Direccion = txtDireccion.Text.Trim();
                _cliente.Telefono = txtTelefono.Text.Trim();
                _cliente.Email = txtEmail.Text.Trim();

                // Verifica si el cliente es nuevo o existente
                if (_cliente.IdCliente == 0) // Si el IdCliente es 0, significa que es un cliente nuevo
                {
                    _clienteService.CrearCliente(_cliente); // Crear nuevo cliente
                }
                else
                {
                    _clienteService.ActualizarCliente(_cliente); // Actualizar cliente existente
                }

                DialogResult = true;  // Indica que el usuario guardó los cambios
                Close();  // Cierra la ventana
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el cliente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Cancelar la operación y cerrar la ventana sin guardar
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;  // Indica que no se guardaron los cambios
            Close();  // Cierra la ventana
        }
    }
}
