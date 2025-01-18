using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
    /// Lógica de interacción para CrearUsuario.xaml
    /// </summary>
    public partial class CrearUsuario : Window
    {
        public CrearUsuario()
        {
            InitializeComponent();
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            string nombreCompleto = txtNombreCompleto.Text;
            string email = txtEmail.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(nombreCompleto) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, ingresa todos los campos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Llamar al método que agrega el usuario
            AgregarUsuario(nombreCompleto, email, username, password);
            MessageBox.Show("Usuario registrado exitosamente.", "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close(); // Cerrar la ventana de registro
        }

        private void AgregarUsuario(string nombreCompleto, string email, string username, string password)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;

            // Generar hash de la contraseña.
            

            // Fecha de creación: Obtener la fecha y hora actuales
            DateTime fechaCreacion = DateTime.Now;

            // Asignar RoleID. Si no deseas roles, usa NULL o asigna un valor predeterminado.
            int? roleID = null; // o asigna un valor como 1 si es un rol predeterminado

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Consulta SQL para agregar el usuario
                    string query = "INSERT INTO Usuario (NombreCompleto, Email, NombreUsuario, Contraseña, Activo, FechaCreacion, RoleID) " +
                                   "VALUES (@NombreCompleto, @Email, @NombreUsuario, @Contraseña, 1, @FechaCreacion, @RoleID)"; // Activo por defecto es 1

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@NombreUsuario", username);
                        cmd.Parameters.AddWithValue("@Contraseña", password);
                        cmd.Parameters.AddWithValue("@FechaCreacion", fechaCreacion);
                        cmd.Parameters.AddWithValue("@RoleID", roleID ?? (object)DBNull.Value); // Si RoleID es null, se pasa DBNull

                        // Ejecutar la consulta
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Usuario agregado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Error al agregar el usuario: {ex.Message}", "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
       
       
    }
}
