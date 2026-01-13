using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WasmProject.Models;
using WasmProject.Services;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WasmProject.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly WasmDbContext _context;
        private readonly PropertySyncService _syncService;

        public PropertiesController(WasmDbContext context, PropertySyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        
        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserID");
        }

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var myProperties = await _context.Properties
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View("Properties_index", myProperties);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @property = await _context.Properties
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PropertyId == id);

            if (@property == null) return NotFound();

            return View(@property);
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            if (GetCurrentUserId() == null) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property)
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            
            ModelState.Remove("User");
            ModelState.Remove("UserId"); 
            ModelState.Remove("Status");
            ModelState.Remove("SourceType");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("IsVerified");

            if (!string.IsNullOrEmpty(property.SerialNumber) && property.SerialNumber.Length < 10)
            {
                ModelState.AddModelError("SerialNumber", "يجب أن يتكون الرقم التسلسلي من 10 خانات على الأقل.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    property.UserId = userId.Value;
                    property.CreatedAt = DateTime.Now;
                    property.Status = "Safe";
                    property.SourceType = "Manual";
                    property.IsVerified = false;

                    _context.Add(property);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "تم توثيق الملكية بنجاح في وسم";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "خطأ في قاعدة البيانات: " + ex.InnerException?.Message);
                }
            }
            return View(property);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncTest(IFormFile invoicePdf)
        {
            if (invoicePdf == null || invoicePdf.Length == 0) return RedirectToAction("Create");

            StringBuilder text = new StringBuilder();
            try
            {
                using (var stream = invoicePdf.OpenReadStream())
                using (var reader = new PdfReader(stream))
                using (var pdfDoc = new PdfDocument(reader))
                {
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)));
                    }
                }

                string content = text.ToString();

                var extractedProperty = new Property
                {
                    Brand = content.Contains("Apple") ? "Apple" : (content.Contains("Samsung") ? "Samsung" : "جهاز ذكي"),
                    SerialNumber = "SN-" + DateTime.Now.Ticks.ToString().Substring(10),
                    Category = "Electronics",
                    SourceType = "SmartSync",
                    CreatedAt = DateTime.Now
                };

                return View("SyncTest", extractedProperty);
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @property = await _context.Properties.FindAsync(id);
            if (@property == null) return NotFound();

            return View(@property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyId,UserId,Category,Brand,SerialNumber,Status,CreatedAt,SourceType")] Property @property)
        {
            if (id != @property.PropertyId) return NotFound();

            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.PropertyId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@property);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @property = await _context.Properties.FindAsync(id);
            if (@property == null) return NotFound();

            return View(@property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @property = await _context.Properties.FindAsync(id);
            if (@property != null)
            {
                _context.Properties.Remove(@property);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public IActionResult Transfer()
        {
            if (GetCurrentUserId() == null) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(string serialNumber, string newOwnerNationalID)
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.SerialNumber == serialNumber && p.UserId == userId);

            if (property == null)
            {
                ViewBag.Error = "العنصر غير موجود في سجلاتك.";
                return View();
            }

            var newOwner = await _context.UserDbs
                .FirstOrDefaultAsync(u => u.NationalID == newOwnerNationalID);

            if (newOwner == null)
            {
                ViewBag.Error = "الهوية الوطنية المدخلة غير مسجلة في نظام وسم.";
                return View();
            }

            property.UserId = newOwner.UserID;
            property.CreatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"تم نقل الملكية بنجاح إلى {newOwner.FullName}.";
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.PropertyId == id);
        }
    }
}