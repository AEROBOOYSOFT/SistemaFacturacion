using System.Linq;

using SistemaFacturacion.CLASES_CRUD;
using SistemaFacturacion;
namespace SistemaFacturacion.CLASES
{
    public static class SesionActual
    {
        // Usuario actual en la sesión
        public static Usuario Usuario { get; set; }

        // Método para cerrar sesión
        public static void CerrarSesion()
        {
            Usuario = null;  // Reseteamos el objeto Usuario
        }

        // Verificar si el usuario tiene un rol indicado
        public static bool TieneRol(string rol)
        {
            if (Usuario == null || Usuario.Roles == null)
                return false;

            // Verifica si el nombre del rol coincide con alguno de los roles del usuario
            return Usuario.Roles.Any(r => r.Nombre == rol);
        }

        // Verificar si el usuario tiene un permiso específico
        public static bool TienePermiso(string nombrePermiso)
        {
            if (Usuario == null || Usuario.Roles == null)
                return false;

            // Verifica si el usuario tiene el permiso a través de alguno de sus roles
            foreach (var rol in Usuario.Roles)
            {
                if (rol.Permisos.Any(p => p.NombrePermiso == nombrePermiso))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
