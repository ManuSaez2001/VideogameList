using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VideogameListMVC.Controllers {
    [Authorize]
    public class GameListController : Controller {
        // GET: GameListController
        public ActionResult Index() {
            return View();
        }

        // GET: GameListController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: GameListController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: GameListController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: GameListController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: GameListController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: GameListController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: GameListController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }
    }
}
