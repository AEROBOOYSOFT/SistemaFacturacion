using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFacturacion.CLASES
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; } // Almacenar encriptada
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int RolID { get; internal set; }
        public Rol Rol { get; internal set; }

        public Usuario() { }

        public Usuario(int usuarioId, string nombreCompleto, string email, string nombreUsuario, string contraseña, bool activo, DateTime fechaCreacion)
        {
            UsuarioID = usuarioId;
            NombreCompleto = nombreCompleto;
            Email = email;
            NombreUsuario = nombreUsuario;
            Contraseña = contraseña;
            Activo = activo;
            FechaCreacion = fechaCreacion;
        }
    }
}
