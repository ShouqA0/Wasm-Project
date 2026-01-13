using Microsoft.AspNetCore.Mvc;
using WasmProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasmProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly WasmDbContext _context;

        public AdminController(WasmDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult Login()
        {
           
            return View("AdminLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string adminEmail, string password)
        {
            
            if (adminEmail == "admin@wasm.sa" && password == "Wasm2026")
            {
                HttpContext.Session.SetString("AdminRole", "SuperAdmin");
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "بيانات الدخول غير صحيحة.";
            return View("AdminLogin"); 
        }

        
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminRole");
            return RedirectToAction("Login");
        }

        
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            
            if (HttpContext.Session.GetString("AdminRole") != "SuperAdmin")
            {
                return RedirectToAction("Login");
            }

            var users = await _context.UserDbs.ToListAsync();

            
            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalProperties = await _context.Properties.CountAsync();
            ViewBag.LostReports = await _context.Properties.CountAsync(p => p.Status == "Lost");

            
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            var chartData = new List<int>();
            foreach (var day in last7Days)
            {
                var nextDay = day.AddDays(1);
                
                int count = await _context.Properties
                    .CountAsync(p => p.CreatedAt >= day && p.CreatedAt < nextDay);
                chartData.Add(count);
            }

            ViewBag.ChartLabels = last7Days.Select(d => d.ToString("MM/dd")).ToList();
            ViewBag.ChartData = chartData;

            return View(users);
        }

        
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (HttpContext.Session.GetString("AdminRole") != "SuperAdmin") return RedirectToAction("Login");

            var user = await _context.UserDbs.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int UserID, string deletionReason)
        {
            if (HttpContext.Session.GetString("AdminRole") != "SuperAdmin") return RedirectToAction("Login");

            
            var user = await _context.UserDbs
                .Include(u => u.Properties) 
                .FirstOrDefaultAsync(u => u.UserID == UserID);

            if (user != null)
            {
                try
                {
                    
                    if (user.Properties != null && user.Properties.Any())
                    {
                        _context.Properties.RemoveRange(user.Properties);
                    }

                    
                    _context.UserDbs.Remove(user);

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "تم حذف الحساب وجميع الممتلكات المرتبطة به بنجاح.";
                    return RedirectToAction("Dashboard");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "لا يمكن حذف المستخدم لوجود سجلات مرتبطة أخرى: " + ex.Message;
                    return View("ConfirmDelete", user);
                }
            }
            return NotFound();
        }
    }
}