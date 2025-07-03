using System.Drawing;

namespace EmployeeVisualizer.Services;

public class PieChartGenerator
{
    private readonly Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple, Color.Yellow, Color.Cyan, Color.Brown, Color.Gray, Color.Pink };

    public Bitmap CreatePieChart(List<EmployeeSummary> employees)
    {
        var bmp = new Bitmap(600, 400);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.White);

        double total = employees.Sum(e => e.TotalHours);
        float angle = 0;
        var rect = new Rectangle(30, 30, 200, 200);

        for (int i = 0; i < employees.Count; i++)
        {
            float sweep = (float)(employees[i].TotalHours / total * 360);
            using var brush = new SolidBrush(colors[i % colors.Length]);
            g.FillPie(brush, rect, angle, sweep);
            angle += sweep;
        }

        for (int i = 0; i < employees.Count; i++)
        {
            using var brush = new SolidBrush(colors[i % colors.Length]);
            g.FillRectangle(brush, 250, 40 + i * 25, 15, 15);
            g.DrawString(employees[i].EmployeeName, new Font("Arial", 9), Brushes.Black, 270, 40 + i * 25);
        }

        return bmp;
    }

    public void SaveChart(Bitmap chart, string filePath)
    {
        chart.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        Console.WriteLine($"Chart saved: {filePath}");
    }
}
