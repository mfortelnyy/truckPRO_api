using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using truckPRO_api.Models;

namespace truckPro_api.DTOs
{
    public class LogEntryParent
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime StartTime { get; set; }
        
        //start of shift is required, end of the shift is determined by the driver
        public DateTime? EndTime { get; set; }

        public LogEntryType LogEntryType { get; set; }

        //store list of images urls for the shift, nullable
        public List<string>? ImageUrls { get; set; }

        // store parent log entry id
        // onduty shift log id - driving, break. 
        // offduty shift log id - break which counts as sleep
        public int? ParentLogEntryId { get; set; }
        public List<LogEntry>? ChildLogEntries { get; set; }
        public bool IsApprovedByManager { get; set; }  
    }
}