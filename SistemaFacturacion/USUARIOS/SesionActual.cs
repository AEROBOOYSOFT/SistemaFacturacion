using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.USUARIOS
{
    public static class SesionActual
    {
        public static int UsuarioID { get; set; }
        public static string NombreCompleto { get; set; }
        public static string NombreUsuario { get; set; }
        public static List<string> Roles { get; set; } = new List<string>();

        public static void CerrarSesion()
        {
            UsuarioID = 0;
            NombreCompleto = null;
            NombreUsuario = null;
            Roles.Clear();
        }

        public static bool TieneRol(string rol)
        {
            return Roles.Contains(rol);
        }
    }
}
