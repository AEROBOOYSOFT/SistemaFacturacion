using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES
{
    public class UsuarioRol
    {
        public int UsuarioID { get; set; }
        public int RoleID { get; set; }
        public DateTime FechaAsignacion { get; set; }

        public UsuarioRol() { }

        public UsuarioRol(int usuarioId, int roleId, DateTime fechaAsignacion)
        {
            UsuarioID = usuarioId;
            RoleID = roleId;
            FechaAsignacion = fechaAsignacion;
        }
    }
}
