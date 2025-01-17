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
    public class Usuarioscrud
    {
        private readonly string _connectionString;

        public Usuarioscrud()
        {
            // Leer la cadena de conexión desde el archivo App.config
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
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
                          (@NombreCompleto, @Email, @NombreUsuario, @Contraseña, @Activo, @FechaCreacion)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@NombreCompleto", usuario.NombreCompleto));
                        command.Parameters.Add(new SqlParameter("@Email", usuario.Email));
                        command.Parameters.Add(new SqlParameter("@NombreUsuario", usuario.NombreUsuario));
                        command.Parameters.Add(new SqlParameter("@Contraseña", usuario.Contraseña)); // Cifrar antes de almacenar
                        command.Parameters.Add(new SqlParameter("@Activo", usuario.Activo));
                        command.Parameters.Add(new SqlParameter("@FechaCreacion", usuario.FechaCreacion));
                        command.ExecuteNonQuery();
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
                        command.Parameters.Add(new SqlParameter("@Contraseña", usuario.Contraseña)); // Cifrar antes de almacenar
                        command.Parameters.Add(new SqlParameter("@Activo", usuario.Activo));
                        command.ExecuteNonQuery();
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
                    var query = "SELECT * FROM Usuarios";
                    using (var command = new SqlCommand(query, connection))
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
                                Contraseña = reader.GetString(4), // Descifrar al usar, si está encriptada
                                Activo = reader.GetBoolean(5),
                                FechaCreacion = reader.GetDateTime(6)
                            });
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
