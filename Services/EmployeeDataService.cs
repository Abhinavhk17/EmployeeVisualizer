using EmployeeVisualizer.Models;
using Newtonsoft.Json;

namespace EmployeeVisualizer.Services;

// Service for fetching and processing employee time data from REST API
public class EmployeeDataService
{
    private readonly HttpClient _httpClient;
    
    // API endpoint with authentication
    private const string ApiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

    public EmployeeDataService()
    {
        _httpClient = new HttpClient();
    }

    // Fetches time entries from API and filters valid data
    public async Task<List<TimeEntry>> GetTimeEntriesAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(ApiUrl);
            var timeEntries = JsonConvert.DeserializeObject<List<TimeEntry>>(response) ?? new List<TimeEntry>();
            
            // Filter out deleted entries and empty names
            return timeEntries
                .Where(entry => entry.DeletedOn == null && !string.IsNullOrEmpty(entry.EmployeeName))
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            return new List<TimeEntry>();
        }
    }

    // Groups time entries by employee and calculates total hours
    public List<EmployeeSummary> CalculateEmployeeSummaries(List<TimeEntry> timeEntries)
    {
        return timeEntries
            .GroupBy(entry => entry.EmployeeName)
            .Select(group => new EmployeeSummary
            {
                EmployeeName = group.Key!,
                TotalHours = group.Sum(entry => entry.GetTotalHours())
            })
            .OrderByDescending(summary => summary.TotalHours)
            .ToList();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// Employee summary data for reports and charts
public class EmployeeSummary
{
    public string EmployeeName { get; set; } = string.Empty;
    public double TotalHours { get; set; }
}
