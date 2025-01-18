using SistemaFacturacion.CLASES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

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
                        // Asignar parámetros
                        command.Parameters.Add(new SqlParameter("@Nombre", rol.NombreRol));
                        command.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(rol.Descripcion) ? (object)DBNull.Value : rol.Descripcion));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el rol: " + ex.Message);
            }
        }

        // Actualizar un rol existente en la base de datos
        public void ActualizarRol(Rol rol)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "UPDATE Roles SET Nombre = @Nombre, Descripcion = @Descripcion WHERE RolID = @RolID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Nombre", rol.NombreRol));
                        command.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(rol.Descripcion) ? (object)DBNull.Value : rol.Descripcion));
                        command.Parameters.Add(new SqlParameter("@RolID", rol.RolID));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el rol: " + ex.Message);
            }
        }

        // Eliminar un rol por su ID
        public void EliminarRol(int rolId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM Roles WHERE RolID = @RolID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@RolID", rolId));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el rol: " + ex.Message);
            }
        }

        // Asignar permisos a un rol
        public void AsignarPermisoARol(int rolId, int permisoId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO RolPermiso (RolID, PermisoID) VALUES (@RolID, @PermisoID)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@RolID", rolId));
                        command.Parameters.Add(new SqlParameter("@PermisoID", permisoId));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al asignar permiso al rol: " + ex.Message);
            }
        }

        // Quitar permisos de un rol
        public void QuitarPermisoDeRol(int rolId, int permisoId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM RolPermiso WHERE RolID = @RolID AND PermisoID = @PermisoID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@RolID", rolId));
                        command.Parameters.Add(new SqlParameter("@PermisoID", permisoId));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al quitar permiso del rol: " + ex.Message);
            }
        }
    }
}
