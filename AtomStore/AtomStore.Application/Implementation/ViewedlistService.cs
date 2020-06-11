using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using AtomStore.Infrastructure.Interfaces;
using AtomStore.Utilities.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Implementation
{
    public class ViewedlistService : IViewedlistService
    {
        private readonly IRepository<ViewedList, int> _viewedlistRepository;
        private IRepository<Product, int> _productRepository;
        private IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        public ViewedlistService(
            IRepository<ViewedList, int> viewedlistRepository,
            IRepository<Product, int> productRepository,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager
            )
        {
            _viewedlistRepository = viewedlistRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public void Create(ViewedlistViewModel viewedlistVm)
        {
            var viewedlist = Mapper.Map<ViewedlistViewModel, ViewedList>(viewedlistVm);
            _viewedlistRepository.Add(viewedlist);
        }

        public void Delete(int id)
        {
            _viewedlistRepository.Remove(id);
        }

        public List<ViewedlistViewModel> GetAll(Guid userId)
        {
            return _viewedlistRepository.FindAll(x => x.UserId == userId).OrderByDescending(x => x.DateCreated).ProjectTo<ViewedlistViewModel>().ToList();
        }

        public PagedResult<ViewedlistViewModel> GetAllPaging(Guid userId, int page, int pageSize)
        {
            var viewedlist = _viewedlistRepository.FindAll(x => x.UserId == userId);

            int totalRow = viewedlist.Count();

            viewedlist = viewedlist.OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var data = viewedlist.ProjectTo<ViewedlistViewModel>().ToList();

            var paginationSet = new PagedResult<ViewedlistViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public ViewedlistViewModel GetByProductAndUserId(int productId, Guid userId)
        {
            return Mapper.Map<ViewedList, ViewedlistViewModel>(_viewedlistRepository.FindSingle(x => x.ProductId == productId && x.UserId == userId));
        }


    }
}