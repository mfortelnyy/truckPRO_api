namespace truckPRO_api.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        //foreign key -> user (driver)
        public int userId { get; set; }
        public User User { get; set; }
        public DateTime startTime { get; set; }
        public DateTime? endTime { get; set; }
        public LogEntryType LogEntryType { get; set; }
        
        //store list of images for the shift, nullable


    }
}
