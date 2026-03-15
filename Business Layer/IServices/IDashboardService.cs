using Business_Layer.DTO;
using Business_Layer.Filters;
using Models.Helper;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Business_Layer.IServices
{
  public  interface IDashboardService
    {
        Task<DashboardOverviewVm> GetOverviewAsync();
        Task<PagedResult<OwnerVm>> GetOwnersAsync(OwnerFilter filter);
        Task<List<HouseDetailVm>>GetHouseDetailAsync(int ownerid, DateRange range);
        Task<OwnerVm> GetOwnerById(int id);
    }
}
