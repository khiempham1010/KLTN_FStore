using AtomStore.Utilities.Dtos;
using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Common;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using AtomStore.Data.Enums;
using AtomStore.Data.IRepositories;
using AtomStore.Infrastructure.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AtomStore.Utilities.Constants;
using AtomStore.Utilities.Helpers;
using OfficeOpenXml;

namespace AtomStore.Application.Implementation
{
    public class ProductService : IProductService
    {
        //private readonly IProductRepository _productRepository;
        //private IProductQuantityRepository _productQuantityRepository;
        //private IProductImageRepository _productImageRepository;
        ////ITagRepository _tagRepository;
        ////IProductTagRepository _productTagRepository;
        ////IUnitOfWork _unitOfWork;
        //public ProductService(
        //    IProductRepository productRepository,
        //    IProductQuantityRepository productQuantityRepository,
        //    IProductImageRepository productImageRepository,

        //    ITagRepository tagRepository,
        //    IProductTagRepository productTagRepository,
        //    IUnitOfWork unitOfWork
        //    )
        //{
        //    _productRepository = productRepository;
        //    _productQuantityRepository = productQuantityRepository;
        //    _productImageRepository = productImageRepository;
        //    _tagRepository = tagRepository;
        //    _productTagRepository = productTagRepository;
        //    _unitOfWork = unitOfWork;
        //}
        private IRepository<Tag, string> _tagRepository;
        private IRepository<ProductTag, int> _productTagRepository;
        private IRepository<ProductQuantity, int> _productQuantityRepository;
        private IRepository<ProductImage, int> _productImageRepository;
        private IRepository<Product, int> _productRepository;
        private IRepository<ProductCategory, int> _productCategoryRepository;
        private readonly IRepository<Color, int> _colorRepository;
        private readonly IRepository<Size, int> _sizeRepository;
        private IUnitOfWork _unitOfWork;

        public ProductService(IRepository<Product, int> productRepository,
            IRepository<Tag, string> tagRepository,
            IRepository<ProductQuantity, int> productQuantityRepository,
            IRepository<ProductImage, int> productImageRepository,
            IUnitOfWork unitOfWork,
            IRepository<ProductTag, int> productTagRepository,
            IRepository<ProductCategory, int> productCategoryRepository,
            IRepository<Color, int> colorRepository,
            IRepository<Size, int> sizeRepository)
        {

            _productRepository = productRepository;
            _tagRepository = tagRepository;
            _productQuantityRepository = productQuantityRepository;
            _productTagRepository = productTagRepository;
            _productImageRepository = productImageRepository;
            _unitOfWork = unitOfWork;
            _productCategoryRepository = productCategoryRepository;
            _colorRepository = colorRepository;
            _sizeRepository = sizeRepository;
        }

