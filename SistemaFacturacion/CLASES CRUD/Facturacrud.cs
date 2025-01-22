using SistemaFacturacion.Clases;
using SistemaFacturacion.CLASES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            if (factura == null || factura.Detalles == null || factura.Detalles.Count == 0)
                throw new ArgumentException("La factura y sus detalles no pueden estar vacíos.");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar factura
                        string queryFactura = @"
                    INSERT INTO Facturas (ClienteID, Fecha, Subtotal, Impuestos, Total, Estado, NombreCliente) 
                    OUTPUT INSERTED.FacturaID 
                    VALUES (@ClienteID, @Fecha, @Subtotal, @Impuestos, @Total, @Estado, @NombreCliente)";

                        int idFactura;
                        using (SqlCommand cmdFactura = new SqlCommand(queryFactura, conn, transaction))
                        {
                            cmdFactura.Parameters.AddWithValue("@ClienteID", factura.IdCliente);
                            cmdFactura.Parameters.AddWithValue("@Fecha", factura.Fecha);
                            cmdFactura.Parameters.AddWithValue("@Subtotal", factura.Subtotal);
                            cmdFactura.Parameters.AddWithValue("@Impuestos", factura.Impuestos);
                            cmdFactura.Parameters.AddWithValue("@Total", factura.Total);
                            cmdFactura.Parameters.AddWithValue("@Estado", factura.Estado);
                            cmdFactura.Parameters.AddWithValue("@NombreCliente", factura.NombreCliente ?? (object)DBNull.Value);

                            idFactura = (int)cmdFactura.ExecuteScalar();
                        }

                        // Insertar detalles de la factura
                        string queryDetalle = @"
    INSERT INTO DetalleFactura 
    (FacturaID, ProductoID, Cantidad, PrecioUnitario, Subtotal, ImpuestoPorProducto, Descuento, TotalLinea, DescripcionProducto, Estado) 
    VALUES 
    (@FacturaID, @ProductoID, @Cantidad, @PrecioUnitario, @Subtotal, @ImpuestoPorProducto, @Descuento, @TotalLinea, @DescripcionProducto, @Estado)";

                        using (SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conn, transaction))
                        {
                            var paramFacturaID = new SqlParameter("@FacturaID", idFactura);
                            var paramProductoID = new SqlParameter("@ProductoID", SqlDbType.Int);
                            var paramCantidad = new SqlParameter("@Cantidad", SqlDbType.Int);
                            var paramPrecioUnitario = new SqlParameter("@PrecioUnitario", SqlDbType.Decimal);
                            var paramSubtotal = new SqlParameter("@Subtotal", SqlDbType.Decimal);
                            var paramImpuestoPorProducto = new SqlParameter("@ImpuestoPorProducto", SqlDbType.Decimal);
                            var paramDescuento = new SqlParameter("@Descuento", SqlDbType.Decimal);
                            var paramTotalLinea = new SqlParameter("@TotalLinea", SqlDbType.Decimal);
                            var paramDescripcionProducto = new SqlParameter("@DescripcionProducto", SqlDbType.NVarChar, 255);
                            var paramEstado = new SqlParameter("@Estado", SqlDbType.TinyInt);

                            cmdDetalle.Parameters.Add(paramFacturaID);
                            cmdDetalle.Parameters.Add(paramProductoID);
                            cmdDetalle.Parameters.Add(paramCantidad);
                            cmdDetalle.Parameters.Add(paramPrecioUnitario);
                            cmdDetalle.Parameters.Add(paramSubtotal);
                            cmdDetalle.Parameters.Add(paramImpuestoPorProducto);
                            cmdDetalle.Parameters.Add(paramDescuento);
                            cmdDetalle.Parameters.Add(paramTotalLinea);
                            cmdDetalle.Parameters.Add(paramDescripcionProducto);
                            cmdDetalle.Parameters.Add(paramEstado);

                            foreach (var detalle in factura.Detalles)
                            {
                                paramProductoID.Value = detalle.IdProducto;
                                paramCantidad.Value = detalle.Cantidad;
                                paramPrecioUnitario.Value = detalle.PrecioUnitario;
                                paramSubtotal.Value = detalle.Subtotal;
                                paramImpuestoPorProducto.Value = detalle.ImpuestoPorProducto;
                                paramDescuento.Value = detalle.Descuento;
                                paramTotalLinea.Value = detalle.TotalLinea;
                                paramDescripcionProducto.Value = detalle.DescripcionProducto ?? (object)DBNull.Value;
                                paramEstado.Value = detalle.Estado;

                                cmdDetalle.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error al guardar la factura. Detalles: {ex.Message}", ex);
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
        // Método para obtener estado de una factura específica
        public static string ObtenerEstadoFactura(int facturaID)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                CASE 
                    WHEN f.Total <= ISNULL(SUM(p.MontoPagado), 0) 
                    THEN 'Pagada' 
                    ELSE 'Pendiente' 
                END AS EstadoFactura
            FROM Facturas f
            LEFT JOIN Pagos p ON f.FacturaID = p.FacturaID
            WHERE f.FacturaID = @FacturaID
            GROUP BY f.Total;
        ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FacturaID", facturaID);

                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "Pendiente";
            }
        }
    }
}