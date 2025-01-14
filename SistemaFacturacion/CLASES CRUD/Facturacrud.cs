using SistemaFacturacion.Clases;
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

    }
}