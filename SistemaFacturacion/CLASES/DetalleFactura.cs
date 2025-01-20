using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.Clases
{
    public class DetalleFactura
    {
        public int IdDetalle { get; set; }
        public int IdFactura { get; set; }
        public int IdProducto { get; set; }
        public Producto Producto { get; set; } // Relación con el producto
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario; // Calculado
        public decimal ImpuestoPorProducto { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalLinea => Subtotal + ImpuestoPorProducto - Descuento;
        public string DescripcionProducto { get; set; }
        public byte Estado { get; set; } // 1: Activa, 2: Anulada, 3: Devuelta
                                         // Propiedad calculada que extrae el nombre del producto
        public string NombreProducto => Producto?.Nombre;  // Propiedad para obtener el nombre del producto
    }

}
