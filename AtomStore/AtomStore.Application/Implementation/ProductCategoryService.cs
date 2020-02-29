using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using AtomStore.Data.Enums;
using AtomStore.Data.IRepositories;
using AtomStore.Infrastructure.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Implementation
{
    public class ProductCategoryService : IProductCategoryService
    {
        private IProductCategoryRepository _productCategoryRepository;
        private IUnitOfWork _unitOfWork;
        public ProductCategoryService(IProductCategoryRepository productCategoryRepository, IUnitOfWork unitOfWork)
        {
            _productCategoryRepository = productCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        public ProductCategoryViewModel Add(ProductCategoryViewModel productCategoryVm)
        {
            var productCategory = Mapper.Map<ProductCategoryViewModel, ProductCategory>(productCategoryVm);
            _productCategoryRepository.Add(productCategory);
            return productCategoryVm;
        }

        public void Delete(int id)
        {
            _productCategoryRepository.Remove(id);
        }

        public List<ProductCategoryViewModel> GetAll()
        {
            return _productCategoryRepository.FindAll().OrderBy(x => x.ParentId)
                 .ProjectTo<ProductCategoryViewModel>().ToList();
        }

        public List<ProductCategoryViewModel> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
                return _productCategoryRepository.FindAll(x => x.Name.Contains(keyword))
                    .OrderBy(x => x.ParentId).ProjectTo<ProductCategoryViewModel>().ToList();
            else
                return _productCategoryRepository.FindAll().OrderBy(x => x.ParentId)
                    .ProjectTo<ProductCategoryViewModel>()
                    .ToList();
        }

        public List<ProductCategoryViewModel> GetAllByParentId(int parentId)
        {
            return _productCategoryRepository.FindAll(x => x.Status == Status.Active
            && x.ParentId == parentId)
             .ProjectTo<ProductCategoryViewModel>()
             .ToList();
        }

        public ProductCategoryViewModel GetById(int id)
        {
            return Mapper.Map<ProductCategory, ProductCategoryViewModel>(_productCategoryRepository.FindById(id));
        }

        public List<ProductCategoryViewModel> GetHomeCategories(int top)
        {
            var query = _productCategoryRepository
                .FindAll(x => x.HomeFlag == true, c => c.Products)
                  .OrderBy(x => x.HomeOrder)
                  .Take(top).ProjectTo<ProductCategoryViewModel>();

            var categories = query.ToList();
            foreach (var category in categories)
            {
                //category.Products = _productRepository
                //    .FindAll(x => x.HotFlag == true && x.CategoryId == category.Id)
                //    .OrderByDescending(x => x.DateCreated)
                //    .Take(5)
                //    .ProjectTo<ProductViewModel>().ToList();
            }
            return categories;
        }

        public void ReOrder(int sourceId, int targetId, string point)
        {

            //source.SortOrder = target.SortOrder;
            //target.SortOrder = tempOrder;

            //_productCategoryRepository.Update(source);
            //_productCategoryRepository.Update(target);

            var source = _productCategoryRepository.FindById(sourceId);
            var target = _productCategoryRepository.FindById(targetId);
            var productCategorys = _productCategoryRepository.FindAll().ProjectTo<ProductCategoryViewModel>().ToList();
            var sortProductCategorys = productCategorys.OrderBy(x => x.SortOrder).Where(x=>x.ParentId==source.ParentId);

            int targeSortOrdert = target.SortOrder;
            int sourceSortOrder = source.SortOrder;

            //if move product category from root to root
            if (source.ParentId != null && target.ParentId == null)
            {
                source.ParentId = null;

                foreach (var item in sortProductCategorys)
                {
                    if (item.SortOrder >= targeSortOrdert)
                    {
                        var productCategoryViewModel = _productCategoryRepository.FindById(item.Id);
                        productCategoryViewModel.SortOrder += 1;
                        _productCategoryRepository.Update(productCategoryViewModel);
                    }
                }
                source.SortOrder = targeSortOrdert;
                _productCategoryRepository.Update(source);
            }
            else
            {
                if (targeSortOrdert > sourceSortOrder)
                {
                    int temp = sourceSortOrder;
                    sourceSortOrder = targeSortOrdert;
                    targeSortOrdert = temp;
                }

                foreach (var item in sortProductCategorys)
                {
                    if (item.SortOrder >= targeSortOrdert && item.SortOrder < sourceSortOrder)
                    {
                        var productCategoryViewModel = _productCategoryRepository.FindById(item.Id);
                        productCategoryViewModel.SortOrder += 1;
                        _productCategoryRepository.Update(productCategoryViewModel);
                    }
                }

                source.SortOrder = targeSortOrdert;
                _productCategoryRepository.Update(source);
                var sortProductCategorys1 = productCategorys.OrderBy(x => x.SortOrder).Where(x => x.ParentId == source.ParentId).ToList();
                for (int i = 0; i < sortProductCategorys.Count(); i++)
                {
                    var productCategoryViewModel = _productCategoryRepository.FindById(sortProductCategorys1[i].Id);
                    productCategoryViewModel.SortOrder = i;
                    _productCategoryRepository.Update(productCategoryViewModel);
                }
            }
            
            
            //move Product category from an folder to root
            //else 
            //{
            //    source1.ParentId = null;
            //    if (point == "top")
            //    {

            //    }
            //}

            //source.SortOrder = targetId;
            //for (int i = sourceId - 1; i <= targetId; i--)
            //{
            //    var target = _productCategoryRepository.FindById(i);
            //    target.SortOrder += 1;
            //    _productCategoryRepository.Update(target);
            //}
            //_productCategoryRepository.Update(source);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(ProductCategoryViewModel productCategoryVm)
        {
            var productCategory = Mapper.Map<ProductCategoryViewModel, ProductCategory>(productCategoryVm);
            _productCategoryRepository.Update(productCategory);
        }

        public void UpdateParentId(int sourceId, int targetId, Dictionary<int, int> items)
        {
            var sourceCategory = _productCategoryRepository.FindById(sourceId);
            sourceCategory.ParentId = targetId;
            _productCategoryRepository.Update(sourceCategory);

            //Get all sibling
            var sibling = _productCategoryRepository.FindAll(x => items.ContainsKey(x.Id));
            foreach (var child in sibling)
            {
                child.SortOrder = items[child.Id];
                _productCategoryRepository.Update(child);
            }
        }
    }
}
