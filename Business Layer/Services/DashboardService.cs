using Business_Layer.DTO;
using Business_Layer.Filters;
using Business_Layer.IServices;
using Data_Access.Data;
using Microsoft.EntityFrameworkCore;
using Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models.Models;

namespace Business_Layer.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext dbContext;

        public DashboardService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<PagedResult<OwnerVm>> GetOwnersAsync(OwnerFilter filter)
        {
            if(filter.pagenumber<=0)
            {
                filter.pagenumber = 1;
            }
            if(filter.pageSize<=0)
            {
                filter.pageSize = 10;
            }
            var query = dbContext.owners.AsQueryable();
            if(!string.IsNullOrEmpty(filter.searchName))
            {
                query = query.Where(o => o.FullName.Contains(filter.searchName));
            }
            var count = await query.CountAsync();
            var owners =await query.Skip((filter.pagenumber - 1) * filter.pageSize).Take(filter.pageSize)
                .Select(o => new OwnerVm
                {
                    ownerid=o.OwnerId,
                    FullName = o.FullName,
                    PhoneNumber = o.PhoneNumber,
                    houseCount = o.houses.Count()
                }).ToListAsync();

            var result = new PagedResult<OwnerVm>
            {
                Data = owners,
                PageNumber = filter.pagenumber,
                PageSize = filter.pageSize,
                TotalItems = count
            };
            return result;

        }

        public async Task<List<HouseDetailVm>> GetHouseDetailAsync(int ownerId, DateRange range)
        {
            var houses = await dbContext.houses
                .Include(h => h.owner)
                .Include(h => h.DailyUsages)
                .Where(h => h.OwnerId == ownerId)
                .Select(h => new HouseDetailVm
                {
                    HouseId = h.HouseId,
                    OwnerName = h.owner.FullName,
                    Address = h.Address,
                    CityZone = h.CityZone,

                    todayUsage = h.DailyUsages
                        .Where(d => d.UsageDate.Date == DateTime.Today)
                        .Sum(d => d.HeaterValue),

                    DailyUsages = h.DailyUsages
                        .Where(d =>
                            (range.DateFrom == null || d.UsageDate >= range.DateFrom) &&
                            (range.DateTo == null || d.UsageDate <= range.DateTo))
                        .ToList()
                })
                .ToListAsync();

           
            var houseIds = houses.Select(h => h.HouseId).ToList();

            var heatersByHouse = await dbContext.heaters
                .Where(h => houseIds.Contains(h.HouseId))
                .ToListAsync();

            foreach (var house in houses)
            {
                house.heaters = heatersByHouse
                    .Where(h => h.HouseId == house.HouseId)
                    .ToList();
            }

            return houses;
        }

        async Task<DashboardOverviewVm> IDashboardService.GetOverviewAsync()
        {
            var totalowners = await dbContext.owners.CountAsync();
            var totalHouses = await dbContext.houses.CountAsync();
            var totalconsumpution =await dbContext.DailyUsages.SumAsync(b => b.HeaterValue);
            var todayconsumption = await dbContext.DailyUsages.Where(b => b.UsageDate.Date == DateTime.Today).SumAsync(b => b.HeaterValue);
            var top5 =await dbContext.houses
                .Include(b=>b.owner)
                .Include(b=>b.DailyUsages)
                .Select(b => new HouseDetailVm
            {

                Address = b.Address,
                CityZone = b.CityZone,
                OwnerName = b.owner.FullName,
                todayUsage = b.DailyUsages.Where(b => b.UsageDate.Date == DateTime.Today).Sum(m => m.HeaterValue)



            }).OrderByDescending(b=>b.todayUsage)
            .Take(5)
            .ToListAsync();
            var result = new DashboardOverviewVm
            {
                TotalOwners = totalowners,
                TotalHouses = totalHouses,
                TotalConsumption = totalconsumpution,
                TodayConsumption = todayconsumption,
                top5Houses = top5
            };

            return result;
          
        }

        public async Task<OwnerVm> GetOwnerById(int id )
        {
            var data = await dbContext.owners
          .Include(o => o.houses)
          .FirstOrDefaultAsync(o => o.OwnerId == id);

            if (data == null)
                return null;
            var result = new OwnerVm
            {
                FullName = data.FullName,
                ownerid = data.OwnerId,
                houseCount = data.houses.Count(),
                houses = data.houses.ToList(),
                PhoneNumber = data.PhoneNumber
            };
            return result;
        }

    }
}
