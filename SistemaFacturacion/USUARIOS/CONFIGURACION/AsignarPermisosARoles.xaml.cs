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
    /// Lógica de interacción para AsignarPermisosARoles.xaml
    /// </summary>
    /// 
    public partial class AsignarPermisosARoles : Window
    {
        private readonly Servicioderoles _rolService;
    private readonly PermisosService _permisoService;
    private List<PermisoAsignado> _permisosAsignados;
    private Rol _rolSeleccionado;
  
        public AsignarPermisosARoles()
        {
            InitializeComponent();
            _rolService = new Servicioderoles();
            _permisoService = new PermisosService();
            CargarRoles();
        }
        // Cargar los roles en el ComboBox
        private void CargarRoles()
        {
            try
            {
                var roles = _rolService.ObtenerTodosLosRoles();
                cbRoles.ItemsSource = roles;
                cbRoles.DisplayMemberPath = "NombreRol";
                cbRoles.SelectedValuePath = "RolID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Evento al cambiar el rol seleccionado
        private void CbRoles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbRoles.SelectedItem is Rol rolSeleccionado)
            {
                _rolSeleccionado = rolSeleccionado;
                CargarPermisosPorRol(rolSeleccionado.RolID);
            }
        }

        // Cargar permisos asignados y no asignados para el rol seleccionado
        private void CargarPermisosPorRol(int rolID)
        {
            try
            {
                var todosPermisos = _permisoService.ObtenerTodosLosPermisos();
                var permisosAsignados = _permisoService.ObtenerPermisosPorRol(rolID);

                _permisosAsignados = todosPermisos.Select(p => new PermisoAsignado
                {
                    PermisoID = p.PermisoID,
                    NombrePermiso = p.NombrePermiso,
                    Descripcion = p.Descripcion,
                    Asignado = permisosAsignados.Any(pa => pa.PermisoID == p.PermisoID)
                }).ToList();

                dgPermisos.ItemsSource = _permisosAsignados;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar permisos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Guardar los cambios realizados en los permisos asignados al rol
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var permisosAAsignar = _permisosAsignados.Where(p => p.Asignado).Select(p => p.PermisoID).ToList();
                _permisoService.AsignarPermisosARol(_rolSeleccionado.RolID, permisosAAsignar);

                MessageBox.Show("Cambios guardados con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cambios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Cerrar la ventana
        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Clase auxiliar para manejar los permisos en la UI
    public class PermisoAsignado : Permiso
    {
        public bool Asignado { get; set; }
    }
}
