using SistemaFacturacion.CLASES;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SistemaFacturacion.CLASES
{
    public class Autenticacion
    {
        public Usuario UsuarioAutenticado { get; private set; }

        // Método para iniciar sesión con validación contra la base de datos
        public bool IniciarSesion(string nombreUsuario, string contraseña)
        {
            // Obtener la cadena de conexión a la base de datos
            string connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Consulta para validar el usuario en la base de datos
                    string query = "SELECT UsuarioID, NombreCompleto, Email, NombreUsuario, Contraseña, Activo FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Activo = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                        // Ejecutar la consulta y obtener los datos del usuario
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string passwordFromDb = reader["Contraseña"].ToString();

                                // Comparar la contraseña proporcionada con la almacenada (en un sistema real debería ser un hash seguro)
                                if (contraseña == passwordFromDb) // En un entorno real, deberías comparar con un hash seguro
                                {
                                    // Si las credenciales son correctas, crear un nuevo objeto Usuario
                                    UsuarioAutenticado = new Usuario(
                                        (int)reader["UsuarioID"],
                                        reader["NombreCompleto"].ToString(),
                                        reader["Email"].ToString(),
                                        reader["NombreUsuario"].ToString(),
                                        passwordFromDb, // Aquí almacenamos la contraseña de forma temporal, en un sistema real debería ser un hash
                                        (bool)reader["Activo"],
                                        DateTime.Now // Aquí puedes agregar la fecha de creación o cualquier otra propiedad si es necesario
                                    );

                                    return true; // Usuario autenticado correctamente
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Manejar los errores de la base de datos
                    Console.WriteLine($"Error de base de datos: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier otro error
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }
            }

            // Si las credenciales no son correctas o si ocurre un error, retornamos falso
            UsuarioAutenticado = null;
            return false;
        }

        // Método para cerrar sesión
        public void CerrarSesion()
        {
            UsuarioAutenticado = null;
        }
    }
}
