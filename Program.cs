using EmployeeVisualizer.Services;
using System.Diagnostics;

namespace EmployeeVisualizer;

/// <summary>
/// Employee Visualizer - A C# console application that fetches employee time entry data 
/// from a REST API and generates both HTML table and PNG pie chart visualizations.
/// 
/// Main functionalities:
/// 1. Fetches employee time data from REST API endpoint
/// 2. Processes and summarizes total working hours per employee
/// 3. Generates an HTML report with employee table (highlights <100 hours)
/// 4. Creates a PNG pie chart showing time distribution percentages
/// 5. Provides interactive file opening functionality
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        // Display application header
        Console.WriteLine("Employee Visualizer");
        Console.WriteLine("==================");
        Console.WriteLine();
        
        // Initialize service classes that handle different aspects of the application
        var dataService = new EmployeeDataService();        // Handles API calls and data processing
        var htmlGenerator = new HtmlTableGenerator()         // Generates HTML table reports
        {
            // Configure the HTML generator (no longer hardcoded!)
            ReportTitle = "Employee Time Analysis Report",
            PageTitle = "Company Time Tracker Dashboard", 
            LowHoursThreshold = 100.0,    // Back to 100 for clear visibility
            PrimaryColor = "#2e7d32",     // Green instead of blue
            LowHoursColor = "#ffcdd2",    // Light red background for better visibility
            LowHoursTextColor = "#d32f2f" // Dark red text for better contrast
        };
        var chartGenerator = new PieChartGenerator();        // Creates PNG pie charts
        
        try
        {
            // STEP 1: Fetch employee time data from REST API
            // This will call the provided API endpoint to retrieve all time entries
            Console.WriteLine("Fetching employee time data from API...");
            
            // Make HTTP GET request to the API and deserialize JSON response into TimeEntry objects
            var timeEntries = await dataService.GetTimeEntriesAsync();
            
            // Validate that we received data from the API
            if (!timeEntries.Any())
            {
                Console.WriteLine("No time entries found or failed to fetch data.");
                return;
            }

            Console.WriteLine($"Successfully fetched {timeEntries.Count} time entries.");
            
            // STEP 2: Process and summarize the raw time entry data
            // This will group entries by employee and calculate total hours worked per person
            var employeeSummaries = dataService.CalculateEmployeeSummaries(timeEntries);
            
            Console.WriteLine($"Processed data for {employeeSummaries.Count} employees.");
            Console.WriteLine();
            
            // STEP 3: Display summary information in console
            // Shows top 10 employees by total hours worked for user reference
            Console.WriteLine("Top 10 Employees by Total Hours:");
            Console.WriteLine("================================");
            foreach (var employee in employeeSummaries.Take(10))
            {
                // Highlight employees who worked less than the configured threshold
                var status = employee.TotalHours < htmlGenerator.LowHoursThreshold ? $" (< {htmlGenerator.LowHoursThreshold} hours)" : "";
                Console.WriteLine($"{employee.EmployeeName}: {employee.TotalHours:F2} hours{status}");
            }
            Console.WriteLine();
            
            // STEP 4: Generate HTML table report
            // Creates a styled HTML page with a table showing employee names and total hours
            // Automatically highlights table rows for employees with less than 100 hours
            Console.WriteLine("Generating HTML table...");
            var htmlContent = htmlGenerator.GenerateEmployeeTable(employeeSummaries);
            var htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "employee_time_report.html");
            await htmlGenerator.SaveHtmlToFileAsync(htmlContent, htmlFilePath);
            
            // STEP 5: Generate PNG pie chart visualization
            // Creates a colorful pie chart showing the percentage distribution of total time
            // worked by each employee, with a detailed legend showing names and hours
            Console.WriteLine("Generating pie chart...");
            using var pieChart = chartGenerator.GenerateEmployeePieChart(employeeSummaries);
            var chartFilePath = Path.Combine(Directory.GetCurrentDirectory(), "employee_time_piechart.png");
            chartGenerator.SaveChartToFile(pieChart, chartFilePath);
            
            // Display output file locations to user
            Console.WriteLine();
            Console.WriteLine("Output Files:");
            Console.WriteLine($"- HTML Report: {htmlFilePath}");
            Console.WriteLine($"- Pie Chart: {chartFilePath}");
            Console.WriteLine();
            
            // STEP 6: Interactive file opening functionality
            // Provides user-friendly options to automatically open generated files
            
            // Option 1: Open HTML report in default web browser
            Console.WriteLine("Would you like to open the HTML report? (y/n): ");
            var openHtml = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;
            
            if (openHtml && File.Exists(htmlFilePath))
            {
                try
                {
                    // Use system default application to open HTML file (usually web browser)
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = htmlFilePath,
                        UseShellExecute = true  // Required for opening files with default application
                    });
                    Console.WriteLine("HTML report opened in default browser.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not open HTML file: {ex.Message}");
                }
            }
            
            Console.WriteLine();
            
            // Option 2: Open pie chart image in default image viewer
            Console.WriteLine("Would you like to open the pie chart? (y/n): ");
            var openChart = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;
            
            if (openChart && File.Exists(chartFilePath))
            {
                try
                {
                    // Use system default application to open PNG file (usually image viewer)
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = chartFilePath,
                        UseShellExecute = true  // Required for opening files with default application
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
            // Global error handling - catches any unexpected errors during execution
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            // Cleanup: Dispose of HTTP client and other resources
            // This ensures proper cleanup even if an exception occurs
            dataService.Dispose();
        }
        
        // Wait for user input before closing the console application
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
