using DTOs;
using LogicaAplicacion.InterfacesCasosUso;
using LogicaNegocio.Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Token;

namespace VideogameListMVC.Controllers {
    public class UserController : Controller {
        ICURegister cURegister;
        ICULogin cULogin;
        private readonly JwtOptions jwtOptions;
        private readonly ILogger<UserController> _logger;

        public UserController(ICURegister cURegister, ICULogin cULogin, JwtOptions jwtOptions, ILogger<UserController> logger) {
            this.cURegister = cURegister;
            this.cULogin = cULogin;
            this.jwtOptions = jwtOptions;
            this._logger = logger;
        }
        // GET: UserController
        public ActionResult Index() {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: UserController/Create
        public ActionResult SignUp() {
            // Si el usuario ya está autenticado, redirigir a UserIndex
            if (User.Identity?.IsAuthenticated == true) {
                _logger.LogInformation($"Usuario autenticado {User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value} intentó acceder a signup, redirigiendo a UserIndex");
                return RedirectToAction("UserIndex", "Home");
            }
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(DTOUserRegister dto) {
            try {
                if(dto == null) {
                    return View();
                }
                await cURegister.RegisterAsync(dto);
                return RedirectToAction(nameof(Login));
            } catch {
                return View();
            }
        }
        public ActionResult Login() {
            // Si el usuario ya está autenticado, redirigir a UserIndex
            if (User.Identity?.IsAuthenticated == true) {
                _logger.LogInformation($"Usuario autenticado {User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value} intentó acceder a login, redirigiendo a UserIndex");
                return RedirectToAction("UserIndex", "Home");
            }

            // Limpiar cualquier cookie JWT vieja al llegar a la página de login
            if (Request.Cookies.ContainsKey("JWTToken"))
            {
                Response.Cookies.Delete("JWTToken");
                _logger.LogInformation("Cookie JWT eliminada al acceder a la página de login");
            }

            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(DTOUserLogin dto) {
            try {
                if (dto == null) {
                    ModelState.AddModelError("", "El formulario de login no es válido");
                    return View();
                }
                DTOUserLogged dtoLogged = await cULogin.LoginAsync(dto);
                string nuevoJwt = ManejadorToken.CrearToken(dtoLogged, jwtOptions);
                Response.Cookies.Append("JWTToken", nuevoJwt, new CookieOptions {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddSeconds(jwtOptions.TiempoVida),
                    Path = "/"
                });
                _logger.LogInformation($"Usuario {dto.Mail} ha iniciado sesión correctamente");
                return RedirectToAction("UserIndex", "Home");
            } catch (Exception ex) {
                _logger.LogError($"Error en login para usuario {dto?.Mail}: {ex.Message}");
                ModelState.AddModelError("", $"Error al iniciar sesión: {ex.Message}");
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: UserController/Logout
        [HttpGet]
        public ActionResult LogoutGet() {
            Response.Cookies.Delete("JWTToken");
            return RedirectToAction("Login");
        }

        // GET: UserController/Profile
        public ActionResult Profile() {
            return View();
        }
    }
}
