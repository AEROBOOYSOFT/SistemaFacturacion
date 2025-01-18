using SistemaFacturacion.CLASES;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES_CRUD
{
    public class Servicioderoles
    {
        public List<Rol> ObtenerTodosLosRoles()
        {
            var roles = new List<Rol>();

            try
            {
              ///  using (var connection = new SqlConnection(_connectionString))
                {
               //     connection.Open();
                    var query = "SELECT RolID, Nombre, Descripcion FROM Roles";

               //     using (var command = new SqlCommand(query, connection))
                 //   using (var reader = command.ExecuteReader())
                    {
                   //     while (reader.Read())
                        {
                            roles.Add(new Rol
                            {
                      //          RolID = reader.GetInt32(0),
                      //          Nombre = reader.GetString(1),
                      //          Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los roles: " + ex.Message);
            }

            return roles;
        }

    }
}
