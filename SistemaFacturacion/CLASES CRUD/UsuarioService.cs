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
    public class UsuarioService
    {
        private readonly string _connectionString;

        public UsuarioService()
        {
            // Leer la cadena de conexión desde el archivo App.config
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }

        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            var usuarios = new List<Usuario>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT u.UsuarioID, u.NombreUsuario, u.Email, u.Contraseña, u.Activo, u.FechaCreacion, ur.RolID, r.Nombre AS RolNombre
                        FROM Usuario u
                        LEFT JOIN UsuarioRol ur ON u.UsuarioID = ur.UsuarioID
                        LEFT JOIN Rol r ON ur.RolID = r.RolID";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Verificar si el usuario ya está en la lista
                            var usuarioID = reader.GetInt32(0);
                            var usuario = usuarios.FirstOrDefault(u => u.UsuarioID == usuarioID);
                            if (usuario == null)
                            {
                                usuario = new Usuario
                                {
                                    UsuarioID = usuarioID,
                                    NombreUsuario = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Contraseña = reader.GetString(3),
                                    Activo = reader.GetBoolean(4),
                                    FechaCreacion = reader.GetDateTime(5),
                                    Roles = new List<Rol>()
                                };
                                usuarios.Add(usuario);
                            }

                            // Agregar el rol si existe
                            if (!reader.IsDBNull(6) && !usuario.Roles.Any(r => r.RolID == reader.GetInt32(6)))
                            {
                                usuario.Roles.Add(new Rol
                                {
                                    RolID = reader.GetInt32(6),
                                    Nombre = reader.GetString(7)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios: " + ex.Message);
            }

            return usuarios;
        }

        public void GuardarUsuario(Usuario usuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"INSERT INTO Usuario (NombreUsuario, Email, Contraseña, Activo, FechaCreacion) 
                                  VALUES (@NombreUsuario, @Email, @Contraseña, @Activo, @FechaCreacion);
                                  SELECT SCOPE_IDENTITY();"; // Devuelve el ID del usuario insertado

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@NombreUsuario", usuario.NombreUsuario));
                        command.Parameters.Add(new SqlParameter("@Email", usuario.Email));
                        command.Parameters.Add(new SqlParameter("@Contraseña", usuario.Contraseña));
                        command.Parameters.Add(new SqlParameter("@Activo", usuario.Activo));
                        command.Parameters.Add(new SqlParameter("@FechaCreacion", usuario.FechaCreacion));

                        var usuarioID = Convert.ToInt32(command.ExecuteScalar());

                        // Insertar los roles asociados al usuario
                        if (usuario.Roles != null)
                        {
                            foreach (var rol in usuario.Roles)
                            {
                                var queryRol = @"INSERT INTO UsuarioRol (UsuarioID, RolID, FechaAsignacion) 
                                                 VALUES (@UsuarioID, @RolID, @FechaAsignacion)";
                                using (var commandRol = new SqlCommand(queryRol, connection))
                                {
                                    commandRol.Parameters.Add(new SqlParameter("@UsuarioID", usuarioID));
                                    commandRol.Parameters.Add(new SqlParameter("@RolID", rol.RolID));
                                    commandRol.Parameters.Add(new SqlParameter("@FechaAsignacion", DateTime.Now));
                                    commandRol.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el usuario: " + ex.Message);
            }
        }
    }
}