        public ProductViewModel Add(ProductViewModel productVM)
        {
            List<ProductTag> producttags = new List<ProductTag>();
            if (!string.IsNullOrEmpty(productVM.Tags))
            {
                string[] tags = productVM.Tags.Split(',');
                foreach (string t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);
                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag
                        {
                            Id = tagId,
                            Name = t,
                            Type = CommonConstants.ProductTag
                        };
                        _tagRepository.Add(tag);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId
                    };
                    producttags.Add(productTag);
                }
                var product = Mapper.Map<ProductViewModel, Product>(productVM);
                foreach (var producttag in producttags)
                {
                    product.ProductTags.Add(producttag);
                }
                _productRepository.Add(product);
            }
            return productVM;
        }

        public void AddQuantity(int productId, List<ProductQuantityViewModel> quantities)
        {
            _productQuantityRepository.RemoveMultiple(_productQuantityRepository.FindAll(x => x.ProductId == productId).ToList());
            foreach (var quantity in quantities)
            {
                _productQuantityRepository.Add(new ProductQuantity()
                {
                    ProductId = productId,
                    ColorId = quantity.ColorId,
                    SizeId = quantity.SizeId,
                    Quantity = quantity.Quantity
                });
            }
        }

        public void Delete(int id)
        {
            _productRepository.Remove(id);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<ProductViewModel> GetAll()
        {
            return _productRepository.FindAll(x => x.ProductCategory).ProjectTo<ProductViewModel>().ToList();
        }

        public PagedResult<ProductViewModel> GetAllPaging(int? categoryId, int? minPrice, int? maxPrice, string keyword, int page, int pageSize)
        {
            var query = _productRepository.FindAll(x => x.Status == Status.Active);
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));
            if (minPrice.HasValue || maxPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice && x.Price <= maxPrice);
            }
            if (categoryId.HasValue)
            {

                var childCategory = _productCategoryRepository.FindAll(x => x.ParentId == categoryId.Value || x.Id == categoryId.Value);
                //var newCategoryList = childCategory;
                foreach (var item in childCategory)
                {
                    if (item.Id != categoryId.Value)
                    {
                        var childCategory2 = _productCategoryRepository.FindAll(x => x.ParentId == item.Id);
                        childCategory = childCategory.Concat(childCategory2);
                    }
                }
                var productCategory = _productCategoryRepository.FindAll();
                if (childCategory != null)
                {
                    productCategory = productCategory.Except(childCategory);
                    foreach (var item in productCategory)
                    {
                        query = query.Except(_productRepository.FindAll(x => x.CategoryId == item.Id));
                    }
                }
                else
                {
                    query = query.Where(x => x.CategoryId == categoryId.Value);
                }
            }


            int totalRow = query.Count();

            query = query.OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ProjectTo<ProductViewModel>().ToList();

            var paginationSet = new PagedResult<ProductViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public PagedResult<ProductViewModel> GetAllPagingAdmin(int? categoryId, string keyword, int page, int pageSize)
        {
            var query = _productRepository.FindAll();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));
            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);

            int totalRow = query.Count();

            query = query.OrderByDescending(x => x.DateModified)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.ProjectTo<ProductViewModel>().ToList();

            var paginationSet = new PagedResult<ProductViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public ProductViewModel GetById(int id)
        {
            return Mapper.Map<Product, ProductViewModel>(_productRepository.FindById(id));
        }

        public List<ProductQuantityViewModel> GetQuantities(int productId)
        {
            return _productQuantityRepository.FindAll(x => x.ProductId == productId).ProjectTo<ProductQuantityViewModel>().ToList();
        }

        public void ImportExcel(string filePath, int categoryId)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                Product product;
                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    product = new Product();
                    product.CategoryId = categoryId;

                    product.Name = workSheet.Cells[i, 1].Value.ToString();

                    product.Description = workSheet.Cells[i, 2].Value.ToString();

                    decimal.TryParse(workSheet.Cells[i, 3].Value.ToString(), out var originalPrice);
                    product.OriginalPrice = originalPrice;

                    decimal.TryParse(workSheet.Cells[i, 4].Value.ToString(), out var price);
                    product.Price = price;
                    decimal.TryParse(workSheet.Cells[i, 5].Value.ToString(), out var promotionPrice);

                    product.PromotionPrice = promotionPrice;
                    product.Content = workSheet.Cells[i, 6].Value.ToString();
                    product.SeoKeywords = workSheet.Cells[i, 7].Value.ToString();

                    product.SeoDescription = workSheet.Cells[i, 8].Value.ToString();

                    bool.TryParse(workSheet.Cells[i, 10].Value.ToString(), out var homeFlag);
                    product.HomeFlag = homeFlag;

                    product.Status = Status.Active;

                    _productRepository.Add(product);
                }
            }
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(ProductViewModel productVm)
        {
            List<ProductTag> productTags = new List<ProductTag>();

            if (!string.IsNullOrEmpty(productVm.Tags))
            {
                string[] tags = productVm.Tags.Split(',');
                foreach (string t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);
                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag();
                        tag.Id = tagId;
                        tag.Name = t;
                        tag.Type = CommonConstants.ProductTag;
                        _tagRepository.Add(tag);
                    }
                    _productTagRepository.RemoveMultiple(_productTagRepository.FindAll(x => x.Id == productVm.Id).ToList());
                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId
                    };
                    productTags.Add(productTag);
                }
            }

            var product = Mapper.Map<ProductViewModel, Product>(productVm);
            foreach (var productTag in productTags)
            {
                product.ProductTags.Add(productTag);
            }
            _productRepository.Update(product);
        }

        public List<ProductImageViewModel> GetImages(int productId)
        {
            return _productImageRepository.FindAll(x => x.ProductId == productId)
                .ProjectTo<ProductImageViewModel>().ToList();
        }

        public void AddImages(int productId, string[] images)
        {
            _productImageRepository.RemoveMultiple(_productImageRepository.FindAll(x => x.ProductId == productId).ToList());
            foreach (var image in images)
            {
                _productImageRepository.Add(new ProductImage()
                {
                    Path = image,
                    ProductId = productId,
                    Caption = string.Empty
                });
            }
        }

        public List<ProductViewModel> GetLastest(int top)
        {
            return _productRepository.FindAll(x => x.Status == Status.Active).OrderByDescending(x => x.DateCreated)
                .Take(top).ProjectTo<ProductViewModel>().ToList();
        }

        //public List<ProductViewModel> GetHotProduct(int top)
        //{
        //    return _productRepository.FindAll(x => x.Status == Status.Active)
        //        .OrderByDescending(x => x.DateCreated)
        //        .Take(top)
        //        .ProjectTo<ProductViewModel>()
        //        .ToList();
        //}

        public List<ProductViewModel> GetRelatedProducts(int id, int top)
        {
            var product = _productRepository.FindById(id);
            return _productRepository.FindAll(x => x.Status == Status.Active
                && x.Id != id && x.CategoryId == product.CategoryId)
            .OrderByDescending(x => x.DateCreated)
            .Take(top)
            .ProjectTo<ProductViewModel>()
            .ToList();
        }

        public List<ProductViewModel> GetUpsellProducts(int top)
        {
            return _productRepository.FindAll(x => x.PromotionPrice != null)
               .OrderByDescending(x => x.DateModified)
               .Take(top)
               .ProjectTo<ProductViewModel>().ToList();
        }

        public List<TagViewModel> GetProductTags(int productId)
        {
            var tags = _tagRepository.FindAll();
            var productTags = _productTagRepository.FindAll();

            var query = from t in tags
                        join pt in productTags
                        on t.Id equals pt.TagId
                        where pt.ProductId == productId
                        select new TagViewModel()
                        {
                            Id = t.Id,
                            Name = t.Name
                        };
            return query.ToList();
        }

        public bool CheckAvailability(int productId, int size, int color, int quantity)
        {
            var productQuantity = _productQuantityRepository.FindSingle(x => x.ColorId == color && x.SizeId == size && x.ProductId == productId);
            if (productQuantity == null)
                return false;
            return productQuantity.Quantity >= quantity;
        }

        public List<ColorViewModel> GetAvailableColor(int productId)
        {
            List<ProductQuantity> quantitys = _productQuantityRepository.FindAll(x => x.ProductId == productId).ToList();
            List<Color> colors = _colorRepository.FindAll().ToList();

            var query =
                from q in quantitys
                group q by q.ColorId into a
                join c in colors on a.Key equals c.Id
                select c;

            return Mapper.Map<List<Color>,List<ColorViewModel>>(query.ToList());
        }

        public List<SizeViewModel> GetAvailableSize(int productId)
        {
            List<ProductQuantity> quantitys = _productQuantityRepository.FindAll(x => x.ProductId == productId).ToList();
            List<Size> sizes = _sizeRepository.FindAll().ToList();
            var query =
                from q in quantitys
                group q by q.SizeId into a
                join s in sizes on a.Key equals s.Id
                select s;

            return Mapper.Map<List<Size>, List<SizeViewModel>>(query.ToList());
        }
    }
}
