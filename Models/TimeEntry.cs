using Newtonsoft.Json;

namespace EmployeeVisualizer.Models;

/// <summary>
/// Represents a single time entry record from the API.
/// This model maps directly to the JSON structure returned by the REST API endpoint.
/// 
/// Purpose:
/// - Deserializes JSON data from the API into strongly-typed C# objects
/// - Provides properties that match the API response structure
/// - Includes business logic for calculating work hours
/// - Handles data validation and error cases
/// 
/// Usage:
/// - Created automatically during JSON deserialization
/// - Used to calculate total hours worked per employee
/// - Filtered to exclude deleted entries
/// </summary>
public class TimeEntry
{
    /// <summary>
    /// Unique identifier for this time entry record
    /// Used for tracking and data integrity
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the employee who created this time entry
    /// This is the key field used for grouping and aggregation
    /// Can be null if not provided in the API response
    /// </summary>
    public string? EmployeeName { get; set; }
    
    /// <summary>
    /// When the work period started (in UTC timezone)
    /// Note: The API uses "StarTimeUtc" (missing 't') so we map it correctly
    /// </summary>
    [JsonProperty("StarTimeUtc")]
    public DateTime StartTimeUtc { get; set; }
    
    /// <summary>
    /// When the work period ended (in UTC timezone)
    /// Used with StartTimeUtc to calculate total working hours
    /// </summary>
    public DateTime EndTimeUtc { get; set; }
    
    /// <summary>
    /// Optional notes or description for this time entry
    /// May contain details about what work was performed
    /// </summary>
    public string EntryNotes { get; set; } = string.Empty;
    
    /// <summary>
    /// Timestamp when this entry was deleted (if applicable)
    /// Null means the entry is active and should be included in calculations
    /// Non-null means the entry was deleted and should be excluded
    /// </summary>
    public DateTime? DeletedOn { get; set; }
    
    /// <summary>
    /// Calculates the total hours worked for this time entry.
    /// 
    /// Business Logic:
    /// - Subtracts StartTimeUtc from EndTimeUtc to get duration
    /// - Handles edge cases where end time is before start time (data errors)
    /// - Returns 0 for invalid time ranges to prevent negative hours
    /// 
    /// Returns:
    /// - Double representing hours worked (e.g., 8.5 for 8 hours 30 minutes)
    /// - 0 if the time range is invalid
    /// </summary>
    public double GetTotalHours()
    {
        // Handle cases where end time is before start time (likely data entry errors)
        if (EndTimeUtc < StartTimeUtc)
        {
            return 0;
        }
        
        // Calculate the difference and convert to total hours
        return (EndTimeUtc - StartTimeUtc).TotalHours;
    }
}
