using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WasmProject.Controllers
{
    public class ChatbotController : Controller
    {
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult GetResponse(string userMessage)
        {
            
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return Json(new { response = "حياك الله في وسم.. كيف أقدر أخدمك اليوم؟" });
            }

            string botResponse = "";
            string message = userMessage.Trim().ToLower();

            
            if (message.Contains("تسجيل") || message.Contains("وثق"))
            {
                botResponse = "لتوثيق ملكيتك، اذهب إلى صفحة 'تسجيل ملكية'. يمكنك استخدام المزامنة الذكية لرفع فاتورة PDF وسنقوم بالباقي!";
            }
            else if (message.Contains("فقدت") || message.Contains("ضايع") || message.Contains("مسروق"))
            {
                botResponse = "نأسف لسماع ذلك. يرجى التوجه لصفحة 'إبلاغ عن فقدان' فوراً لتسجيل حالة الغرض في النظام وتنبيه الآخرين.";
            }
            else if (message.Contains("نقل") || message.Contains("تنازل"))
            {
                botResponse = "نقل الملكية يتطلب وجود رقم هوية المالك الجديد. اذهب لصفحة 'نقل ملكية' لإتمام الإجراء بشكل آمن.";
            }
            else if (message.Contains("وسم") || message.Contains("من انتم") || message.Contains("ماهو"))
            {
                botResponse = "وسم هو نظام وطني لتوثيق الملكية وحماية الممتلكات الشخصية عبر الأرقام التسلسلية، لضمان حقك وسهولة استعادة مفقوداتك.";
            }
            else if (message.Contains("سلام") || message.Contains("مرحب"))
            {
                botResponse = "وعليكم السلام ورحمة الله وبركاته، حياك الله في وسم. أنا مساعدك الرقمي، كيف أقدر أخدمك؟";
            }
            else
            {
                botResponse = "أهلاً بك في مساعد وسم. لم أفهم طلبك بدقة، هل تقصد الاستفسار عن (التوثيق، البلاغات، أو نقل الملكية)؟";
            }

            return Json(new { response = botResponse });
        }
    }
}