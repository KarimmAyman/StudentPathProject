using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public enum NotificationTypeEnum
    {
        Info,
        Warning,
        Error,
        Success,
        Reminder
    }
    public class Notification
    {
        public int NotificationId { get; set; }  // Primary key for notification
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [StringLength(500)]  // Limiting message length, adjust as needed
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default to current UTC time

        public bool IsRead { get; set; } = false;  // Default to unread

        // Optional: add a NotificationType field to categorize notifications
        public NotificationTypeEnum? NotificationType { get; set; }

        // Optional: Soft deletion flag
        public bool IsDeleted { get; set; } = false;
    }




}
