using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using ISERTEC_OC.Models;

namespace ISERTEC_OC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IConfiguration configuration;

        public UsuariosController(
        IConfiguration configuration
        )
        {
            this.configuration = configuration;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Usuario Usuario)
        {
            using var conexion = new SqlConnection(configuration.GetConnectionString("ConnectionISERTEC_OC"));
            Usuario value = conexion.QueryFirstOrDefault<Usuario>("EXEC SP_USUARIO  @Id,@Nombre, @UsuarioNombre, @Contrasenia, @Operacion",
                new
                {
                    Id=0,
                    Nombre="",
                    UsuarioNombre = Usuario.UsuarioNombre,
                    Contrasenia = Usuario.Contrasenia,
                    Operacion = "PorUser"
                });
            if (value == null)
            {
                return RedirectToAction("Login");
            }
            else {
                Sesion(value);
                return RedirectToAction("Index", "Orden"); 
            }
        }

        [HttpPost]
        public async Task<IActionResult> Salir()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Usuarios");
        }
        private async Task Sesion(Usuario usuario)
        {
            List<Claim> c = new List<Claim>()
                                {
                                        new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioNombre)

                                };

            ClaimsIdentity ci = new(c, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties p = new();

            p.AllowRefresh = true;
            p.IsPersistent = true;
            p.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ci), p);
        }
    }
}
