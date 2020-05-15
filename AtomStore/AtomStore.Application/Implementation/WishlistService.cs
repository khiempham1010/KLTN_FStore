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
    public class WishlistService : IWishlistService
    {
        private readonly IRepository<WishList, int> _wishlistRepository;
        private IRepository<Product, int> _productRepository;
        private IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        public WishlistService(
            IRepository<WishList, int> wishlistRepository,
            IRepository<Product, int> productRepository,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager
            )
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public void Create(WishlistViewModel wishlistVm)
        {
            var wishlist = Mapper.Map<WishlistViewModel, WishList>(wishlistVm);
            _wishlistRepository.Add(wishlist);
        }

        public void Delete(int id)
        {
            _wishlistRepository.Remove(id);
        }

        public List<WishlistViewModel> GetAll(Guid userId)
        {
            return _wishlistRepository.FindAll(x => x.UserId == userId).ProjectTo<WishlistViewModel>().ToList();
        }

        public PagedResult<WishlistViewModel> GetAllPaging(Guid userId, int page, int pageSize)
        {
            var wishlist = _wishlistRepository.FindAll(x => x.UserId == userId);

            int totalRow = wishlist.Count();

            wishlist = wishlist.OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var data = wishlist.ProjectTo<WishlistViewModel>().ToList();

            var paginationSet = new PagedResult<WishlistViewModel>()
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

        public WishlistViewModel GetByProductAndUserId(int productId, Guid userId)
        {
            return Mapper.Map<WishList, WishlistViewModel>(_wishlistRepository.FindSingle(x => x.ProductId == productId&&x.UserId==userId));
        }


    }
}
