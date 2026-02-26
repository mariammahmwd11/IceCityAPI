using Business_Layer.IServices;
using Data_Access.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Models.Models;

namespace Week1_Advanced_API_Migrations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseController : ControllerBase
    {
        private readonly IHouseService houseService;
        private readonly ApplicationDbContext context;

        
        public HouseController(IHouseService houseService,ApplicationDbContext context)
        {
            this.houseService = houseService;
            this.context = context;
        }


        [HttpGet("{houseId:int}/cost")]
        public IActionResult CalculateCost(int houseId)
        {
            var house = context.houses.FirstOrDefault(b=>b.HouseId==houseId);
            if(house==null)
            {
                return BadRequest("Invalid Id");
            }
            var result=  houseService.calculateMonthlyCost(houseId);
            return Ok(new { HouseId =houseId , MonthlyAverageCost = result });


        }
        [HttpGet("{houseId}/monthly-report")]
        [Authorize(Roles = "Admin,Operator,User")]
        public IActionResult MonthlyReport(int houseId)
        {
            var house = context.houses.FirstOrDefault(b => b.HouseId == houseId);
            if (house == null)
            {
                return BadRequest("Invalid Id");
            }
            var Report = context.monthlyCostReports.Where(b => b.HouseId == houseId && b.CreatedAt >= DateTime.Now.AddDays(-30))
            .OrderByDescending(b => b.CreatedAt).FirstOrDefault();
            if (Report == null)
            {
                var newmonthlycost = houseService.calculateMonthlyCost(houseId);
                var readings = houseService.Readings(houseId);
                Report = new MonthlyCostReport
                {
                    HouseId = houseId,
                    ReportMonth = DateTime.Now.ToString("yyyy-MM"),
                    TotalWorkingHours = houseService.calcultetotalWorkingHours(readings),
                    MedianHeaterValue = houseService.calcultemedianHeatersValue(readings),
                    MonthlyAverageCost = newmonthlycost,
                    CreatedAt = DateTime.Now
                };
                context.monthlyCostReports.Add(Report);
                context.SaveChanges();

            }
            return Ok(Report);
        }
    }
    }

