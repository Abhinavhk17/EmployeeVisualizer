using Newtonsoft.Json;

namespace EmployeeVisualizer.Models;

public class TimeEntry
{
    public string Id { get; set; } = "";
    public string? EmployeeName { get; set; }

    // Note: API property is misspelled as 'StarTimeUtc'
    [JsonProperty("StarTimeUtc")]
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public string EntryNotes { get; set; } = "";
    public DateTime? DeletedOn { get; set; }

    // Returns the total hours for this entry, never negative
    public double GetTotalHours()
    {
        double hours = (EndTimeUtc - StartTimeUtc).TotalHours;
        return hours > 0 ? hours : 0;
    }
}
