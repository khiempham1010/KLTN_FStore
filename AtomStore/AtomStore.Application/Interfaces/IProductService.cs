using AtomStore.Application.ViewModels.Common;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface IProductService : IDisposable
    {
        List<ProductViewModel> GetAll();

        PagedResult<ProductViewModel> GetAllPaging(int? categoryId, int? minPrice, int? maxPrice, string keyword, int page, int pageSize);

        PagedResult<ProductViewModel> GetAllPagingAdmin(int? categoryId, string keyword, int page, int pageSize);

        ProductViewModel Add(ProductViewModel product);

        void Update(ProductViewModel product);

        void Delete(int id);

        ProductViewModel GetById(int id);

        void ImportExcel(string filePath, int categoryId);


        void Save();

        void AddQuantity(int productId, List<ProductQuantityViewModel> quantities);

        List<ProductQuantityViewModel> GetQuantities(int productId);

        void AddImages(int productId, string[] images);

        List<ProductImageViewModel> GetImages(int productId);
        List<ProductViewModel> GetLastest(int top);

        List<ProductViewModel> GetHotProduct(int top);
        List<ProductViewModel> GetRelatedProducts(int id, int top);
        List<ProductViewModel> GetBestSellingProduct(int top);
        List<ProductViewModel> GetUpsellProducts(int top);

        List<TagViewModel> GetProductTags(int productId);

        bool CheckAvailability(int productId, int size, int color, int quantity);

        List<ColorViewModel> GetAvailableColor(int peoductId);

        List<SizeViewModel> GetAvailableSize(int peoductId);
    }
}
