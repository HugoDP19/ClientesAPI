using ClientesAPI.Data;
using ClientesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Runtime.CompilerServices;
using System.Text;

namespace ClientesAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context; //Acceso BD
        private readonly IConfiguration _config; //Configuración

        //Constructor
        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //Registrar
        //POST: api/auth/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
        {
            //Si el usuario ya existe, cancelamos el proceso
            if (await _context.Usuarios.AnyAsync(u => u.Username == request.Username)) 
            {
                //Server code 409
                return Conflict(new { mensaje = "El usuario ya existe" });
            }

            //Sino existe, creamos uno nuevo utilizando el AuthDtos
            var usuario = new Usuario
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Rol = "Usuario"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            //Server code 200
            return Ok(new { mensaje= "Usuario registrado correctamente" });

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == request.Username);
            
            // ¿La contraseña se desencripta?
            //No, se utiliza un algoritmo matemático para   
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            {
                //Server code 401
                return Unauthorized(new { mensaje = "Usuario incorrecto" });
            }

            //var => declaración dinámica objetos
            //Deserealización
            var (token, expira) = GenerarToken(usuario);
            return Ok(new { token, expira });

        }

        //El método retornará:
        //token     : cadena larga de caracteres
        //fecha hora: vigencia del token
        private (string token, DateTime expira) GenerarToken(Usuario usuario)
        {
            //Json Web Token
            var jwt = _config.GetSection("Jwt");

            //Nota: Los parámetros ["Params"] vienen del archivo de configuración (JSON)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var expira = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"]!));

            //Configuraciones del algoritmo que emite el certificado y la firma de seguridad
            //algoritmos criptográficos
            var descriptor = new SecurityTokenDescriptor
            {
                //IDusuario, Nombre, Rol, Permiso
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Username),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Issuer = jwt["Issuer"],
                Audience = jwt["Audience"],
                Expires = expira,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);
            return (token, expira);
        }
    }
}
