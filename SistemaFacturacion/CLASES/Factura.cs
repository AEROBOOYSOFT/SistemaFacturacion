using System;
using System.Collections.Generic;

namespace SistemaFacturacion.Clases
{
    public class Factura
    {
        public int IdFactura { get; set; }  // Identificador único de la factura
        public int IdCliente { get; set; }  // Clave foránea que referencia al cliente
        public string NombreCliente => Cliente?.Nombre;  // Propiedad solo lectura que obtiene el nombre del cliente // Nombre del cliente (esto lo obtendrás al consultar la base de datos)
        public DateTime Fecha { get; set; }  // Fecha en la que se emitió la factura
        public decimal Total { get; set; }  // Total de la factura

        public bool Estado { get; set; }  // Indica si la factura está activa (true) o anulada (false)

        // Propiedad calculada que devuelve el estado como texto
        public string EstadoTexto => Estado ? "Activa" : "Anulada";

        // Relación con Cliente
        public Cliente Cliente { get; set; }

        // Relación con DetalleFactura
        public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    }
}
