using SistemaFacturacion.CLASES;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SistemaFacturacion.CLASES_CRUD
{
    public static class PagoCRUD
    {
        private static string connectionString = "Server=aerobooy;Database=SistemaFacturacion;User ID=sa;Password=aerobooy2020;Encrypt=False;TrustServerCertificate=True";

        // Método para insertar un nuevo pago
        public static void InsertarPago(Pago pago)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Pagos (FacturaID, FechaPago, Monto, MetodoPago, UsuarioID) " +
                               "VALUES (@FacturaID, @FechaPago, @Monto, @MetodoPago, @UsuarioID)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FacturaID", pago.FacturaID);
                    cmd.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                    cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                    cmd.Parameters.AddWithValue("@MetodoPago", pago.MetodoPago);
                    cmd.Parameters.AddWithValue("@UsuarioID", pago.UsuarioID);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        // Método para obtener los pagos de una factura específica
        public static List<Pago> ObtenerPagosPorFactura(int facturaId)
        {
            List<Pago> pagos = new List<Pago>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Pagos WHERE FacturaID = @FacturaID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FacturaID", facturaId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(new Pago
                            {
                                PagoID = Convert.ToInt32(reader["PagoID"]),
                                FacturaID = Convert.ToInt32(reader["FacturaID"]),
                                FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                                Monto = Convert.ToDecimal(reader["Monto"]),
                                MetodoPago = reader["MetodoPago"].ToString()
                            });
                        }
                    }
                }
            }

            return pagos;
        }
       

        // Método para actualizar un pago
        public static void ActualizarPago(Pago pago)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Pagos SET FechaPago = @FechaPago, MontoPagado = @Monto, MetodoPago = @MetodoPago " +
                               "WHERE PagoID = @PagoID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PagoID", pago.PagoID);
                    cmd.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                    cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                    cmd.Parameters.AddWithValue("@MetodoPago", pago.MetodoPago);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /* Método para eliminar un pago numero 1
         public static void EliminarPago(int pagoId)
         {
             using (SqlConnection conn = new SqlConnection(connectionString))
             {
                 conn.Open();
                 string query = "DELETE FROM Pagos WHERE PagoID = @PagoID";

                 using (SqlCommand cmd = new SqlCommand(query, conn))
                 {
                     cmd.Parameters.AddWithValue("@PagoID", pagoId);
                     cmd.ExecuteNonQuery();
                 }
             }
         }
           */


        // Método para eliminar un pago numero 2 metodo boleano
        public static bool EliminarPago(int pagoID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Pagos WHERE PagoID = @PagoID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PagoID", pagoID);

                int filasAfectadas = cmd.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }
       


        // Método para obtener el monto total pagado de una factura
        public static decimal ObtenerTotalPagado(int facturaId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT SUM(Monto) FROM Pagos WHERE FacturaID = @FacturaID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FacturaID", facturaId);
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }
        public static List<Pago> BuscarPagos(DateTime? fechaInicio, DateTime? fechaFin, string metodoPago)
        {
            List<Pago> listaPagos = new List<Pago>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Pagos WHERE 1=1";

                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    query += " AND FechaPago BETWEEN @FechaInicio AND @FechaFin";
                }

                if (!string.IsNullOrEmpty(metodoPago) && metodoPago != "Todos")
                {
                    query += " AND MetodoPago = @MetodoPago";
                }

                SqlCommand cmd = new SqlCommand(query, conn);

                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value);
                }

                if (!string.IsNullOrEmpty(metodoPago) && metodoPago != "Todos")
                {
                    cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
                }

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listaPagos.Add(new Pago
                    {
                        PagoID = Convert.ToInt32(reader["PagoID"]),
                        FacturaID = Convert.ToInt32(reader["FacturaID"]),
                        FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                        Monto = Convert.ToDecimal(reader["MontoPagado"]),
                        MetodoPago = reader["MetodoPago"].ToString()
                    });
                }
            }

            return listaPagos;
        }


      

    }
}
