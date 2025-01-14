using System;
using System.Collections.Generic;

namespace SistemaFacturacion.Clases
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdCliente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        // Relación con Cliente
        public Cliente Cliente { get; set; }

        // Relación con DetalleFactura
        public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    }
}
