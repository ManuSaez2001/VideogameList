using DTOs;
using LogicaAplicacion.InterfacesCasosUso;
using LogicaNegocio.Dominio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Token;

namespace VideogameListMVC.Controllers {
    public class UserController : Controller {
        ICURegister cURegister;
        ICULogin cULogin;
        private readonly JwtOptions jwtOptions;
        public UserController(ICURegister cURegister, ICULogin cULogin, JwtOptions jwtOptions) {
            this.cURegister = cURegister;
            this.cULogin = cULogin;
            this.jwtOptions = jwtOptions;
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
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(DTOUserRegister dto) {
            try {
                if(dto == null) {
                    return View();
                }
                cURegister.Register(dto);
                return RedirectToAction(nameof(Login));
            } catch {
                return View();
            }
        }
        public ActionResult Login() {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(DTOUserLogin dto) {
            try {
                if (dto == null) {
                    return View();
                }
                DTOUserLogged dtoLogged = cULogin.Login(dto);
                string nuevoJwt = ManejadorToken.CrearToken(dtoLogged, jwtOptions);
                Response.Cookies.Append("JWTToken", nuevoJwt, new CookieOptions {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddSeconds(jwtOptions.TiempoVida),
                    Path = "/"
                });
                return RedirectToAction(nameof(Index));
            } catch {
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
