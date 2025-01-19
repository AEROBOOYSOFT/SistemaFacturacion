using SistemaFacturacion.Clases;
using SistemaFacturacion.CLASES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SistemaFacturacion.CLASES_CRUD
{
    public static class Facturacrud
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;


        // Obtener lista de clientes
        public static List<Cliente> ObtenerClientes()
        {
            var clientes = new List<Cliente>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT IdCliente, Nombre, Cedula, Dirección, Teléfono, Email FROM Clientes";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            IdCliente = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Cedula = reader.GetString(2),
                            Direccion = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Email = reader.IsDBNull(5) ? null : reader.GetString(5)
                        });
                    }
                }
            }

            return clientes;
        }

        // Obtener lista de productos
        public static List<Producto> ObtenerProductos()
        {
            var productos = new List<Producto>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ProductoID, Nombre, Descripcion, Precio, Stock, Estado FROM Productos WHERE Estado = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            IdProducto = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Precio = reader.GetDecimal(3),
                            Stock = reader.GetInt32(4),
                            Estado = reader.GetBoolean(5)
                        });
                    }
                }
            }

            return productos;
        }

        // Guardar factura y sus detalles
        public static void GuardarFactura(Factura factura)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar factura
                        string queryFactura = "INSERT INTO Facturas (ClienteID, Fecha, Total, Estado) " +
                                              "OUTPUT INSERTED.FacturaID " +
                                              "VALUES (@ClienteID, @Fecha, @Total, @Estado)";
                        int idFactura;
                        using (SqlCommand cmd = new SqlCommand(queryFactura, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ClienteID", factura.IdCliente);
                            cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
                            cmd.Parameters.AddWithValue("@Total", factura.Total);
                            cmd.Parameters.AddWithValue("@Estado", 1); // Factura activa
                            idFactura = (int)cmd.ExecuteScalar();
                        }

                        // Insertar detalles de factura
                        string queryDetalle = "INSERT INTO DetalleFactura (FacturaID, ProductoID, Cantidad, PrecioUnitario, Subtotal) " +
                                              "VALUES (@FacturaID, @ProductoID, @Cantidad, @PrecioUnitario, @Subtotal)";
                        using (SqlCommand cmd = new SqlCommand(queryDetalle, conn, transaction))
                        {
                            foreach (var detalle in factura.Detalles)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@FacturaID", idFactura);
                                cmd.Parameters.AddWithValue("@ProductoID", detalle.IdProducto);
                                cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                                cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                                cmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al guardar la factura: " + ex.Message);
                    }
                }
            }
        }


        public static Factura ObtenerFacturaConCliente(int facturaId)
        {
            Factura factura = null;
            string query = @"
        SELECT f.FacturaID, f.Fecha, f.Total, c.Nombre AS NombreCliente
        FROM Facturas f
        INNER JOIN Clientes c ON f.ClienteID = c.IdCliente
        WHERE f.FacturaID = @FacturaID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FacturaID", facturaId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            factura = new Factura
                            {
                                IdFactura = (int)reader["FacturaID"],
                                Fecha = (DateTime)reader["Fecha"],
                                Total = (decimal)reader["Total"],
                                Cliente = new Cliente
                                {
                                    Nombre = reader["NombreCliente"].ToString()
                                }
                            };
                        }
                    }
                }
            }

            return factura;
        }
        public static void ActualizarStock(int productoId, int nuevoStock)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "UPDATE Productos SET Stock = @Stock WHERE ProductoID = @ProductoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Stock", nuevoStock);
                    command.Parameters.AddWithValue("@ProductoID", productoId);
                    command.ExecuteNonQuery();
                }
            }
        }

    
    public static void RegistrarMovimiento(MovimientoInventario movimiento)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var query = "INSERT INTO MovimientosInventario (ProductoID, TipoMovimiento, Cantidad, FechaMovimiento, Descripcion) " +
                        "VALUES (@ProductoID, @TipoMovimiento, @Cantidad, @FechaMovimiento, @Descripcion)";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductoID", movimiento.ProductoID);
                command.Parameters.AddWithValue("@TipoMovimiento", movimiento.TipoMovimiento);
                command.Parameters.AddWithValue("@Cantidad", movimiento.Cantidad);
                command.Parameters.AddWithValue("@FechaMovimiento", movimiento.FechaMovimiento);
                command.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(movimiento.Descripcion) ? (object)DBNull.Value : movimiento.Descripcion);
                command.ExecuteNonQuery();
            }
        }
    }
        public static List<MovimientoInventario> ObtenerMovimientos()
        {
            var movimientos = new List<MovimientoInventario>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM MovimientosInventario";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movimientos.Add(new MovimientoInventario
                        {
                            MovimientoID = (int)reader["MovimientoID"],
                            ProductoID = (int)reader["ProductoID"],
                            TipoMovimiento = reader["TipoMovimiento"].ToString(),
                            Cantidad = (int)reader["Cantidad"],
                            FechaMovimiento = (DateTime)reader["FechaMovimiento"],
                            Descripcion = reader["Descripcion"].ToString()
                        });
                    }
                }
            }
            return movimientos;
        }

        public static Producto ObtenerProductoPorId(int productoId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Productos WHERE ProductoID = @ProductoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductoID", productoId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Producto
                            {
                                IdProducto = (int)reader["ProductoID"],
                                Nombre = reader["Nombre"].ToString(),
                                Stock = (int)reader["Stock"],
                                Precio = (decimal)reader["Precio"]
                            };
                        }
                    }
                }
            }
            return null;
        }

    }
}