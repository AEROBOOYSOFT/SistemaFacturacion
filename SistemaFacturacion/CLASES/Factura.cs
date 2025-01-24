using System;
using System.Collections.Generic;

namespace SistemaFacturacion.Clases
{
    public class Factura
    {
        public int IdFactura { get; set; }  // Identificador único de la factura
        public int IdCliente { get; set; }  // Clave foránea que referencia al cliente
        public string NombreCliente => Cliente?.Nombre;  // Nombre del cliente
        public DateTime Fecha { get; set; }  // Fecha de emisión
        public decimal Subtotal { get; set; }  // Monto antes de impuestos
        public decimal Impuestos { get; set; }  // Monto de impuestos calculados
        public decimal Total { get; set; }  // Total después de impuestos
        public bool Estado { get; set; }  // Indica si está activa o anulada
        public string EstadoTexto => Estado ? "Activa" : "Anulada";  // Texto para el estado
        public bool Pagada { get; set; }  // Indica si la factura ha sido pagada
        public decimal SaldoPendiente { get; set; }  // Monto pendiente de pago
        public Cliente Cliente { get; set; }  // Relación con Cliente
        public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();  // Relación con DetalleFactura
    }

}