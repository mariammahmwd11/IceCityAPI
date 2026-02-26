using Business_Layer.DTO;
using Business_Layer.Helper;
using Data_Access.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace Week1_Advanced_API_Migrations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        //اشتغلت بيه هنا لان البروجكت صغير :)
        public SensorController(ApplicationDbContext applicationDbContext)
        {
            context = applicationDbContext;
        }

        [HttpPost("readings")]
        [Authorize(Roles = "Admin,Operator")]
        public IActionResult ReadSensorData(SensorReaderDTO readerDTO)
        {
            if(ModelState.IsValid)
            {
                var sensorreading = new SensorReading
                {
                    HeaterId = readerDTO.HeaterId,
                    ReadingDate = readerDTO.ReadingDate,
                    WorkingHours = readerDTO.WorkingHours,
                    HeaterValue = readerDTO.HeaterValue
                };
                context.sensorReadings.Add(sensorreading);
                context.SaveChanges();
                return Ok(sensorreading);
            }
            return BadRequest("There is a proplem");
        }
    }
}
