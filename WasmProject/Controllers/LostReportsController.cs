using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasmProject.Models;

namespace WasmProject.Controllers
{
    public class LostReportsController : Controller
    {
        private readonly WasmDbContext _context;

        public LostReportsController(WasmDbContext context)
        {
            _context = context;
        }

        // صفحة تقديم البلاغ
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Report(int? propId)
        {
            int? userid = HttpContext.Session.GetInt32("UserID");

            var query = _context.Properties.AsQueryable();

            if (userid != null)
            {
                query = query.Where(p => p.UserId == userid);
            }

            var userProperties = await query.ToListAsync();

            ViewBag.UserProperties = userProperties;
            ViewBag.UserReports = await _context.LostItems
                .Include(l => l.Property)
                .Where(l => l.Property.UserId == userid) 
                .OrderByDescending(l => l.LostDate)
                .ToListAsync();
            

            ViewBag.SelectedPropId = propId;

            var model = new LostItem
            {
                PropertyId = propId ?? 0,
                LostDate = DateTime.Now
            };

            return View("Report", model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReport(LostItem report)
        {
            ModelState.Remove("Property");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.LostItems.Add(report);

                    var property = await _context.Properties.FindAsync(report.PropertyId);
                    if (property != null)
                    {
                        property.Status = "Lost";
                        _context.Update(property);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Properties");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "خطأ في الحفظ: " + ex.Message);
                }
            }

            ViewBag.UserProperties = await _context.Properties.ToListAsync();
            return View("Report", report);
        }

        
        public async Task<IActionResult> Index()
        {
            var reports = await _context.LostItems.Include(l => l.Property).ToListAsync();
            return View(reports);
        }
    }
}