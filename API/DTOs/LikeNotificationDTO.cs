using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LikeNotificationDTO
    {
        public int UserId { get; set; }       // ID of the user who liked you
        public string Username { get; set; }  // username of user who liked you
        public string PhotoUrl { get; set; }      
        public DateTime CreatedAt { get; set; } 
    }
}