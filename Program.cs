using EmployeeVisualizer.Services;
using System.Diagnostics;
namespace EmployeeVisualizer;
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Employee Time Visualizer\n========================\n");
        try
        {
            var employees = await new EmployeeDataService().GetEmployeeDataAsync();
            if (employees.Count == 0) { Console.WriteLine("No employee data found!"); return; }
            Console.WriteLine($"Found {employees.Count} employees:");
            employees.Take(10).ToList().ForEach(e => Console.WriteLine($"- {e.EmployeeName}: {e.TotalHours:F2} hours{(e.TotalHours < 100 ? " (LOW)" : "")}"));
            Console.WriteLine("\nCreating HTML report...");
            var html = new HtmlTableGenerator();
            html.SaveToFile(html.GenerateHtml(employees), "employee_report.html");
            Console.WriteLine("Creating pie chart...");
            var chart = new PieChartGenerator();
            using var c = chart.CreatePieChart(employees);
            chart.SaveChart(c, "employee_chart.png");
            Console.WriteLine("\nFiles created:\n- HTML Report: employee_report.html\n- Pie Chart: employee_chart.png\n");
            Console.Write("Open HTML report? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y") Process.Start(new ProcessStartInfo("employee_report.html") { UseShellExecute = true });
            Console.Write("Open pie chart? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y") Process.Start(new ProcessStartInfo("employee_chart.png") { UseShellExecute = true });
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
