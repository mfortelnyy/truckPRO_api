using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace truckPRO_api.Models
{
    public class LogEntry
    {
        [Key]
        public int Id { get; set; }
        
        //foreign key -> user (driver)
        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime StartTime { get; set; }
        //start of shift is required, end of the shift is determined by the driver
        public DateTime? EndTime { get; set; }
        
        [Required]
        [EnumDataType(typeof(LogEntryType))]
        public LogEntryType LogEntryType { get; set; }

        //store list of images for the shift, nullable
        public IFormFileCollection? Images { get; set; }


    }
}
