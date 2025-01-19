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

        // Lista de permisos asociados a este rol
        public List<Permiso> Permisos { get; set; }

        // Constructor por defecto
        public Rol()
        {
            Permisos = new List<Permiso>(); // Inicializamos la lista de permisos
        }

        // Constructor con parámetros
        public Rol(int rolId, string nombre, string descripcion)
        {
            RolID = rolId;
            Nombre = nombre;
            Descripcion = descripcion;
            Permisos = new List<Permiso>(); // Inicializamos la lista de permisos
        }
    }
}

