using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasmProject.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; } 

        [Required]
        public int SenderId { get; set; } 

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string MessageContent { get; set; } 

        public DateTime SentAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        
        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }
    }
}