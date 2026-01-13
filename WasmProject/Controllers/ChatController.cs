using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasmProject.Models;
using WasmProject.Services;

namespace WasmProject.Controllers
{
    public class ChatController : Controller
    {
        private readonly WasmDbContext _context;

        public ChatController(WasmDbContext context)
        {
            _context = context;
        }

        // عرض صفحة المحادثة بين الواجد والمالك
        public async Task<IActionResult> Index(int propertyId)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null) return NotFound();

            
            var messages = await _context.ChatMessages
                .Where(m => m.PropertyId == propertyId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.MessageContent = EncryptionService.Decrypt(msg.MessageContent);
            }

            ViewBag.Property = property;
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int propertyId, string messageText)
        {
            if (string.IsNullOrEmpty(messageText)) return RedirectToAction("Index", new { propertyId });

            var property = await _context.Properties.FindAsync(propertyId);

            var newMessage = new ChatMessage
            {
                PropertyId = propertyId,
                SenderId = HttpContext.Session.GetInt32("UserID") ?? 0, 
                ReceiverId = property.UserId ?? 0,
                MessageContent = EncryptionService.Encrypt(messageText), 
                SentAt = DateTime.Now
            };

            _context.ChatMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { propertyId });
        }
    }
}