using SistemaFacturacion.CLASES;
using SistemaFacturacion.CLASES_CRUD;
using System;
using System.Windows;

namespace SistemaFacturacion.USUARIOS
{
    public partial class GestionPermisos : Window
    {
        private readonly PermisosService _permisosService;

        public GestionPermisos()
        {
            InitializeComponent();
            _permisosService = new PermisosService();
            CargarPermisos();
        }

        // Carga la lista de permisos en el DataGrid
        private void CargarPermisos()
        {
            try
            {
                dgPermisos.ItemsSource = _permisosService.ObtenerTodosLosPermisos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar permisos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Agregar un nuevo permiso
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var nuevoPermiso = new Permiso
            {
                NombrePermiso = "Nuevo Permiso",
                Descripcion = "Descripción del nuevo permiso"
            };

            try
            {
                _permisosService.GuardarPermiso(nuevoPermiso);
                MessageBox.Show("Permiso agregado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarPermisos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar permiso: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Editar un permiso seleccionado
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPermisos.SelectedItem is Permiso permisoSeleccionado)
            {
                permisoSeleccionado.NombrePermiso = "Permiso Editado"; // Ejemplo: Modificar nombre
                permisoSeleccionado.Descripcion = "Descripción editada"; // Ejemplo: Modificar descripción

                try
                {
                    _permisosService.ActualizarPermiso(permisoSeleccionado);
                    MessageBox.Show("Permiso actualizado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarPermisos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar permiso: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un permiso para editar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Eliminar un permiso seleccionado
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPermisos.SelectedItem is Permiso permisoSeleccionado)
            {
                var confirmacion = MessageBox.Show("¿Estás seguro de eliminar este permiso?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmacion == MessageBoxResult.Yes)
                {
                    try
                    {
                        _permisosService.EliminarPermiso(permisoSeleccionado.PermisoID);
                        MessageBox.Show("Permiso eliminado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarPermisos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar permiso: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un permiso para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
