using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SistemaFacturacion.Clases_crud
{
    public class Productocrud
    {
        private readonly string _connectionString;

        // Constructor que obtiene la cadena de conexión desde App.config
        public Productocrud()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;
        }

        // Obtener un producto por su ID
        public DataRow ObtenerProductoPorId(int productoId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT ProductoID, Nombre, Descripcion, Precio, Stock, Estado FROM Productos WHERE ProductoID = @ProductoID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductoID", productoId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            // Retornar el primer DataRow si existe, o null si no se encuentra el producto
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }

            return null;
        }

        // Crear un nuevo producto
        public void CrearProducto(string nombre, string descripcion, decimal precio, int stock)
        {
            if (string.IsNullOrEmpty(nombre))  // Verificación para 'Nombre'
            {
                throw new ArgumentException("El nombre del producto no puede ser vacío.");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Productos (Nombre, Descripcion, Precio, Stock) VALUES (@Nombre, @Descripcion, @Precio, @Stock)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(descripcion) ? (object)DBNull.Value : descripcion);  // Simplificación
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Stock", stock);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Leer todos los productos
        public DataTable ObtenerProductos()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT ProductoID, Nombre, Descripcion, Precio, Stock, Estado FROM Productos";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        // Actualizar un producto existente
        public void ActualizarProducto(int productoId, string nombre, string descripcion, decimal precio, int stock, bool estado)
        {
            if (string.IsNullOrEmpty(nombre))  // Verificación para 'Nombre'
            {
                throw new ArgumentException("El nombre del producto no puede ser vacío.");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Productos SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, Stock = @Stock, Estado = @Estado WHERE ProductoID = @ProductoID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductoID", productoId);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(descripcion) ? (object)DBNull.Value : descripcion);  // Simplificación
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Stock", stock);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Eliminar un producto
        public void EliminarProducto(int productoId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Productos WHERE ProductoID = @ProductoID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductoID", productoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
