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

namespace SistemaFacturacion.USUARIOS.CONFIGURACION
{
    /// <summary>
    /// Lógica de interacción para ConfigurarITBIS.xaml
    /// </summary>
    public partial class ConfigurarITBIS : Window
    {
        public ConfigurarITBIS()
        {
            InitializeComponent();
            CargarITBIS();
        }
        private void CargarITBIS()
        {
            decimal impuesto = ConfiguracionCRUD.ObtenerImpuestoITBIS() * 100;
            txtITBIS.Text = impuesto.ToString("F2");
        }
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombreEmpresa.Text) ||
                string.IsNullOrWhiteSpace(txtRUC.Text) ||
                string.IsNullOrWhiteSpace(txtITBIS.Text))
            {
                MessageBox.Show("Por favor complete todos los campos obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtITBIS.Text, out decimal itbis) || itbis < 0 || itbis > 100)
            {
                MessageBox.Show("El valor del ITBIS debe ser un número entre 0 y 100.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Insertar en la base de datos
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO Configuración (NombreEmpresa, RUC, Teléfono, Dirección, ImpuestoITBIS) 
                        VALUES (@NombreEmpresa, @RUC, @Telefono, @Direccion, @ImpuestoITBIS)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreEmpresa", txtNombreEmpresa.Text.Trim());
                        cmd.Parameters.AddWithValue("@RUC", txtRUC.Text.Trim());
                        cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                        cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                        cmd.Parameters.AddWithValue("@ImpuestoITBIS", itbis);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Configuración guardada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la configuración: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
    

