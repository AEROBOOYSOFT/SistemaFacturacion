using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES
{
    public class Pago
    {
        public int PagoID { get; set; }
        public int FacturaID { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; }
        public string Observaciones { get; set; }
        public int UsuarioID { get; set; } 
    }

}
