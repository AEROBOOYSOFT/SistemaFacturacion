using SistemaFacturacion.Clases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SistemaFacturacion.CLASES_CRUD
{
    public class consultafacturacrud
    {
        private static readonly string _connectionString;

        static consultafacturacrud()
        {
            // Leer la cadena de conexión desde el archivo App.config
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }

        // Método para guardar una factura
        public static void GuardarFactura(Factura factura)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Facturas (ClienteID, Fecha, Total, Estado, NombreCliente) " +
                            "VALUES (@ClienteID, @Fecha, @Total, @Estado, @NombreCliente); " +
                            "SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClienteID", factura.IdCliente);
                    command.Parameters.AddWithValue("@Fecha", factura.Fecha);
                    command.Parameters.AddWithValue("@Total", factura.Total);
                    command.Parameters.AddWithValue("@Estado", 1); // Estado: Emitida (1)
                    command.Parameters.AddWithValue("@NombreCliente", factura.Cliente.Nombre);

                    var idFactura = Convert.ToInt32(command.ExecuteScalar());
                    factura.IdFactura = idFactura;
                }

                // Guardar los detalles de la factura
                foreach (var detalle in factura.Detalles)
                {
                    GuardarDetalleFactura(detalle, factura.IdFactura);
                }
            }
        }

        // Método para guardar los detalles de una factura
        private static void GuardarDetalleFactura(DetalleFactura detalle, int idFactura)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO DetalleFactura (FacturaID, ProductoID, Cantidad, PrecioUnitario, Subtotal) " +
                            "VALUES (@FacturaID, @ProductoID, @Cantidad, @PrecioUnitario, @Subtotal);";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FacturaID", idFactura);
                    command.Parameters.AddWithValue("@ProductoID", detalle.IdProducto);
                    command.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    command.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                    command.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Método para obtener las facturas
        public static List<Factura> ObtenerFacturas()
        {
            var facturas = new List<Factura>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
            SELECT f.FacturaID, f.ClienteID, c.Nombre AS NombreCliente, f.Fecha, f.Total, f.Estado
            FROM Facturas f
            INNER JOIN Clientes c ON f.ClienteID = c.IdCliente";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var factura = new Factura
                            {
                                IdFactura = reader.GetInt32(0),
                                IdCliente = reader.GetInt32(1),
                                Fecha = reader.GetDateTime(3),
                                Total = reader.GetDecimal(4),
                                Estado = reader.GetBoolean(5),
                                Cliente = new Cliente
                                {
                                    Nombre = reader.IsDBNull(2) ? "Desconocido" : reader.GetString(2)  // Asigna el nombre del cliente
                                }
                            };

                            facturas.Add(factura);
                        }
                    }
                }
            }

            return facturas;
        }

        // Método para anular una factura
        public static void AnularFactura(int idFactura)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE Facturas SET Estado = 0 WHERE FacturaID = @IdFactura";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdFactura", idFactura);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}

