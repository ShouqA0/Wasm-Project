//المسؤول عن معالجة رسائل المتاجر الموثقه


using WasmProject.Models;
using Microsoft.EntityFrameworkCore;

namespace WasmProject.Services
{
    public class PropertySyncService
    {
        private readonly WasmDbContext _context;

        public PropertySyncService(WasmDbContext context)
        {
            _context = context;
        }
        public async Task<string> ProcessIncomingSms(string senderId, string messageBody, string userPhone)
        {
            
            var store = await _context.TrustedStores
                .FirstOrDefaultAsync(s => s.SenderId == senderId.ToUpper() && s.IsActive == true);

            if (store == null) return "Sender not trusted.";

            
            string productName = messageBody.Contains("iPhone") ? "iPhone 15 Pro" : "Electronic Device";

            
            string serialNumber = messageBody.Contains("SN:") ? "SN998877" : null;

            if (!string.IsNullOrEmpty(serialNumber))
            {
                var newProperty = new Property
                {
                    Brand = store.DisplayName,
                    Category = productName, 
                    SerialNumber = serialNumber,
                    IsVerified = true,
                    SourceType = "SMS", 
                    CreatedAt = DateTime.Now,
                    UserId = 1 
                };
                _context.Properties.Add(newProperty);
            }
            else
            {
                _context.PendingSyncs.Add(new PendingSync
                {
                    UserPhone = userPhone,
                    DetectedItemName = productName,
                    SenderId = senderId,
                    SourceType = "SMS", 
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return "Processed successfully.";
        }
    }
}
