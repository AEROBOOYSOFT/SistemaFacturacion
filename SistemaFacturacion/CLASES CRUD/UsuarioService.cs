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
                    var query = @"SELECT u.UsuarioID, u.NombreUsuario, u.Email, u.RolID, r.Nombre AS RolNombre
                          FROM Usuarios u
                          INNER JOIN Roles r ON u.RolID = r.RolID";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                UsuarioID = reader.GetInt32(0),
                                NombreUsuario = reader.GetString(1),
                                Email = reader.GetString(2),
                                RolID = reader.GetInt32(3),
                                Rol = new Rol
                                {
                                    RolID = reader.GetInt32(3),
                                    Nombre = reader.GetString(4)
                                }
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

        public void GuardarUsuario(Usuario usuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"INSERT INTO Usuarios (NombreUsuario, Email, Password, RolID) 
                          VALUES (@NombreUsuario, @Email, @Password, @RolID)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@NombreUsuario", usuario.NombreUsuario));
                        command.Parameters.Add(new SqlParameter("@Email", usuario.Email));
                        command.Parameters.Add(new SqlParameter("@Password", usuario.Password));
                        command.Parameters.Add(new SqlParameter("@RolID", usuario.RolID));
                        command.ExecuteNonQuery();
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
