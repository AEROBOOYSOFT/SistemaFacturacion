using SistemaFacturacion.CLASES;
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
        private RolService _rolService;
        public GestionRoles()
        {
            InitializeComponent();
            _rolService = new RolService();
            CargarRoles();
        }
        private void CargarRoles()
        {
            var roles = _rolService.ObtenerTodosLosRoles();
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
        public List<Rol> ObtenerTodosLosRoles()
        {
            var roles = new List<Rol>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT RolID, Nombre, Descripcion FROM Roles";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Rol
                            {
                                RolID = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los roles: " + ex.Message);
            }

            return roles;
        }
        public void GuardarRol(Rol rol)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Roles (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Nombre", rol.Nombre));
                        command.Parameters.Add(new SqlParameter("@Descripcion", rol.Descripcion));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el rol: " + ex.Message);
            }
        }

    }

}
