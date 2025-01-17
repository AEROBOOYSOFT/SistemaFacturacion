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
    public class UsuarioRoles
    {

        private readonly string _connectionString;

        public UsuarioRoles()
        {
            // Leer la cadena de conexión desde el archivo App.config
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }

        public void AsignarRolAUsuario(int usuarioId, int rolId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
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
        public void EliminarRolDeUsuario(int usuarioId, int rolId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM UsuarioRoles WHERE UsuarioID = @UsuarioID AND RolID = @RolID";

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
                throw new Exception("Error al eliminar el rol del usuario: " + ex.Message);
            }
        }
        public List<Rol> ObtenerRolesDeUsuario(int usuarioId)
        {
            var roles = new List<Rol>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"SELECT R.RolID, R.Nombre, R.Descripcion
                          FROM UsuarioRoles UR
                          INNER JOIN Roles R ON UR.RolID = R.RolID
                          WHERE UR.UsuarioID = @UsuarioID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@UsuarioID", usuarioId));

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
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los roles del usuario: " + ex.Message);
            }

            return roles;
        }
        public List<Usuario> ObtenerUsuariosDeRol(int rolId)
        {
            var usuarios = new List<Usuario>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"SELECT U.UsuarioID, U.NombreCompleto, U.Email, U.NombreUsuario, U.Activo, U.FechaCreacion
                          FROM UsuarioRoles UR
                          INNER JOIN Usuarios U ON UR.UsuarioID = U.UsuarioID
                          WHERE UR.RolID = @RolID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@RolID", rolId));

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarios.Add(new Usuario
                                {
                                    UsuarioID = reader.GetInt32(0),
                                    NombreCompleto = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    NombreUsuario = reader.GetString(3),
                                    Activo = reader.GetBoolean(4),
                                    FechaCreacion = reader.GetDateTime(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios del rol: " + ex.Message);
            }

            return usuarios;
        }

    }
}
