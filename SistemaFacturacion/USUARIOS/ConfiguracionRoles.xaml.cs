using SistemaFacturacion.CLASES;
using SistemaFacturacion.CLASES_CRUD;
using System.Windows;

namespace SistemaFacturacion.USUARIOS
{
    public partial class ConfiguracionRoles : Window
    {
        private Servicioderoles _rolService;
        private PermisosService _permisosService;
        private Rol _rolSeleccionado;

        public ConfiguracionRoles()
        {
            InitializeComponent();
            _rolService = new Servicioderoles();
            _permisosService = new PermisosService();
            CargarRoles();
        }

        // Cargar lista de roles
        private void CargarRoles()
        {
            var roles = _rolService.ObtenerTodosLosRoles();
            lbRoles.ItemsSource = roles;
            lbRoles.DisplayMemberPath = "Nombre";
        }

        // Cargar permisos del rol seleccionado
        private void LbRoles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lbRoles.SelectedItem is Rol rolSeleccionado)
            {
                _rolSeleccionado = rolSeleccionado;
                var permisos = _permisosService.ObtenerPermisosPorRol(rolSeleccionado.RolID);
                lbPermisos.ItemsSource = permisos;
            }
        }

        // Agregar un nuevo rol
        private void BtnAgregarRol_Click(object sender, RoutedEventArgs e)
        {
            var nuevoRol = new Rol { NombreRol = "Nuevo Rol" }; // Cambiar por una ventana para ingresar el nombre
            _rolService.GuardarRol(nuevoRol);
            CargarRoles();
        }

        // Asignar permiso al rol
        private void BtnAsignarPermiso_Click(object sender, RoutedEventArgs e)
        {
            if (_rolSeleccionado == null)
            {
                MessageBox.Show("Selecciona un rol primero.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var permiso = new Permiso { NombrePermiso = "Nuevo Permiso" }; // Cambiar por una selección real
            _rolService.AsignarPermisoARol(_rolSeleccionado.RolID, permiso.PermisoID);
            LbRoles_SelectionChanged(null, null);
        }

        // Quitar permiso del rol
        private void BtnQuitarPermiso_Click(object sender, RoutedEventArgs e)
        {
            if (lbPermisos.SelectedItem is Permiso permisoSeleccionado)
            {
                _rolService.QuitarPermisoDeRol(_rolSeleccionado.RolID, permisoSeleccionado.PermisoID);
                LbRoles_SelectionChanged(null, null);
            }
        }
    }
}
