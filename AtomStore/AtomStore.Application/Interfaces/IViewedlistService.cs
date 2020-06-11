using AtomStore.Application.ViewModels.Product;
using AtomStore.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface IViewedlistService
    {
        ViewedlistViewModel GetByProductAndUserId(int productId, Guid userId);
        List<ViewedlistViewModel> GetAll(Guid userId);
        PagedResult<ViewedlistViewModel> GetAllPaging(Guid userId, int page, int pageSize);
        void Create(ViewedlistViewModel viewlistVm);
        void Delete(int id);
        void Save();
    }
}
