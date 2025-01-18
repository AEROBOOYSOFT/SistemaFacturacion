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

        // Obtener todos los roles de la base de datos
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
                                NombreRol = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de error, se lanza una excepción con el mensaje de error.
                throw new Exception("Error al obtener los roles: " + ex.Message);
            }

            return roles;
        }

        // Guardar un nuevo rol en la base de datos
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
                        // Asegurando que los parámetros son correctamente asignados
                        command.Parameters.Add(new SqlParameter("@Nombre", rol.NombreRol));
                        command.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(rol.Descripcion) ? (object)DBNull.Value : rol.Descripcion));

                        // Ejecutar la consulta SQL
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de error, se lanza una excepción con el mensaje de error.
                throw new Exception("Error al guardar el rol: " + ex.Message);
            }
        }
    }
}
