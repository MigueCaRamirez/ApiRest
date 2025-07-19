
namespace API_Dinamita.ModelsDto
{
    public class UsuarioDto
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
    }

    public class UsuarioRegister
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public string Correo { get; set; }
    }

    public class UsuarioFront
    {
        public int N_Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
    }
}
