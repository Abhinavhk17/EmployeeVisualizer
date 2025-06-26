using EmployeeVisualizer.Models;
using Newtonsoft.Json;

namespace EmployeeVisualizer.Services;

/// <summary>
/// Service class responsible for fetching employee time entry data from the REST API
/// and processing it into usable business objects.
/// 
/// Core Responsibilities:
/// 1. Makes HTTP GET requests to the external API endpoint
/// 2. Deserializes JSON response data into TimeEntry objects
/// 3. Filters and validates the raw data
/// 4. Aggregates time entries by employee to calculate total working hours
/// 5. Sorts employees by total hours worked (descending order)
/// 6. Manages HTTP client lifecycle and resource cleanup
/// 
/// Data Processing Logic:
/// - Excludes deleted time entries (DeletedOn != null)
/// - Excludes entries with missing employee names
/// - Groups remaining entries by employee name
/// - Sums up total hours per employee using TimeEntry.GetTotalHours()
/// - Orders results by total hours worked (highest first)
/// </summary>
public class EmployeeDataService
{
    private readonly HttpClient _httpClient;
    
    // API endpoint URL with authentication code
    // This is the provided REST API that returns employee time entry data in JSON format
    private const string ApiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

    /// <summary>
    /// Initializes the service with a new HTTP client for making API requests
    /// </summary>
    public EmployeeDataService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Fetches time entries from the external REST API endpoint.
    /// 
    /// Process:
    /// 1. Makes HTTP GET request to the API URL
    /// 2. Receives JSON response containing array of time entry objects
    /// 3. Deserializes JSON into List<TimeEntry> using Newtonsoft.Json
    /// 4. Filters out invalid/deleted entries
    /// 5. Returns clean list of active time entries
    /// 
    /// Data Filtering:
    /// - Excludes entries where DeletedOn is not null (deleted entries)
    /// - Excludes entries with null or empty EmployeeName
    /// 
    /// Error Handling:
    /// - Catches any HTTP or deserialization exceptions
    /// - Logs error messages to console
    /// - Returns empty list on failure to prevent application crash
    /// 
    /// Returns:
    /// - List<TimeEntry>: Clean list of active time entries
    /// - Empty list if API call fails or no valid data found
    /// </summary>
    public async Task<List<TimeEntry>> GetTimeEntriesAsync()
    {
        try
        {
            // Make HTTP GET request to fetch JSON data
            var response = await _httpClient.GetStringAsync(ApiUrl);
            
            // Deserialize JSON response into strongly-typed objects
            var timeEntries = JsonConvert.DeserializeObject<List<TimeEntry>>(response) ?? new List<TimeEntry>();
            
            // Filter out deleted entries and entries without employee names
            // This ensures data quality for our calculations and reports
            return timeEntries
                .Where(entry => entry.DeletedOn == null && !string.IsNullOrEmpty(entry.EmployeeName))
                .ToList();
        }
        catch (Exception ex)
        {
            // Log error and return empty list to prevent application crash
            Console.WriteLine($"Error fetching data: {ex.Message}");
            return new List<TimeEntry>();
        }
    }

    /// <summary>
    /// Groups time entries by employee and calculates total hours worked per person.
    /// 
    /// Processing Steps:
    /// 1. Groups all time entries by EmployeeName
    /// 2. For each employee group, sums up total hours from all their time entries
    /// 3. Creates EmployeeSummary objects with name and total hours
    /// 4. Sorts results in descending order by total hours (highest first)
    /// 
    /// Business Logic:
    /// - Uses TimeEntry.GetTotalHours() method for accurate hour calculations
    /// - Handles multiple time entries per employee correctly
    /// - Produces sorted list suitable for reports and visualizations
    /// 
    /// Parameters:
    /// - timeEntries: Pre-filtered list of valid TimeEntry objects
    /// 
    /// Returns:
    /// - List<EmployeeSummary>: Sorted list of employees with their total working hours
    /// - Empty list if no valid time entries provided
    /// </summary>
    public List<EmployeeSummary> CalculateEmployeeSummaries(List<TimeEntry> timeEntries)
    {
        return timeEntries
            .GroupBy(entry => entry.EmployeeName)        // Group by employee name
            .Select(group => new EmployeeSummary         // Create summary for each employee
            {
                EmployeeName = group.Key!,               // Employee name from group key
                TotalHours = group.Sum(entry => entry.GetTotalHours()) // Sum all hours for this employee
            })
            .OrderByDescending(summary => summary.TotalHours) // Sort by total hours (highest first)
            .ToList();
    }

    /// <summary>
    /// Properly disposes of the HTTP client to free up system resources.
    /// Should be called when the service is no longer needed.
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

/// <summary>
/// Data transfer object representing an employee's total working hours summary.
/// Used to aggregate and display employee time data in reports and visualizations.
/// 
/// Purpose:
/// - Holds calculated total hours per employee
/// - Used by HTML table generator for creating reports
/// - Used by pie chart generator for creating visual representations
/// - Provides clean, simple structure for UI binding
/// 
/// Properties:
/// - EmployeeName: Full name of the employee
/// - TotalHours: Sum of all working hours across all time entries
/// </summary>
public class EmployeeSummary
{
    /// <summary>
    /// Full name of the employee
    /// Used as the primary identifier and display text
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;
    
    /// <summary>
    /// Total number of hours worked by this employee
    /// Calculated by summing all time entries for this person
    /// Used for sorting, highlighting, and percentage calculations
    /// </summary>
    public double TotalHours { get; set; }
}
