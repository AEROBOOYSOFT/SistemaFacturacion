using SistemaFacturacion.CLASES;
using SistemaFacturacion.CLASES_CRUD;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace SistemaFacturacion.USUARIOS
{
    /// <summary>
    /// Lógica de interacción para GestionRoles.xaml
    /// </summary>
    public partial class GestionRoles : Window
    {
        private Servicioderoles _Servicioderoles;
        public GestionRoles()
        {
            InitializeComponent();
            _Servicioderoles = new Servicioderoles();
            CargarRoles();
        }
        private void CargarRoles()
        {
            var roles = _Servicioderoles.ObtenerTodosLosRoles();
            dgRoles.ItemsSource = roles;
        }
        private void BtnGuardarRol_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreRol.Text))
            {
                MessageBox.Show("El nombre del rol es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var nuevoRol = new Rol
            {
                Nombre = txtNombreRol.Text,
                Descripcion = txtDescripcionRol.Text
            };

            try
            {
               _rolService.GuardarRol(nuevoRol);
                MessageBox.Show("Rol guardado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarRoles();
                txtNombreRol.Clear();
                txtDescripcionRol.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el rol: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
      

    }

}
