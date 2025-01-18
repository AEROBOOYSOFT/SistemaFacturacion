using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES
{
    public class AuditoriaServicio
    {

        private readonly string _connectionString;

        public AuditoriaServicio()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }
        public void RegistrarAccion(int usuarioID, string accion, string detalles)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Auditoria (UsuarioID, Accion, Detalles) VALUES (@UsuarioID, @Accion, @Detalles)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        command.Parameters.AddWithValue("@Accion", accion);
                        command.Parameters.AddWithValue("@Detalles", detalles);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar acción en auditoría: " + ex.Message);
            }
        }

    }
}
