using System.ComponentModel.DataAnnotations;

namespace WasmProject.Models
{
    public class UserDb
    {
        [Key]
        public int UserID { get; set; } 

        [Required]
        public string UserName { get; set; } 

        [Required]
        public string FullName { get; set; } 

        [Required]
        public string NationalID { get; set; } 

        public string PhoneNumber { get; set; } 

        [Required]
        public string VisualPatternHash { get; set; }

       
        [EmailAddress]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // لربط المستخدم بممتلكاته 
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}