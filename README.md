# Employee Visualizer using C#

A C# console application that fetches employee time entry data from a REST API and generates both HTML table reports and PNG pie chart visualizations.

## 🎯 Project Overview

The Employee Visualizer automates the process of:
1. **Fetching** employee time data from an external REST API
2. **Processing** and aggregating working hours by employee
3. **Generating** professional HTML reports with conditional formatting
4. **Creating** visual pie charts showing time distribution percentages
5. **Providing** interactive file opening for immediate viewing

## ✨ Key Features

### 📊 **Data Processing**
- Fetches data from REST API
- Filters invalid entries
- Aggregates hours per employee
- Sorts by total hours

### 📋 **HTML Report Generation**
- Professional responsive tables
- Conditional formatting for low hours
- Embedded CSS styling
- Modern Bootstrap-like appearance

### 📈 **Pie Chart Visualization**
- Colorful percentage distribution charts
- Clean design with comprehensive legend
- High-quality PNG export


##  Sample Output

### **Console Output**
```
Employee Visualizer
==================

Fetching employee time data from API...
Successfully fetched 766 time entries.
Processed data for 10 employees.

Top 10 Employees by Total Hours:
================================
Patrick Huthinson: 212.13 hours
Abhay Singh: 207.65 hours
Stewart Malachi: 197.83 hours
John Black: 197.02 hours
Mary Poppins: 167.92 hours
Tim Perkinson: 163.17 hours
Kavvay Verma: 158.12 hours
Rita Alley: 112.18 hours
Raju Sunuwar: 95.67 hours (< 100 hours)
Tamoy Smith: 86.78 hours (< 100 hours)

Generating HTML table...
HTML table saved to: employee_time_report.html

Generating pie chart...
Pie chart saved to: employee_time_piechart.png

Output Files:
- HTML Report: employee_time_report.html
- Pie Chart: employee_time_piechart.png
```

### **Generated Files**
- **`employee_time_report.html`** - Professional HTML table report
- **`employee_time_piechart.png`** - Visual pie chart (1000x700 pixels)

---

## 📸 Sample Outputs

### **Pie Chart Visualization**
![Employee Time Distribution Pie Chart](screenshots/piechart.png)

### **HTML Report Screenshot**
![HTML Report Table](screenshots/html.png)


