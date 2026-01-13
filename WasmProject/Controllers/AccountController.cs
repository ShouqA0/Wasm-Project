using Microsoft.AspNetCore.Mvc;
using WasmProject.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;

namespace WasmProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly WasmDbContext _context;
        private readonly IDataProtector _protector;

        public AccountController(WasmDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("Wasm.RecoveryToken.v1");
        }


        [HttpGet]
        public IActionResult Login() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string VisualPatternHash)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(VisualPatternHash))
            {
                ViewBag.Error = "يرجى إدخال اسم المستخدم ورسم النمط الخاص بك.";
                return View();
            }

            var user = await _context.UserDbs
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                ViewBag.Error = "عذراً، هذا المستخدم غير مسجل في نظام وسم.";
                return View();
            }

            if (user.VisualPatternHash != VisualPatternHash)
            {
                ViewBag.Error = "النمط المرئي غير صحيح، يرجى المحاولة مرة أخرى.";
                ViewBag.AttemptedUsername = username;
                return View();
            }

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserName", user.UserName);

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public IActionResult Register() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserDb user)
        {
            try
            {
                
               
                ModelState.Remove("CreatedAt");
                ModelState.Remove("Properties"); 

                
                if (string.IsNullOrEmpty(user.UserName) ||
                    string.IsNullOrEmpty(user.VisualPatternHash) ||
                    string.IsNullOrEmpty(user.NationalID))
                {
                    ViewBag.Error = "يرجى تعبئة جميع الحقول الأساسية (اسم المستخدم، النمط، والهوية الوطنية)";
                    return View(user);
                }

                
                bool userExists = await _context.UserDbs
                    .AnyAsync(u => u.UserName == user.UserName || u.NationalID == user.NationalID);

                if (userExists)
                {
                    ViewBag.Error = "عذراً، اسم المستخدم أو رقم الهوية الوطنية مسجل مسبقاً في نظام وسم.";
                    return View(user);
                }

                
                user.CreatedAt = DateTime.Now;
                if (string.IsNullOrEmpty(user.Email))
                {
                    user.Email = $"{user.UserName}@wasm.sa";
                }

                
                _context.UserDbs.Add(user);
                var saveResult = await _context.SaveChangesAsync();

                
                if (saveResult > 0)
                {
                    
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("UserName", user.UserName);

                    
                    return RedirectToAction("Index", "Properties");
                }
                else
                {
                    ViewBag.Error = "فشل في حفظ البيانات، يرجى المحاولة مرة أخرى.";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                
                ViewBag.Error = "خطأ تقني: " + (ex.InnerException?.Message ?? ex.Message);
                return View(user);
            }
        }



       
        [HttpGet]
        public IActionResult ForgotPattern() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPattern(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "يرجى إدخال البريد الإلكتروني.";
                return View();
            }

            var user = await _context.UserDbs.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "هذا البريد الإلكتروني غير مرتبط بأي حساب في وسم.";
                return View();
            }

            var expiryTime = DateTime.UtcNow.AddHours(1).Ticks;
            var rawData = $"{user.Email}|{expiryTime}";
            string token = _protector.Protect(rawData);

            string resetLink = Url.Action("ResetPattern", "Account", new { t = token }, Request.Scheme);

            ViewBag.Success = "تم التحقق من هويتك، استخدم الرابط أدناه لتحديث النمط:";
            ViewBag.Link = resetLink;

            return View();
        }

        [HttpGet]
        public IActionResult ResetPattern(string t)
        {
            if (string.IsNullOrEmpty(t)) return RedirectToAction("Login");

            try
            {
                string decryptedData = _protector.Unprotect(t);
                string[] parts = decryptedData.Split('|');
                string email = parts[0];
                long expiryTicks = long.Parse(parts[1]);

                if (DateTime.UtcNow > new DateTime(expiryTicks))
                {
                    ViewBag.Error = "انتهت صلاحية الرابط.";
                    return View("ForgotPattern");
                }

                ViewBag.Email = email;
                ViewBag.Token = t;
                return View();
            }
            catch
            {
                ViewBag.Error = "رابط غير صالح.";
                return View("ForgotPattern");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePattern(string email, string newPatternHash)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPatternHash))
                return BadRequest("بيانات ناقصة.");

            var user = await _context.UserDbs.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.VisualPatternHash = newPatternHash;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        public IActionResult AdminLogin() => View();
    }
}