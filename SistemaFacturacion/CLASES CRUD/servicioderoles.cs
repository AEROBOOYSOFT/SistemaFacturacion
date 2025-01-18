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
    public class Servicioderoles
    {
        private readonly string _connectionString;

        public Servicioderoles()
        {
            // Leer la cadena de conexión desde el archivo App.config
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }
        public List<Rol> ObtenerTodosLosRoles()
        {
            var roles = new List<Rol>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT RolID, Nombre, Descripcion FROM Roles";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Rol
                            {
                                RolID = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
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
        public void GuardarRol(Rol rol)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Roles (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Nombre", rol.Nombre));
                        command.Parameters.Add(new SqlParameter("@Descripcion", rol.Descripcion));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el rol: " + ex.Message);
            }
        }


    }
}
