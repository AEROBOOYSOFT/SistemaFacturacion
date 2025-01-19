using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.Clases
{
    public class Configuracion
    {
        public int IdConfiguracion { get; set; }
        public string NombreEmpresa { get; set; }
        public string Ruc { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public decimal ImpuestoITBIS { get; set; }
    }
}
