using AtomStore.Application.ViewModels.Product;
using AtomStore.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface IWishlistService
    {
        List<WishlistViewModel> GetAll(Guid userId);
        PagedResult<WishlistViewModel> GetAllPaging(Guid userId, int page, int pageSize);
        void Create(WishlistViewModel wishlistVm);
        void Delete(int id);
        void Save();
    }
}
