using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES_CRUD
{
    public class UsuarioRoles
    {
        public void AsignarRolAUsuario(int usuarioId, int rolId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO UsuarioRoles (UsuarioID, RolID) VALUES (@UsuarioID, @RolID)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@UsuarioID", usuarioId));
                        command.Parameters.Add(new SqlParameter("@RolID", rolId));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al asignar el rol al usuario: " + ex.Message);
            }
        }
    }
}
