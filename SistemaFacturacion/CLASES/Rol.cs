using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES
{
    public class Rol
    {
        public int RolID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public Rol() { }

        public Rol(int roleId, string nombre, string descripcion)
        {
            RolID = roleId;
            Nombre = nombre;
            Descripcion = descripcion;
        }
    }
}
