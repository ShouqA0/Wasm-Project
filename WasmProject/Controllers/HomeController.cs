using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WasmProject.Models;

namespace WasmProject.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly WasmDbContext _context;

        
        public HomeController(WasmDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        [AllowAnonymous]
        public IActionResult SearchOwner()
        {
            return View();
        }

        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> FindResult(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                ViewBag.Message = "يرجى إدخال الرقم التسلسلي للبحث.";
                return View("SearchOwner");
            }

            
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.SerialNumber == serialNumber);

            if (property == null)
            {
                ViewBag.Message = "هذا الرقم التسلسلي غير مسجل في نظام وسم حالياً.";
                return View("SearchOwner");
            }

            
            if (property.Status == "Lost")
            {
                
                return View("OwnerFound", property);
            }
            else
            {
                ViewBag.Message = "هذا الرقم مسجل، ولكن لم يتم التبليغ عن فقده بعد.";
                return View("SearchOwner");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About()
        {
            return View();
        }
    }
}