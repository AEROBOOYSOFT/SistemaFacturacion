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

namespace SistemaFacturacion.INVENTARIO
{
    /// <summary>
    /// Lógica de interacción para ConsultaMovimientosInventario.xaml
    /// </summary>
    public partial class ConsultaMovimientosInventario : Window
    {
        public ConsultaMovimientosInventario()
        {
            InitializeComponent();
            CargarProductos();
            CargarMovimientos(); // Carga inicial de todos los movimientos
        }
        private void CargarProductos()
        {
            var productos = Facturacrud.ObtenerProductos();
            cmbProductos.ItemsSource = productos;
            cmbProductos.DisplayMemberPath = "Nombre";
            cmbProductos.SelectedValuePath = "IdProducto";
            cmbProductos.SelectedIndex = -1; // Ningún producto seleccionado por defecto
        }
        private void CargarMovimientos(List<MovimientoInventario> movimientos = null)
        {
            if (movimientos == null)
                movimientos = Facturacrud.ObtenerMovimientos();

            dgMovimientos.ItemsSource = movimientos.Select(m => new
            {
                NombreProducto = Facturacrud.ObtenerProductoPorId(m.ProductoID)?.Nombre ?? "Desconocido",
                m.TipoMovimiento,
                m.Cantidad,
                m.FechaMovimiento,
                m.Descripcion
            }).ToList();
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            var movimientos = Facturacrud.ObtenerMovimientos();

            // Filtro por producto
            if (cmbProductos.SelectedValue != null)
            {
                int productoId = (int)cmbProductos.SelectedValue;
                movimientos = movimientos.Where(m => m.ProductoID == productoId).ToList();
            }

            // Filtro por tipo de movimiento
            if (cmbTipoMovimiento.SelectedItem != null && ((ComboBoxItem)cmbTipoMovimiento.SelectedItem).Content.ToString() != "Todos")
            {
                string tipoMovimiento = ((ComboBoxItem)cmbTipoMovimiento.SelectedItem).Content.ToString();
                movimientos = movimientos.Where(m => m.TipoMovimiento == tipoMovimiento).ToList();
            }

            // Filtro por rango de fechas
            if (dpFechaDesde.SelectedDate.HasValue)
                movimientos = movimientos.Where(m => m.FechaMovimiento >= dpFechaDesde.SelectedDate.Value).ToList();

            if (dpFechaHasta.SelectedDate.HasValue)
                movimientos = movimientos.Where(m => m.FechaMovimiento <= dpFechaHasta.SelectedDate.Value).ToList();

            CargarMovimientos(movimientos);
        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
