﻿using SistemaFacturacion.CLIENTES;
using SistemaFacturacion.FACTURACION;
using SistemaFacturacion.PRODUCTOS;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SistemaFacturacion
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // Evento para abrir ClienteFormulario
        private void AbrirClienteFormulario_Click(object sender, RoutedEventArgs e)
        {
            var ventanaCliente = new ClienteFormulario();
            ventanaCliente.ShowDialog();  // Mostrar la ventana como un cuadro de diálogo modal
        }



        // Evento para abrir la ventana de consulta de clientes
        private void AbrirConsultaClientes_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para abrir la ventana de consulta de clientes
            // Aquí deberías abrir tu ventana de clientes (ejemplo con ConsultaClientes)
            ConsultaClientes ventanaClientes = new ConsultaClientes();
            ventanaClientes.ShowDialog();
        }

        // Evento para abrir la ventana de consulta de productos
        private void AbrirConsultaProductos_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para abrir la ventana de consulta de productos
            ConsultaProductos ventanaProductos = new ConsultaProductos();
            ventanaProductos.ShowDialog();
        }
        private void AbrirRegistroFacturas_Click(object sender, RoutedEventArgs e)
        {
            var registroFacturas = new RegistroFacturas();
            registroFacturas.Show();
        }

        // Método para abrir la ventana de consulta de facturas
        private void AbrirConsultaFacturas_Click(object sender, RoutedEventArgs e)
        {
            var consultaFacturas = new ConsultaFacturas();
            consultaFacturas.ShowDialog();
        }

    }
}
