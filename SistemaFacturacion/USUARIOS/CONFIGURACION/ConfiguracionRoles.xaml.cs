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

namespace SistemaFacturacion.USUARIOS.CONFIGURACION
{
    /// <summary>
    /// Lógica de interacción para ConfiguracionRoles.xaml
    /// </summary>
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
            var nuevoRol = new Rol { Nombre = "Nuevo Rol" }; // Cambiar por una ventana para ingresar el nombre
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

            // Verifica si hay permisos seleccionados
            if (lbPermisos.SelectedItems.Count > 0)
            {
                // Construye una lista de PermisoIDs de los permisos seleccionados
                List<int> permisosIDs = lbPermisos.SelectedItems
                                                .Cast<Permiso>()  // Convierte los elementos seleccionados a tipo Permiso
                                                .Select(p => p.PermisoID)  // Extrae el PermisoID
                                                .ToList();  // Convierte a una lista de IDs

                // Asigna los permisos seleccionados al rol
                _rolService.AsignarPermisosARol(_rolSeleccionado.RolID, permisosIDs);

                // Actualiza la lista de permisos del rol seleccionado
                LbRoles_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show("Selecciona al menos un permiso para asignar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
