using System.ComponentModel.DataAnnotations;

namespace WasmProject.Models
{
    public class LostReport
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        public int PropertyID { get; set; } 

        [Required(ErrorMessage = "يرجى تحديد تاريخ الفقدان")]
        public DateTime DateLost { get; set; }

        [Required(ErrorMessage = "يرجى ذكر الموقع التقريبي للفقدان")]
        public string Location { get; set; }

        [StringLength(500)]
        public string AdditionalDetails { get; set; }

        public DateTime ReportDate { get; set; } = DateTime.Now; 

        public bool IsFound { get; set; } = false; 
    }
}