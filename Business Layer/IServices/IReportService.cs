using Business_Layer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.IServices
{
   public interface IReportService
    {
        Task<ReportResult> GenerateHouseReportAsync(int houseId, ReportOptions options);
    }
}
