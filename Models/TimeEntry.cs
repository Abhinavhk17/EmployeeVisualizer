using Newtonsoft.Json;

namespace EmployeeVisualizer.Models;

public class TimeEntry
{
    public string Id { get; set; } = "";
    public string? EmployeeName { get; set; }
    
    [JsonProperty("StarTimeUtc")]
    public DateTime StartTimeUtc { get; set; }
    
    public DateTime EndTimeUtc { get; set; }
    public string EntryNotes { get; set; } = "";
    public DateTime? DeletedOn { get; set; }
    
    public double GetTotalHours()
    {
        var hours = (EndTimeUtc - StartTimeUtc).TotalHours;
        return hours > 0 ? hours : 0;
    }
}
