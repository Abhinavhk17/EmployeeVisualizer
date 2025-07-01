using Newtonsoft.Json;

namespace EmployeeVisualizer.Models;

// Time entry record from API
public class TimeEntry
{
    public string Id { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    
    [JsonProperty("StarTimeUtc")]
    public DateTime StartTimeUtc { get; set; }
    
    public DateTime EndTimeUtc { get; set; }
    public string EntryNotes { get; set; } = string.Empty;
    public DateTime? DeletedOn { get; set; }
    
    // Calculate total hours worked
    public double GetTotalHours()
    {
        if (EndTimeUtc < StartTimeUtc)
        {
            return 0;
        }
        
        return (EndTimeUtc - StartTimeUtc).TotalHours;
    }
}
