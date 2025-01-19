using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES
{
    public class MovimientoInventario
    {
        public int MovimientoID { get; set; }
        public int ProductoID { get; set; }
        public string TipoMovimiento { get; set; } // Entrada, Salida, Ajuste
        public int Cantidad { get; set; }
        public DateTime FechaMovimiento { get; set; } = DateTime.Now;
        public string Descripcion { get; set; }
    }
}
