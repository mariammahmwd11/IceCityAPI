using Business_Layer.DTO;
using Business_Layer.Filters;
using Business_Layer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace Week1_Advanced_API_Migrations.Areas.Mayor.Controllers
{
    [Area("Mayor")]
    [Authorize(Roles = "Mayor")]
    public class MayorController : Controller
    {
        private readonly IDashboardService dashboardService;
        private readonly IReportService reportService;

        public MayorController(IDashboardService dashboardService, IReportService reportService)
        {
            this.dashboardService = dashboardService;
            this.reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var data = await dashboardService.GetOverviewAsync();
            return View("Dashboard", data);
        }

        [HttpGet]
        public async Task<IActionResult> Owners(string search = "", int page = 1)
        {
            var ownerFilter = new OwnerFilter
            {
                searchName = search,
                pagenumber = page,
                pageSize = 10
            };
            var data = await dashboardService.GetOwnersAsync(ownerFilter);
            ViewBag.CurrentPage = data.PageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)data.TotalItems / ownerFilter.pageSize);
            ViewBag.Search = search;
            ViewBag.PageSize = ownerFilter.pageSize;
            return View("Owners", data);
        }

        [HttpGet]
        public async Task<IActionResult> Owner(int id)
        {
            var data = await dashboardService.GetOwnerById(id);
            return View("OwnerDetails", data);
        }
        [HttpGet]

        [Route("Mayor/Mayor/HouseDetails/{ownerId}")]
        public async Task<IActionResult> HouseDetails(int ownerId, int? houseId = null)
        {
            var dateRange = new DateRange
            {
                DateFrom = DateTime.Now.AddDays(-30),
                DateTo = DateTime.Now
            };

            var data = await dashboardService.GetHouseDetailAsync(ownerId, dateRange);

            if (data == null || data.Count == 0)
            {
                ViewBag.OwnerId = ownerId;
                return View("HouseDetails", new List<HouseDetailVm>());
            }

            int activeId = houseId ?? data[0].HouseId;
            if (!data.Any(h => h.HouseId == activeId))
                activeId = data[0].HouseId;

            ViewBag.ActiveHouseId = activeId;
            ViewBag.OwnerId = ownerId;

         
            var heatersByHouse = data.ToDictionary(
                h => h.HouseId,
                h => h.heaters ?? new List<Heater>()
            );
            ViewBag.HeatersByHouse = heatersByHouse;

            return View("HouseDetails", data);
        }

        [HttpGet]
        public async Task<IActionResult> Reports(int houseId, int houseIndex = 0)
        {
            var options = new ReportOptions
            {
                dateFrom = DateTime.Now.AddDays(-30),
                dateTo = DateTime.Now,
                pdf = true,
                csv = true
            };

            var data = await reportService.GenerateHouseReportAsync(houseId, options);
            ViewBag.HouseIndex = houseIndex;
            return View("PrintReport", data);
        }
    }
}