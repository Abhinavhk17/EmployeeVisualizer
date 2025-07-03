namespace EmployeeVisualizer.Services;

public class HtmlTableGenerator
{
    public string GenerateHtml(List<EmployeeSummary> employees)
    {
        // Build table rows with conditional class for low hours
        var tableRows = string.Join("", employees.Select(emp =>
            $"<tr{(emp.TotalHours < 100 ? " class='low-hours'" : "")}>" +
            $"<td>{emp.EmployeeName}</td>" +
            $"<td>{emp.TotalHours:F2}</td>" +
            "</tr>"
        ));

        // Return formatted HTML
        return $@"<!DOCTYPE html>
<html>
<head>
    <title>Employee Time Report</title>
    <style>
        body {{ font-family: Arial; margin: 20px; }}
        table {{ border-collapse: collapse; width: 100%; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        th {{ background-color: #4CAF50; color: white; }}
        .low-hours {{ background-color: #ffcccc; }}
    </style>
</head>
<body>
    <h1>Employee Time Summary</h1>
    <table>
        <tr><th>Employee Name</th><th>Total Hours</th></tr>
        {tableRows}
    </table>
    <p>Generated: {DateTime.Now}</p>
    <p>Red rows: Employees with less than 100 hours</p>
</body>
</html>";
    }

    public void SaveToFile(string html, string filePath)
    {
        File.WriteAllText(filePath, html);
        Console.WriteLine($"HTML saved: {filePath}");
    }
}
