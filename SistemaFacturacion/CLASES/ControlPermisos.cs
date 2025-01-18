using SistemaFacturacion.USUARIOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES
{
    public class ControlPermisos
    {
        public static bool TienePermiso(string nombrePermiso)
        {
            // Si no hay usuario activo o no tiene permisos cargados, devolver "false".
            if (SesionActual.Usuario == null || SesionActual.Usuario.Permisos == null)
                return false;

            // Verifica si el usuario tiene un permiso con el nombre indicado.
            return SesionActual.Usuario.Permisos.Any(p => p.NombrePermiso == nombrePermiso);
        }
    }
}
