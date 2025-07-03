using EmployeeVisualizer.Models;
using Newtonsoft.Json;

namespace EmployeeVisualizer.Services;

public class EmployeeDataService
{
    private readonly HttpClient _httpClient = new();

    public async Task<List<EmployeeSummary>> GetEmployeeDataAsync()
    {
        try
        {
            string url = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
            var response = await _httpClient.GetStringAsync(url);
            var timeEntries = JsonConvert.DeserializeObject<List<TimeEntry>>(response) ?? new();

            // Filter, group, and summarize employee hours
            var employeeGroups = timeEntries
                .Where(e => e.DeletedOn == null && !string.IsNullOrEmpty(e.EmployeeName))
                .GroupBy(e => e.EmployeeName)
                .Select(g => new EmployeeSummary
                {
                    EmployeeName = g.Key!,
                    TotalHours = g.Sum(e => e.GetTotalHours())
                })
                .OrderByDescending(e => e.TotalHours)
                .ToList();

            return employeeGroups;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new List<EmployeeSummary>();
        }
    }
}

public class EmployeeSummary
{
    public string EmployeeName { get; set; } = "";
    public double TotalHours { get; set; }
}
