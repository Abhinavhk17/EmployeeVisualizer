using System.Text;

namespace EmployeeVisualizer.Services;

public class HtmlTableGenerator
{
    public string GenerateHtml(List<EmployeeSummary> employees)
    {
        var html = new StringBuilder();
        
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head><title>Employee Time Report</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial; margin: 20px; }");
        html.AppendLine("table { border-collapse: collapse; width: 100%; }");
        html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
        html.AppendLine("th { background-color: #4CAF50; color: white; }");
        html.AppendLine(".low-hours { background-color: #ffcccc; }");
        html.AppendLine("</style>");
        html.AppendLine("</head><body>");
        
        html.AppendLine("<h1>Employee Time Summary</h1>");
        html.AppendLine("<table>");
        html.AppendLine("<tr><th>Employee Name</th><th>Total Hours</th></tr>");
        
        foreach (var emp in employees)
        {
            var rowClass = emp.TotalHours < 100 ? " class='low-hours'" : "";
            html.AppendLine($"<tr{rowClass}>");
            html.AppendLine($"<td>{emp.EmployeeName}</td>");
            html.AppendLine($"<td>{emp.TotalHours:F2}</td>");
            html.AppendLine("</tr>");
        }
        
        html.AppendLine("</table>");
        html.AppendLine($"<p>Generated: {DateTime.Now}</p>");
        html.AppendLine("<p>Red rows: Employees with less than 100 hours</p>");
        html.AppendLine("</body></html>");
        
        return html.ToString();
    }
    
    public void SaveToFile(string html, string filePath)
    {
        File.WriteAllText(filePath, html);
        Console.WriteLine($"HTML saved: {filePath}");
    }
}
