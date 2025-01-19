using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES_CRUD
{
    public static class ConfiguracionCRUD
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;

        // Obtener la configuración actual
        public static decimal ObtenerImpuestoITBIS()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var query = "SELECT ImpuestoITBIS FROM Configuración WHERE IdConfiguración = 1";
                using (var command = new SqlCommand(query, connection))
                {
                    var valor = command.ExecuteScalar()?.ToString();
                    return decimal.TryParse(valor, out var impuesto) ? impuesto : 0m; // Retornar el valor tal como está
                }
            }
        }

        // Actualizar el impuesto ITBIS
        public static void ActualizarImpuestoITBIS(decimal nuevoImpuesto)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var query = "UPDATE Configuración SET ImpuestoITBIS = @Impuesto WHERE IdConfiguración = 1";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Impuesto", nuevoImpuesto * 100); // Guardar como porcentaje
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
