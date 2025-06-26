using EmployeeVisualizer.Services;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace EmployeeVisualizer.Services;

/// <summary>
/// Generates PNG pie charts for employee time data
/// </summary>
public class PieChartGenerator
{
    // Color palette for pie slices
    private readonly Color[] _colors = {
        Color.FromArgb(255, 99, 132),   // Red
        Color.FromArgb(54, 162, 235),   // Blue
        Color.FromArgb(255, 205, 86),   // Yellow
        Color.FromArgb(75, 192, 192),   // Teal
        Color.FromArgb(153, 102, 255),  // Purple
        Color.FromArgb(255, 159, 64),   // Orange
        Color.FromArgb(199, 199, 199),  // Gray
        Color.FromArgb(83, 102, 255),   // Light Blue
        Color.FromArgb(255, 99, 255),   // Pink
        Color.FromArgb(99, 255, 132),   // Light Green
        Color.FromArgb(255, 159, 255),  // Light Pink
        Color.FromArgb(159, 255, 99),   // Lime
    };

    // Generates pie chart from employee data
    public Bitmap GenerateEmployeePieChart(List<EmployeeSummary> employeeSummaries, int width = 1000, int height = 700)
    {
        var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        
        // High quality graphics
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        
        graphics.Clear(Color.White);
        
        var totalHours = employeeSummaries.Sum(e => e.TotalHours);
        
        // No data case
        if (totalHours == 0)
        {
            DrawNoDataMessage(graphics, width, height);
            return bitmap;
        }
        
        // Chart positioning
        var chartSize = Math.Min(width - 400, height - 200);
        var chartRect = new Rectangle(50, 80, chartSize, chartSize);
        
        DrawTitle(graphics, "Employee Time Distribution", width);
        
        var currentAngle = 0f;
        var colorIndex = 0;
        
        // Draw pie slices
        foreach (var employee in employeeSummaries)
        {
            var percentage = (float)(employee.TotalHours / totalHours);
            var sweepAngle = percentage * 360f;
            var color = _colors[colorIndex % _colors.Length];
            
            // Draw slice
            using var brush = new SolidBrush(color);
            graphics.FillPie(brush, chartRect, currentAngle, sweepAngle);
            
            // Draw border
            using var pen = new Pen(Color.White, 2);
            graphics.DrawPie(pen, chartRect, currentAngle, sweepAngle);
            
            currentAngle += sweepAngle;
            colorIndex++;
        }
        
        // Draw legend
        DrawImprovedLegend(graphics, employeeSummaries, totalHours, chartRect.Right + 30, 120);
        
        return bitmap;
    }
    
    // Draws chart title
    private void DrawTitle(Graphics graphics, string title, int width)
    {
        using var titleFont = new Font("Arial", 20, FontStyle.Bold);
        using var titleBrush = new SolidBrush(Color.FromArgb(51, 51, 51));
        
        var titleSize = graphics.MeasureString(title, titleFont);
        var titleX = (width - titleSize.Width) / 2;
        
        graphics.DrawString(title, titleFont, titleBrush, titleX, 10);
    }
    
    // (Not used) Draws percentage labels on slices
    private void DrawSliceLabel(Graphics graphics, Rectangle chartRect, float startAngle, float sweepAngle, string label)
    {
        // Calculate center angle
        var midAngle = startAngle + sweepAngle / 2;
        var radians = midAngle * Math.PI / 180;
        
        // Position label at slice center
        var labelRadius = Math.Min(chartRect.Width, chartRect.Height) / 3.2f;
        var labelX = chartRect.X + chartRect.Width / 2 + (float)(Math.Cos(radians) * labelRadius);
        var labelY = chartRect.Y + chartRect.Height / 2 + (float)(Math.Sin(radians) * labelRadius);
        
        using var labelFont = new Font("Arial", 11, FontStyle.Bold);
        using var labelBrush = new SolidBrush(Color.White);
        using var shadowBrush = new SolidBrush(Color.Black);
        
        // Center text
        var labelSize = graphics.MeasureString(label, labelFont);
        labelX -= labelSize.Width / 2;
        labelY -= labelSize.Height / 2;
        
        // Draw shadow then text
        graphics.DrawString(label, labelFont, shadowBrush, labelX + 1, labelY + 1);
        graphics.DrawString(label, labelFont, labelBrush, labelX, labelY);
    }
    
    // Draws legend with employee details
    private void DrawImprovedLegend(Graphics graphics, List<EmployeeSummary> employeeSummaries, double totalHours, int startX, int startY)
    {
        using var legendFont = new Font("Arial", 11);
        using var legendBrush = new SolidBrush(Color.Black);
        using var headerFont = new Font("Arial", 12, FontStyle.Bold);
        
        var itemHeight = 28;
        var colorSquareSize = 16;
        
        graphics.DrawString("Employee Time Breakdown", headerFont, legendBrush, startX, startY - 25);
        
        // Draw legend entries
        for (int i = 0; i < employeeSummaries.Count; i++)
        {
            var employee = employeeSummaries[i];
            var color = _colors[i % _colors.Length];
            var percentage = employee.TotalHours / totalHours;
            var y = startY + i * itemHeight;
            
            // Color square
            using var colorBrush = new SolidBrush(color);
            graphics.FillRectangle(colorBrush, startX, y + 3, colorSquareSize, colorSquareSize);
            graphics.DrawRectangle(Pens.Black, startX, y + 3, colorSquareSize, colorSquareSize);
            
            // Employee text
            var legendText = $"{employee.EmployeeName}";
            var timeText = $"{employee.TotalHours:F1}h ({percentage:P1})";
            
            graphics.DrawString(legendText, legendFont, legendBrush, startX + colorSquareSize + 8, y);
            graphics.DrawString(timeText, legendFont, new SolidBrush(Color.Gray), startX + colorSquareSize + 8, y + 14);
        }
    }

    // Draws "No data" message
    private void DrawNoDataMessage(Graphics graphics, int width, int height)
    {
        using var font = new Font("Arial", 16);
        using var brush = new SolidBrush(Color.Gray);
        
        var message = "No data available";
        var messageSize = graphics.MeasureString(message, font);
        var x = (width - messageSize.Width) / 2;
        var y = (height - messageSize.Height) / 2;
        
        graphics.DrawString(message, font, brush, x, y);
    }
    
    // Saves pie chart to PNG file
    public void SaveChartToFile(Bitmap chart, string filePath)
    {
        try
        {
            chart.Save(filePath, ImageFormat.Png);
            Console.WriteLine($"Pie chart saved to: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving pie chart: {ex.Message}");
        }
    }
}
