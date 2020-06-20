using AtomStore.Application.ViewModels.System;
using AtomStore.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtomStore.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddAsync(AppUserViewModel userVm);

        Task DeleteAsync(string id);

        Task<List<AppUserViewModel>> GetAllAsync();
        int GetUserCount();
        PagedResult<AppUserViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<AppUserViewModel> GetById(string id);


        Task UpdateAsync(AppUserViewModel userVm);

        Task UpdateUserAsync(AppUserViewModel userVm);
        Task UpdateAsyncClient(AppUserViewModel userVm);
    }
}
