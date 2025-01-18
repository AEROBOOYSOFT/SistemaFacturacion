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
    public class Rolescrud
    {
        private readonly string _connectionString;

        public Rolescrud()
        {
            // Leer la cadena de conexión desde el archivo App.config
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }

        public void CrearRol(Rol rol)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Roles (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Nombre", rol.NombreRol));
                        command.Parameters.Add(new SqlParameter("@Descripcion", rol.Descripcion ?? (object)DBNull.Value));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el rol: " + ex.Message);
            }
        }

        public void ActualizarRol(Rol rol)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "UPDATE Roles SET Nombre = @Nombre, Descripcion = @Descripcion WHERE RoleID = @RoleID";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RoleID", rol.RolID);
                        command.Parameters.AddWithValue("@Nombre", rol.NombreRol);
                        command.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(rol.Descripcion) ? (object)DBNull.Value : rol.Descripcion);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el rol: " + ex.Message);
            }
        }

        public void EliminarRol(int roleId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM Roles WHERE RoleID = @RoleID";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RoleID", roleId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el rol: " + ex.Message);
            }
        }

        public List<Rol> ObtenerRoles()
        {
            var roles = new List<Rol>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM Roles ORDER BY Nombre";  // Añadido ORDER BY para mejorar el orden de los roles
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
                throw new Exception("Error al obtener los roles: " + ex.Message);
            }

            return roles;
        }
    }
}
