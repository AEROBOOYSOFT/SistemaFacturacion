using SistemaFacturacion.CLASES;
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

namespace SistemaFacturacion.USUARIOS
{
    /// <summary>
    /// Lógica de interacción para GestionUsuarios.xaml
    /// </summary>
    public partial class GestionUsuarios : Window
    {
        private UsuarioService _usuarioService;
        private RolService _rolService;

        public GestionUsuarios()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            _rolService = new RolService();
            CargarUsuarios();
            CargarRoles();
        }
        private void CargarUsuarios()
        {
            var usuarios = _usuarioService.ObtenerTodosLosUsuarios();
            dgUsuarios.ItemsSource = usuarios;
        }

        private void CargarRoles()
        {
            var roles = _rolService.ObtenerTodosLosRoles();
            cbRoles.ItemsSource = roles;
        }

        private void BtnGuardarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtEmailUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtPasswordUsuario.Password) ||
                cbRoles.SelectedItem == null)
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var nuevoUsuario = new Usuario
            {
                NombreUsuario = txtNombreUsuario.Text,
                Email = txtEmailUsuario.Text,
                Password = txtPasswordUsuario.Password,
                RolID = ((Rol)cbRoles.SelectedItem).RolID
            };

            try
            {
                _usuarioService.GuardarUsuario(nuevoUsuario);
                MessageBox.Show("Usuario guardado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarUsuarios();
                txtNombreUsuario.Clear();
                txtEmailUsuario.Clear();
                txtPasswordUsuario.Clear();
                cbRoles.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}