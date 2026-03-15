using Business_Layer.DTO;
using Business_Layer.IServices;
using Data_Access.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
namespace Business_Layer.Services
{
   public class ReportService:IReportService
    {
        private readonly ApplicationDbContext dbContext;

        public ReportService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ReportResult> GenerateHouseReportAsync(int houseId, ReportOptions options)
        {
            var house =await  dbContext.houses.Include(b => b.owner)
                .Include(b=>b.DailyUsages)
                  .FirstOrDefaultAsync(b => b.HouseId == houseId);

            var from = options.dateFrom ?? DateTime.Today.AddDays(-30);
            var to = options.dateTo ?? DateTime.Today;

            var dailyusage = house.DailyUsages
                 .Where(u => u.UsageDate.Date >= from.Date && u.UsageDate.Date <= to.Date)
                .Select(b => new DailyUsageVm
            {
                Date = b.UsageDate,
                HeaterValue = b.HeaterValue,
                HoursWorked = b.HoursWorked
            }).ToList();

            var medianValue = dailyusage.Count() > 0
            ? dailyusage.OrderBy(u => u.HeaterValue).ElementAt(dailyusage.Count() / 2).HeaterValue
            : 0;
            var TotalWorkingHours = dailyusage.Sum(b => b.HoursWorked);
            var avgCost = medianValue * (TotalWorkingHours / (24m * 30m));//المعادلة الل ف الاول


            //pdf
            QuestPDF.Settings.License = LicenseType.Community;
            byte[] pdfStream = null;

            if (options.pdf)
            {
                var pdfDoc = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(20);
                        page.Content().Column(col =>
                        {
                            col.Item().Text($"House Report - {house.Address}").FontSize(20).Bold();
                            col.Item().Text($"Owner: {house.owner.FullName}");
                            col.Item().Text($"Report Period: {from:yyyy-MM-dd} to {to:yyyy-MM-dd}");
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });
                                table.Header(header =>
                                {
                                    header.Cell().Text("Date");
                                    header.Cell().Text("Heater Value");
                                    header.Cell().Text("Hours Worked");
                                });
                                foreach (var d in dailyusage)
                                {
                                    table.Cell().Text(d.Date.ToString("yyyy-MM-dd"));
                                    table.Cell().Text(d.HeaterValue.ToString());
                                    table.Cell().Text(d.HoursWorked.ToString());
                                }
                            });
                            col.Item().Text($"Total Hours: {TotalWorkingHours}");
                            col.Item().Text($"Median Heater Value: {medianValue}");
                            col.Item().Text($"Monthly Average Cost: {avgCost:C}");
                        });
                    });
                });

                pdfStream = pdfDoc.GeneratePdf();
            }
            //csv
            byte[] csvBytes = null;
            if (options.csv) {

                 var csvSb = new StringBuilder(); 
                csvSb.AppendLine("Date,HeaterValue,HoursWorked");
                foreach (var d in dailyusage)
                {
                    csvSb.AppendLine($"{d.Date:yyyy-MM-dd},{d.HeaterValue},{d.HoursWorked}");
                }
                 csvBytes = Encoding.UTF8.GetBytes(csvSb.ToString());
            }

            //html
            var sb = new StringBuilder();
            sb.AppendLine($"<h2>House Report - {house.Address}</h2>");
            sb.AppendLine($"<p>Owner: {house.owner.FullName}</p>");
            sb.AppendLine($"<p>Report Period: {from:yyyy-MM-dd} to {to:yyyy-MM-dd}</p>");
            sb.AppendLine("<table border='1'><tr><th>Date</th><th>Heater Value</th><th>Hours Worked</th></tr>");
            foreach (var d in dailyusage)
            {
                sb.AppendLine($"<tr><td>{d.Date:yyyy-MM-dd}</td><td>{d.HeaterValue}</td><td>{d.HoursWorked}</td></tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine($"<p>Total Hours: {TotalWorkingHours}</p>");
            sb.AppendLine($"<p>Median Heater Value: {medianValue}</p>");
            sb.AppendLine($"<p>Monthly Average Cost: {avgCost:C}</p>");
            string htmlContent = sb.ToString();
            var heaters = await dbContext.heaters
    .Where(h => h.HouseId == houseId)
    .ToListAsync();

            var result = new ReportResult
            {
                HouseAddress = house.Address,
                OwnerName = house.owner.FullName,
                GeneratedAt = DateTime.UtcNow,
                DailyUsages = dailyusage,
                TotalWorkingHours = TotalWorkingHours,
                MonthlyAverageCost=avgCost,
                MedianHeaterValue=medianValue,
                Heaters = heaters,
                PdfStream = pdfStream,
                CsvContent =csvBytes,
                HtmlContent=htmlContent

            };

            return result;

        }
    }
}
