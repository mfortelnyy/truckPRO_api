using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace truckapi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; } // user who will receive the notification 
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsRead { get; set; } = false; //track if the notification has been read

    }
}