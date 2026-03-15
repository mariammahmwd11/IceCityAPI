using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.DTO
{
    public class ReportResult
    {
        public string OwnerName { get; set; }
        public string HouseAddress { get; set; }
    
        public DateTime GeneratedAt { get; set; }

        public IEnumerable<DailyUsageVm> DailyUsages { get; set; } 
        public int TotalWorkingHours { get; set; }
        public decimal MedianHeaterValue { get; set; }
        public decimal MonthlyAverageCost { get; set; }
        public List<Heater> Heaters { get; set; } = new List<Heater>();

        public string HtmlContent { get; set; }
        public byte[] CsvContent { get; set; }  
        public byte[] PdfStream { get; set; }  
    }

}
