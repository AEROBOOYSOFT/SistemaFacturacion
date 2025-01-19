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
            if (decimal.TryParse(txtITBIS.Text, out var nuevoITBIS))
            {
                ConfiguracionCRUD.ActualizarImpuestoITBIS(nuevoITBIS / 100);
                MessageBox.Show("ITBIS actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Ingrese un valor válido para el ITBIS.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
