using SistemaFacturacion.CLASES;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SistemaFacturacion.USUARIOS
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        // Permite mover la ventana.
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // Minimiza la ventana.
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Cierra la aplicación.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Maneja el evento de inicio de sesión.
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPass.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, ingresa todos los campos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar las credenciales.
            if (ValidarUsuario(username, password))
            {
                MessageBox.Show($"Bienvenido, {username}!", "Inicio de Sesión Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

                // Abrir la ventana principal.
                this.Hide();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método para validar credenciales.
        private bool ValidarUsuario(string username, string password)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Consulta para validar el usuario.
                    string query = "SELECT Contraseña FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Activo = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreUsuario", username);

                        var hashedPasswordFromDb = cmd.ExecuteScalar() as string;

                        if (!string.IsNullOrEmpty(hashedPasswordFromDb))
                        {
                            // Comparar la contraseña hasheada.
                            return VerificarHash(password, hashedPasswordFromDb);
                        }
                        else
                        {
                            MessageBox.Show("El usuario no está activo o no existe.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Error de conexión con la base de datos: {ex.Message}", "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return false;
        }

        // Método para hashear una contraseña.
        private string GenerarHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Método para verificar un hash.
        private bool VerificarHash(string password, string hashedPassword)
        {
            string hashedInput = GenerarHash(password);
            return hashedInput == hashedPassword;
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}





