using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.USUARIOS
{
    // Clase para representar un Permiso
    public class Permiso
    {
        public string NombrePermiso { get; set; }
    }

   

    // Clase estática para manejar la sesión actual
    public static class SesionActual
    {
        // Usuario actual en la sesión
        public static Usuario Usuario { get; set; }

        public static void CerrarSesion()
        {
            Usuario = null;  // Reseteamos el objeto Usuario
        }

        // Verificar si el usuario tiene el rol indicado
        public static bool TieneRol(string rol)
        {
            if (Usuario == null || Usuario.Permisos == null)
                return false;

            // Puedes cambiar la lógica si quieres verificar los roles de alguna manera.
            // Aquí se verifica si el nombre del permiso coincide con el rol
            return Usuario.Permisos.Any(p => p.NombrePermiso == rol);
        }

        // Método para verificar permisos a través de un nombre
        public static bool TienePermiso(string nombrePermiso)
        {
            if (Usuario == null || Usuario.Permisos == null)
                return false;

            // Verifica si el usuario tiene un permiso con el nombre indicado
            return Usuario.Permisos.Any(p => p.NombrePermiso == nombrePermiso);
        }
    }
}
