using EmployeeVisualizer.Services;
using System.Diagnostics;

namespace EmployeeVisualizer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Employee Visualizer");
        Console.WriteLine("==================");
        Console.WriteLine();
        
        // Initialize services
        var dataService = new EmployeeDataService();
        var htmlGenerator = new HtmlTableGenerator()
        {
            ReportTitle = "Employee Time Analysis Report",
            PageTitle = "Company Time Tracker Dashboard", 
            LowHoursThreshold = 100.0,
            PrimaryColor = "#2e7d32",
            LowHoursColor = "#ffcdd2",
            LowHoursTextColor = "#d32f2f"
        };
        var chartGenerator = new PieChartGenerator();
        
        try
        {
            // Fetch data from API
            Console.WriteLine("Fetching employee time data from API...");
            var timeEntries = await dataService.GetTimeEntriesAsync();
            
            if (!timeEntries.Any())
            {
                Console.WriteLine("No time entries found or failed to fetch data.");
                return;
            }

            Console.WriteLine($"Successfully fetched {timeEntries.Count} time entries.");
            
            // Process and summarize data
            var employeeSummaries = dataService.CalculateEmployeeSummaries(timeEntries);
            
            Console.WriteLine($"Processed data for {employeeSummaries.Count} employees.");
            Console.WriteLine();
            
            // Display top employees
            Console.WriteLine("Top 10 Employees by Total Hours:");
            Console.WriteLine("================================");
            foreach (var employee in employeeSummaries.Take(10))
            {
                var status = employee.TotalHours < htmlGenerator.LowHoursThreshold ? $" (< {htmlGenerator.LowHoursThreshold} hours)" : "";
                Console.WriteLine($"{employee.EmployeeName}: {employee.TotalHours:F2} hours{status}");
            }
            Console.WriteLine();
            
            // Generate HTML report
            Console.WriteLine("Generating HTML table...");
            var htmlContent = htmlGenerator.GenerateEmployeeTable(employeeSummaries);
            var htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "employee_time_report.html");
            await htmlGenerator.SaveHtmlToFileAsync(htmlContent, htmlFilePath);
            
            // Generate pie chart
            Console.WriteLine("Generating pie chart...");
            using var pieChart = chartGenerator.GenerateEmployeePieChart(employeeSummaries);
            var chartFilePath = Path.Combine(Directory.GetCurrentDirectory(), "employee_time_piechart.png");
            chartGenerator.SaveChartToFile(pieChart, chartFilePath);
            
            Console.WriteLine();
            Console.WriteLine("Output Files:");
            Console.WriteLine($"- HTML Report: {htmlFilePath}");
            Console.WriteLine($"- Pie Chart: {chartFilePath}");
            Console.WriteLine();
            
            // Interactive file opening
            Console.WriteLine("Would you like to open the HTML report? (y/n): ");
            var openHtml = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;
            
            if (openHtml && File.Exists(htmlFilePath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = htmlFilePath,
                        UseShellExecute = true
                    });
                    Console.WriteLine("HTML report opened in default browser.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not open HTML file: {ex.Message}");
                }
            }
            
            Console.WriteLine();
            
            // Open pie chart option
            Console.WriteLine("Would you like to open the pie chart? (y/n): ");
            var openChart = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;
            
            if (openChart && File.Exists(chartFilePath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = chartFilePath,
                        UseShellExecute = true
                    });
                    Console.WriteLine("Pie chart opened in default image viewer.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not open chart file: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            dataService.Dispose();
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
