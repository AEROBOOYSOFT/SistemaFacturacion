using SistemaFacturacion.CLASES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SistemaFacturacion.CLASES_CRUD
{
    public class Usuarioscrud
    {
        private readonly string _connectionString;

        public Usuarioscrud()
        {
            // Leer la cadena de conexión desde el archivo App.config
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }

        // Método para cifrar la contraseña antes de guardarla
        private string CifrarContraseña(string contraseña)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertir la contraseña a bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contraseña));

                // Convertir los bytes a un string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void CrearUsuario(Usuario usuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"INSERT INTO Usuarios 
                          (NombreCompleto, Email, NombreUsuario, Contraseña, Activo, FechaCreacion) 
                          VALUES 
                          (@NombreCompleto, @Email, @NombreUsuario, @Contraseña, @Activo, @FechaCreacion);
                          SELECT SCOPE_IDENTITY();"; // Devuelve el ID del usuario insertado

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@NombreCompleto", usuario.NombreCompleto));
                        command.Parameters.Add(new SqlParameter("@Email", usuario.Email));
                        command.Parameters.Add(new SqlParameter("@NombreUsuario", usuario.NombreUsuario));
                        command.Parameters.Add(new SqlParameter("@Contraseña", CifrarContraseña(usuario.Contraseña))); // Cifrar contraseña antes de almacenar
                        command.Parameters.Add(new SqlParameter("@Activo", usuario.Activo));
                        command.Parameters.Add(new SqlParameter("@FechaCreacion", usuario.FechaCreacion));

                        // Ejecutar el comando para insertar el usuario y obtener el UsuarioID
                        var usuarioID = Convert.ToInt32(command.ExecuteScalar());

                        // Insertar los roles del usuario (si tiene)
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
                throw new Exception("Error al crear el usuario: " + ex.Message);
            }
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"UPDATE Usuarios 
                          SET NombreCompleto = @NombreCompleto, 
                              Email = @Email, 
                              NombreUsuario = @NombreUsuario, 
                              Contraseña = @Contraseña, 
                              Activo = @Activo 
                          WHERE UsuarioID = @UsuarioID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@UsuarioID", usuario.UsuarioID));
                        command.Parameters.Add(new SqlParameter("@NombreCompleto", usuario.NombreCompleto));
                        command.Parameters.Add(new SqlParameter("@Email", usuario.Email));
                        command.Parameters.Add(new SqlParameter("@NombreUsuario", usuario.NombreUsuario));
                        command.Parameters.Add(new SqlParameter("@Contraseña", CifrarContraseña(usuario.Contraseña))); // Cifrar contraseña antes de almacenar
                        command.Parameters.Add(new SqlParameter("@Activo", usuario.Activo));
                        command.ExecuteNonQuery();
                    }

                    // Actualizar los roles del usuario (si es necesario)
                    // Primero eliminar los roles actuales
                    var deleteRolesQuery = "DELETE FROM UsuarioRol WHERE UsuarioID = @UsuarioID";
                    using (var deleteRolesCommand = new SqlCommand(deleteRolesQuery, connection))
                    {
                        deleteRolesCommand.Parameters.Add(new SqlParameter("@UsuarioID", usuario.UsuarioID));
                        deleteRolesCommand.ExecuteNonQuery();
                    }

                    // Insertar los nuevos roles
                    if (usuario.Roles != null)
                    {
                        foreach (var rol in usuario.Roles)
                        {
                            var insertRolQuery = @"INSERT INTO UsuarioRol (UsuarioID, RolID, FechaAsignacion) 
                                                   VALUES (@UsuarioID, @RolID, @FechaAsignacion)";
                            using (var insertRolCommand = new SqlCommand(insertRolQuery, connection))
                            {
                                insertRolCommand.Parameters.Add(new SqlParameter("@UsuarioID", usuario.UsuarioID));
                                insertRolCommand.Parameters.Add(new SqlParameter("@RolID", rol.RolID));
                                insertRolCommand.Parameters.Add(new SqlParameter("@FechaAsignacion", DateTime.Now));
                                insertRolCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el usuario: " + ex.Message);
            }
        }

        public void EliminarUsuario(int usuarioId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM Usuarios WHERE UsuarioID = @UsuarioID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@UsuarioID", usuarioId));
                        command.ExecuteNonQuery();
                    }

                    // Eliminar los roles asociados al usuario
                    var deleteRolesQuery = "DELETE FROM UsuarioRol WHERE UsuarioID = @UsuarioID";
                    using (var deleteRolesCommand = new SqlCommand(deleteRolesQuery, connection))
                    {
                        deleteRolesCommand.Parameters.Add(new SqlParameter("@UsuarioID", usuarioId));
                        deleteRolesCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el usuario: " + ex.Message);
            }
        }

        public List<Usuario> ObtenerUsuarios()
        {
            var usuarios = new List<Usuario>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"SELECT u.UsuarioID, u.NombreCompleto, u.Email, u.NombreUsuario, u.Contraseña, u.Activo, u.FechaCreacion, ur.RolID, r.Nombre AS RolNombre
                                  FROM Usuarios u
                                  LEFT JOIN UsuarioRol ur ON u.UsuarioID = ur.UsuarioID
                                  LEFT JOIN Roles r ON ur.RolID = r.RolID";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuarioID = reader.GetInt32(0);
                            var usuario = usuarios.FirstOrDefault(u => u.UsuarioID == usuarioID);
                            if (usuario == null)
                            {
                                usuario = new Usuario
                                {
                                    UsuarioID = usuarioID,
                                    NombreCompleto = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    NombreUsuario = reader.GetString(3),
                                    Contraseña = reader.GetString(4), // Descifrar contraseñas cuando sea necesario
                                    Activo = reader.GetBoolean(5),
                                    FechaCreacion = reader.GetDateTime(6),
                                    Roles = new List<Rol>()
                                };
                                usuarios.Add(usuario);
                            }

                            // Agregar el rol si existe
                            if (!reader.IsDBNull(7))
                            {
                                usuario.Roles.Add(new Rol
                                {
                                    RolID = reader.GetInt32(7),
                                    NombreRol = reader.GetString(8)
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
    }
}
