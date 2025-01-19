using SistemaFacturacion.CLASES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SistemaFacturacion.CLASES_CRUD
{
    public class PermisosService
    {
        private readonly string _connectionString;

        public PermisosService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }

        // Obtener todos los permisos
        public List<Permiso> ObtenerTodosLosPermisos()
        {
            var permisos = new List<Permiso>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT PermisoID, NombrePermiso, Descripcion FROM Permiso";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permisos.Add(new Permiso
                            {
                                PermisoID = reader.GetInt32(0),
                                NombrePermiso = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los permisos: " + ex.Message);
            }

            return permisos;
        }

        // Agregar un nuevo permiso
        public void GuardarPermiso(Permiso permiso)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Permiso (NombrePermiso, Descripcion) VALUES (@NombrePermiso, @Descripcion)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@NombrePermiso", permiso.NombrePermiso));
                        command.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(permiso.Descripcion) ? (object)DBNull.Value : permiso.Descripcion));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el permiso: " + ex.Message);
            }
        }

        // Actualizar un permiso
        public void ActualizarPermiso(Permiso permiso)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "UPDATE Permiso SET NombrePermiso = @NombrePermiso, Descripcion = @Descripcion WHERE PermisoID = @PermisoID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@NombrePermiso", permiso.NombrePermiso));
                        command.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(permiso.Descripcion) ? (object)DBNull.Value : permiso.Descripcion));
                        command.Parameters.Add(new SqlParameter("@PermisoID", permiso.PermisoID));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el permiso: " + ex.Message);
            }
        }

        // Eliminar un permiso
        public void EliminarPermiso(int permisoId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM Permiso WHERE PermisoID = @PermisoID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@PermisoID", permisoId));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el permiso: " + ex.Message);
            }
        }

        // Verificar si un rol tiene un permiso específico
        public bool RolTienePermiso(int rolId, int permisoId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT COUNT(1) FROM RolPermiso WHERE RolID = @RolID AND PermisoID = @PermisoID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@RolID", rolId));
                        command.Parameters.Add(new SqlParameter("@PermisoID", permisoId));

                        return Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar el permiso del rol: " + ex.Message);
            }
        }
        // Obtener permisos por rol
        public List<Permiso> ObtenerPermisosPorRol(int rolId)
        {
            var permisos = new List<Permiso>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT p.PermisoID, p.NombrePermiso, p.Descripcion FROM Permiso p " +
                                "INNER JOIN RolPermiso rp ON p.PermisoID = rp.PermisoID " +
                                "WHERE rp.RolID = @RolID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@RolID", rolId));

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                permisos.Add(new Permiso
                                {
                                    PermisoID = reader.GetInt32(0),
                                    NombrePermiso = reader.GetString(1),
                                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los permisos para el rol: " + ex.Message);
            }

            return permisos;
        }


        public void AsignarPermisosARol(int rolID, List<int> permisosIDs)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Eliminar permisos actuales del rol
                    var deleteQuery = "DELETE FROM RolPermiso WHERE RolID = @RolID";
                    using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@RolID", rolID);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Insertar los nuevos permisos
                    var insertQuery = "INSERT INTO RolPermiso (RolID, PermisoID) VALUES (@RolID, @PermisoID)";
                    foreach (var permisoID in permisosIDs)
                    {
                        using (var insertCommand = new SqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@RolID", rolID);
                            insertCommand.Parameters.AddWithValue("@PermisoID", permisoID);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al asignar permisos al rol: " + ex.Message);
            }
        }



    }
}
