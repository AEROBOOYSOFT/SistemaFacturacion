using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using SistemaFacturacion.Clases;  // Necesario para usar ConfigurationManager

namespace SistemaFacturacion.Clases_crud
{
    public class Clientecrud
    {
        private readonly string _connectionString;

        public Clientecrud()
        {
            // Leer la cadena de conexión desde el archivo App.config
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }

        // Crear un nuevo cliente
        public void CrearCliente(Cliente cliente)
        {
            if (string.IsNullOrEmpty(cliente.Nombre)) // Verificación para 'Nombre'
            {
                throw new ArgumentException("El nombre del cliente no puede ser vacío.");
            }
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Clientes (Nombre, Cedula, Dirección, Teléfono, Email) VALUES (@Nombre, @Cedula, @Direccion, @Telefono, @Email)";
                SqlCommand command = new SqlCommand(query, connection);

                // Usar valores válidos para los parámetros
                command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                command.Parameters.AddWithValue("@Cedula", cliente.Cedula ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Direccion", cliente.Direccion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Telefono", cliente.Telefono ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Email", cliente.Email ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }


        // Leer todos los clientes
        public List<Cliente> ObtenerClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT IdCliente, Nombre, Cedula, Dirección, Teléfono, Email FROM Clientes";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clientes.Add(new Cliente
                    {
                        IdCliente = reader.GetInt32(0),
                        Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),  // Verificar si es nulo
                        Cedula = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),  // Verificar si es nulo
                        Direccion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),  // Verificar si es nulo
                        Telefono = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),  // Verificar si es nulo
                        Email = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)  // Verificar si es nulo
                    });
                }
            }

            return clientes;
        }

        // Leer un cliente por Id
        public Cliente ObtenerClientePorId(int idCliente)
        {
            Cliente cliente = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT IdCliente, Nombre, Cedula, Dirección, Teléfono, Email FROM Clientes WHERE IdCliente = @IdCliente";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCliente", idCliente);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    cliente = new Cliente
                    {
                        IdCliente = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Cedula = reader.GetString(2),
                        Direccion = reader.GetString(3),
                        Telefono = reader.GetString(4),
                        Email = reader.GetString(5)
                    };
                }
            }

            return cliente;
        }

        // Actualizar un cliente
        public void ActualizarCliente(Cliente cliente)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Clientes SET Nombre = @Nombre, Cedula = @Cedula, Dirección = @Direccion, Teléfono = @Telefono, Email = @Email WHERE IdCliente = @IdCliente";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                command.Parameters.AddWithValue("@Cedula", cliente.Cedula);
                command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                command.Parameters.AddWithValue("@Email", cliente.Email);

                command.ExecuteNonQuery();
            }
        }

        // Eliminar un cliente
        public void EliminarCliente(int idCliente)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Clientes WHERE IdCliente = @IdCliente";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCliente", idCliente);

                command.ExecuteNonQuery();
            }
        }
    }
}
