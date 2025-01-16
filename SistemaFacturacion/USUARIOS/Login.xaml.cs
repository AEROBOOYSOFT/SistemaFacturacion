
using SistemaFacturacion.CLASES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace SistemaFacturacion.USUARIOS
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        //Permite el movimiento del formulario 
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        //minimiza el formulario
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        //cierra el sistema
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //valida los datos proporcionado por el usuaio,
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Validar();
        }

        private void Validar()
        {
            //Llama la clase usuario
            Usuario usuario = new Usuario();

            usuario.Username = txtUser.Text;
            usuario.Password = txtPass.Password;

            /*confirma que los campos no esten vacios,condición en la que ambas deben de ser verdadera
            si estan vacio pedira los datos, de otro modo realiza la conexion con la base de datos
          */
            if (string.IsNullOrWhiteSpace(usuario.Username) || string.IsNullOrWhiteSpace(usuario.Password))
            {
                MessageBox.Show("Verificar campos.", "Error de Validación", (MessageBoxButton)MessageBoxButtons.OK, (MessageBoxImage)MessageBoxIcon.Warning);
                return;


            }

            //Conexion a la base de datos Sistema de facturación
            string _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                try
                {
                    conn.Open();
                    string query = "Select COUNT(1) from USUARIOS where Username=@Username And Password=@Password ";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = usuario.Username;
                        cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = usuario.Password;


                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count == 1)
                        {
                            this.Hide();
                            MainWindow frm = new MainWindow();
                            frm.Show();
                        }

                        else
                        {
                            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("usuario o contraseña incorrecta", "Error de login", (MessageBoxButton)MessageBoxButtons.OK, (MessageBoxImage)MessageBoxIcon.Error);

                        }
                    }
                }
                catch (SqlException sqlEx)
                {

                    System.Windows.MessageBox.Show("Error de conexión: " + sqlEx.Message);
                }
                catch (Exception ex)
                {

                    System.Windows.MessageBox.Show("Error inesperado: " + ex.Message);
                }

            }
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {

        }
    }



}
    







