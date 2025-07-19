using API_Dinamita.Helpers;
using API_Dinamita.Models;
using API_Dinamita.ModelsDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace API_Dinamita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ContextDB _context;
        private readonly JwtHelper _jwtHelper;
        public LoginController(JwtHelper jwtHelper, ContextDB context)
        {
            _jwtHelper = jwtHelper;
            _context = context;
        }

        // POST: api/Login
        [HttpPost]
        public async Task<IActionResult> PostLogin(UsuarioDto usuario)
        {
            var persona = await _context.Personas.FirstOrDefaultAsync(U => U.NombreUsuario == usuario.NombreUsuario);
            if (persona == null || !BCrypt.Net.BCrypt.Verify(usuario.Contrasena, persona.PasswordHash))
            {
                return Unauthorized("Credenciales inválidas");
            }

            var token = _jwtHelper.GenerateToken(persona);
            var user = new UsuarioFront
            {
                N_Identificacion = persona.N_Identificacion,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                Telefono = persona.Telefono,
                NombreUsuario = persona.NombreUsuario,
                Correo = persona.Correo,
                Rol = persona.Rol
            };

            return Ok(new
            {
                token,
                user
            });
        }


        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(UsuarioRegister usuarioR)
        {
            if (_context.Personas.Any(P => P.NombreUsuario == usuarioR.NombreUsuario))
                return BadRequest("El usuario ya existe.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuarioR.PasswordHash);

            var user = new Personas
            {
                Nombre = usuarioR.Nombre,
                Apellido = usuarioR.Apellido,
                Telefono = usuarioR.Telefono,
                NombreUsuario = usuarioR.NombreUsuario,
                PasswordHash = hashedPassword,
                Correo = usuarioR.Correo,
                Rol = "Usuario"
            };

            _context.Personas.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente.");
        }
    }
}
