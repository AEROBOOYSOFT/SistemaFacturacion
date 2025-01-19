using SistemaFacturacion.CLIENTES;
using SistemaFacturacion.FACTURACION;
using SistemaFacturacion.PRODUCTOS;
using SistemaFacturacion.USUARIOS.CONFIGURACION;  // Asegúrate de que este espacio de nombres esté incluido
using SistemaFacturacion.USUARIOS.GESTIONES_CRUD;
using System;
using System.Windows;

namespace SistemaFacturacion
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Evento para abrir ClienteFormulario
        private void AbrirClienteFormulario_Click(object sender, RoutedEventArgs e)
        {
            var ventanaCliente = new ClienteFormulario();
            ventanaCliente.ShowDialog();  // Mostrar la ventana como un cuadro de diálogo modal
        }

        // Evento para abrir la ventana de consulta de clientes
        private void AbrirConsultaClientes_Click(object sender, RoutedEventArgs e)
        {
            ConsultaClientes ventanaClientes = new ConsultaClientes();
            ventanaClientes.ShowDialog();
        }

        // Evento para abrir la ventana de consulta de productos
        private void AbrirConsultaProductos_Click(object sender, RoutedEventArgs e)
        {
            ConsultaProductos ventanaProductos = new ConsultaProductos();
            ventanaProductos.ShowDialog();
        }

        private void AbrirRegistroFacturas_Click(object sender, RoutedEventArgs e)
        {
            var registroFacturas = new RegistroFacturas();
            registroFacturas.Show();
        }

        // Método para abrir la ventana de consulta de facturas
        private void AbrirConsultaFacturas_Click(object sender, RoutedEventArgs e)
        {
            var consultaFacturas = new ConsultaFacturas();
            consultaFacturas.ShowDialog();
        }

        private void AbrirGestionUsuarios_Click(object sender, RoutedEventArgs e)
        {
            var gestionUsuarios = new SistemaFacturacion.USUARIOS.GESTIONES_CRUD.GestionUsuarios();
            gestionUsuarios.Show();
        }

        private void AbrirGestionRoles_Click(object sender, RoutedEventArgs e)
        {
            var gestionRoles = new SistemaFacturacion.USUARIOS.GESTIONES_CRUD.GestionRoles();
            gestionRoles.Show();
        }

        private void AbrirGestionPermisos_Click(object sender, RoutedEventArgs e)
        {
            var gestionPermisos = new SistemaFacturacion.USUARIOS.GESTIONES_CRUD.GestionPermisos();
            gestionPermisos.Show();
        }

        private void AbrirConfiguracionRoles_Click(object sender, RoutedEventArgs e)
        {
            var configuracionRoles = new SistemaFacturacion.USUARIOS.CONFIGURACION.ConfiguracionRoles();
            configuracionRoles.Show();
        }

        private void AbrirAsignarPermisos_Click(object sender, RoutedEventArgs e)
        {
            var asignarPermisos = new SistemaFacturacion.USUARIOS.CONFIGURACION.AsignarPermisosARoles();
            asignarPermisos.Show();
        }
        private void AbrirConsultaMovimientosInventario_Click(object sender, RoutedEventArgs e)
        {
            // Crear una nueva instancia de la ventana de consulta de movimientos de inventario
            var ventanaConsultaInventario = new INVENTARIO.ConsultaMovimientosInventario();
            ventanaConsultaInventario.Show(); // Mostrar la ventana
        }

    }
}

