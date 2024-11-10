using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }  // Primary key for notification

        public int UserId { get; set; } 
        public User User { get; set; }  

        public string Message { get; set; }  

        public DateTime CreatedAt { get; set; } 

        public bool IsRead { get; set; }  
    }

}
