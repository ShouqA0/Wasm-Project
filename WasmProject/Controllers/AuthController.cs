using Microsoft.AspNetCore.Mvc;
using WasmProject.Models;

namespace WasmProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly WasmDbContext _Context;
        public AuthController(WasmDbContext context)
        {
            _Context = context;

        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserDb user, string patternData)
        {
            if (ModelState.IsValid)
            {
                user.VisualPatternHash = patternData;
                _Context.UserDbs.Add(user);
                _Context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}