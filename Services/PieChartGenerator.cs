using System.Drawing;

namespace EmployeeVisualizer.Services;

public class PieChartGenerator
{
    private readonly Color[] colors = {
        Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple,
        Color.Yellow, Color.Pink, Color.Cyan, Color.Brown, Color.Gray
    };
    
    public Bitmap CreatePieChart(List<EmployeeSummary> employees)
    {
        var bitmap = new Bitmap(800, 600);
        using var g = Graphics.FromImage(bitmap);
        
        g.Clear(Color.White);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        
        // Chart area
        var chartRect = new Rectangle(50, 50, 300, 300);
        var totalHours = employees.Sum(e => e.TotalHours);
        
        if (totalHours == 0) return bitmap;
        
        // Draw title
        g.DrawString("Employee Time Distribution", new Font("Arial", 16, FontStyle.Bold), 
                    Brushes.Black, 200, 10);
        
        // Draw pie slices
        float startAngle = 0;
        for (int i = 0; i < employees.Count; i++)
        {
            var emp = employees[i];
            var percentage = (float)(emp.TotalHours / totalHours);
            var sweepAngle = percentage * 360;
            var color = colors[i % colors.Length];
            
            g.FillPie(new SolidBrush(color), chartRect, startAngle, sweepAngle);
            g.DrawPie(Pens.Black, chartRect, startAngle, sweepAngle);
            
            startAngle += sweepAngle;
        }
        
        // Draw legend
        int legendY = 80;
        for (int i = 0; i < employees.Count; i++)
        {
            var emp = employees[i];
            var color = colors[i % colors.Length];
            var percentage = emp.TotalHours / totalHours;
            
            // Color box
            g.FillRectangle(new SolidBrush(color), 400, legendY, 15, 15);
            g.DrawRectangle(Pens.Black, 400, legendY, 15, 15);
            
            // Text
            var text = $"{emp.EmployeeName}: {emp.TotalHours:F1}h ({percentage:P1})";
            g.DrawString(text, new Font("Arial", 10), Brushes.Black, 420, legendY);
            
            legendY += 25;
        }
        
        return bitmap;
    }
    
    public void SaveChart(Bitmap chart, string filePath)
    {
        chart.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        Console.WriteLine($"Chart saved: {filePath}");
    }
}
