using EmployeeVisualizer.Services;
using System.Text;

namespace EmployeeVisualizer.Services;

// Generates HTML table reports from employee data
public class HtmlTableGenerator
{
    // Configuration properties
    public string ReportTitle { get; set; } = "Employee Time Summary";
    public string PageTitle { get; set; } = "Employee Time Tracker";
    public double LowHoursThreshold { get; set; } = 100.0;
    public string PrimaryColor { get; set; } = "#007acc";
    public string LowHoursColor { get; set; } = "#ffebee";
    public string LowHoursTextColor { get; set; } = "#c62828";

    // Generates HTML page with employee data table
    public string GenerateEmployeeTable(List<EmployeeSummary> employeeSummaries)
    {
        var html = new StringBuilder();
        
        html.AppendLine(GenerateHtmlHeader());
        html.AppendLine(GenerateStyles());
        html.AppendLine(GenerateHtmlBodyStart());
        html.AppendLine(GenerateTableContent(employeeSummaries));
        html.AppendLine(GenerateFooter());
        html.AppendLine(GenerateHtmlBodyEnd());
        
        return html.ToString();
    }

    private string GenerateHtmlHeader()
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{PageTitle}</title>";
    }

    private string GenerateStyles()
    {
        return $@"    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 40px;
            background-color: #f5f5f5;
        }}
        .container {{
            max-width: 800px;
            margin: 0 auto;
            background-color: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        h1 {{
            color: #333;
            text-align: center;
            margin-bottom: 30px;
            border-bottom: 3px solid {PrimaryColor};
            padding-bottom: 10px;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        th, td {{
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }}
        th {{
            background-color: {PrimaryColor};
            color: white;
            font-weight: bold;
        }}
        tr:nth-child(even) {{
            background-color: #f9f9f9;
        }}
        tr.low-hours {{
            background-color: {LowHoursColor} !important;
            color: {LowHoursTextColor} !important;
            font-weight: bold;
        }}
        tr.low-hours td {{
            background-color: {LowHoursColor} !important;
            color: {LowHoursTextColor} !important;
        }}
        .hours-cell {{
            font-weight: bold;
        }}
        .footer {{
            margin-top: 30px;
            text-align: center;
            color: #666;
            font-size: 14px;
        }}
    </style>
</head>";
    }

    private string GenerateHtmlBodyStart()
    {
        return $@"<body>
    <div class=""container"">
        <h1>{ReportTitle}</h1>";
    }

    private string GenerateTableContent(List<EmployeeSummary> employeeSummaries)
    {
        var tableHtml = new StringBuilder();
        
        tableHtml.AppendLine(@"        <table>
            <thead>
                <tr>
                    <th>Employee Name</th>
                    <th>Total Hours Worked</th>
                </tr>
            </thead>
            <tbody>");
        
        foreach (var employee in employeeSummaries)
        {
            var rowClass = employee.TotalHours < LowHoursThreshold ? "low-hours" : "";
            tableHtml.AppendLine($@"                <tr class=""{rowClass}"">
                    <td>{employee.EmployeeName}</td>
                    <td class=""hours-cell"">{employee.TotalHours:F2}</td>
                </tr>");
        }
        
        tableHtml.AppendLine(@"            </tbody>
        </table>");
        
        return tableHtml.ToString();
    }

    private string GenerateFooter()
    {
        return $@"        <div class=""footer"">
            <p>Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
            <p>Employees with less than {LowHoursThreshold} hours are highlighted in <span style=""color: {LowHoursTextColor}; font-weight: bold;"">red</span></p>
        </div>";
    }

    private string GenerateHtmlBodyEnd()
    {
        return @"    </div>
</body>
</html>";
    }

    // Saves HTML content to file
    public async Task SaveHtmlToFileAsync(string htmlContent, string filePath)
    {
        try
        {
            await File.WriteAllTextAsync(filePath, htmlContent);
            Console.WriteLine($"HTML table saved to: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving HTML file: {ex.Message}");
        }
    }
}
