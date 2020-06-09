using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Common;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Application.ViewModels.System;
using AtomStore.Data.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile:Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ProductCategoryViewModel, ProductCategory>()
                .ConstructUsing(c => new ProductCategory(c.Name, c.ParentId, c.HomeOrder, c.HomeFlag,
                c.SortOrder, c.Status, c.SeoPageTitle, c.SeoAlias, c.SeoKeywords, c.SeoDescription));
            CreateMap<ProductViewModel, Product>()
                .ConstructUsing(c => new Product(c.Name, c.CategoryId, c.Image, c.Price, c.OriginalPrice,
                c.PromotionPrice, c.Description, c.Content, c.HomeFlag, c.Tags, c.Unit, c.Status,
                c.SeoPageTitle, c.SeoAlias, c.SeoKeywords, c.SeoDescription,c.DateCreated,c.DateModified));
            CreateMap<AppUserViewModel, AppUser>()
                .ConstructUsing(c => new AppUser(c.Id.GetValueOrDefault(Guid.Empty), c.FullName, c.UserName, c.Email, c.PhoneNumber, c.Avatar, c.Address, c.Status));
            CreateMap<PermissionViewModel, Permission>()
                .ConstructUsing(c => new Permission(c.RoleId, c.FunctionId, c.CanCreate, c.CanRead, c.CanUpdate, c.CanDelete));

            CreateMap<OrderViewModel, Order>()
              .ConstructUsing(c => new Order(c.Id, c.CustomerName, c.CustomerAddress,
              c.CustomerPhone, c.CustomerMessage, c.OrderStatus,
              c.PaymentMethod, c.Status, c.CustomerId, c.CustomerEmail));

            CreateMap<OrderDetailViewModel, OrderDetail>()
              .ConstructUsing(c => new OrderDetail(c.Id, c.OrderId, c.ProductId,
              c.Quantity, c.Price, c.ColorId, c.SizeId));


            CreateMap<ContactViewModel, Contact>()
               .ConstructUsing(c => new Contact(c.Id, c.Name, c.Phone, c.Email, c.Website, c.Address, c.Other, c.Lng, c.Lat, c.Status));

            CreateMap<FeedbackViewModel, Feedback>()
                .ConstructUsing(c => new Feedback(c.Id, c.Name, c.Email, c.Message, c.Status));
            CreateMap<MessageViewModel, Message>()
                .ConstructUsing(c => new Message(c.Id, c.Name, c.Text, c.When, c.UserId, c.ReceiverId));
            CreateMap<WishlistViewModel, WishList>()
                .ConstructUsing(c => new WishList(c.Id, c.UserId, c.ProductId, c.Email, c.ProductName, c.DateCreated, c.DateModified));
            CreateMap<ProductFeedbackViewModel, ProductFeedback>()
                .ConstructUsing(c => new ProductFeedback(c.ProductId, c.Title, c.Content, c.Rating, c.ParentId, c.DateCreated, c.DateModified, c.OwnerId, c.Like, c.Image));
        }
    }
}
