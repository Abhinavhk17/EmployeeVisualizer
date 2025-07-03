using EmployeeVisualizer.Services;
using System.Diagnostics;

namespace EmployeeVisualizer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Employee Time Visualizer");
        Console.WriteLine("========================\n");

        try
        {
            // Fetch employee data
            var dataService = new EmployeeDataService();
            var employees = await dataService.GetEmployeeDataAsync();

            if (employees.Count == 0)
            {
                Console.WriteLine("No employee data found!");
                return;
            }

            Console.WriteLine($"Found {employees.Count} employees:");
            foreach (var emp in employees.Take(10))
            {
                string lowHours = emp.TotalHours < 100 ? " (LOW)" : "";
                Console.WriteLine($"- {emp.EmployeeName}: {emp.TotalHours:F2} hours{lowHours}");
            }
            Console.WriteLine();

            // Generate HTML report
            Console.WriteLine("Creating HTML report...");
            var htmlGenerator = new HtmlTableGenerator();
            string html = htmlGenerator.GenerateHtml(employees);
            string htmlFile = "employee_report.html";
            htmlGenerator.SaveToFile(html, htmlFile);

            // Generate pie chart
            Console.WriteLine("Creating pie chart...");
            var chartGenerator = new PieChartGenerator();
            using var chart = chartGenerator.CreatePieChart(employees);
            string chartFile = "employee_chart.png";
            chartGenerator.SaveChart(chart, chartFile);

            Console.WriteLine("\nFiles created:");
            Console.WriteLine($"- HTML Report: {htmlFile}");
            Console.WriteLine($"- Pie Chart: {chartFile}");

            // Ask to open files
            Console.Write("Open HTML report? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() == "y")
                Process.Start(new ProcessStartInfo(htmlFile) { UseShellExecute = true });

            Console.Write("Open pie chart? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() == "y")
                Process.Start(new ProcessStartInfo(chartFile) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
