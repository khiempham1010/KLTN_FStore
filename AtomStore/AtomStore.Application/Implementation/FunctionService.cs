using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.System;
using AtomStore.Data.Entities;
using AtomStore.Data.Enums;
using AtomStore.Data.IRepositories;
using AtomStore.Infrastructure.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomStore.Application.Implementation
{
    public class FunctionService : IFunctionService
    {
        private IFunctionRepository _functionRepository;
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private IRepository<Permission, int> _permissionRepository;
        private RoleManager<AppRole> _roleManager;

        public FunctionService(IMapper mapper,
             IFunctionRepository functionRepository,
             IUnitOfWork unitOfWork,
             IRepository<Permission, int> permissionRepository,
             RoleManager<AppRole> roleManager)
        {
            _functionRepository = functionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _permissionRepository = permissionRepository;
            _roleManager = roleManager;
        }
        public bool CheckExistedId(string id)
        {
            return _functionRepository.FindById(id) != null;
        }

        public void Add(FunctionViewModel functionVm)
        {
            var function = _mapper.Map<Function>(functionVm);
            _functionRepository.Add(function);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public void Delete(string id)
        {
            _functionRepository.Remove(id);
        }
        public FunctionViewModel GetById(string id)
        {
            var function = _functionRepository.FindSingle(x => x.Id == id);
            return Mapper.Map<Function, FunctionViewModel>(function);
        }


        public Task<List<FunctionViewModel>> GetAll(string filter)
        {
            var query = _functionRepository.FindAll();
            if (!string.IsNullOrEmpty(filter))
            {
                var permissions = new List<Permission>();
                foreach (var item in filter.Split(";"))
                {
                    var role = _roleManager.FindByNameAsync(item).Result;
                    permissions.AddRange(_permissionRepository.FindAll(x=>x.RoleId==role.Id&&x.CanRead==true).ToList());
                }
                var b = _functionRepository.FindAll(x => x.ParentId == null);
                var a = (from q in query
                         join f in permissions on q.Id equals f.FunctionId
                         select q);
                var c = a.Concat(b);
                return c.ProjectTo<FunctionViewModel>().ToListAsync();
            }
            
            return query.ProjectTo<FunctionViewModel>().ToListAsync();
        }

        public IEnumerable<FunctionViewModel> GetAllWithParentId(string parentId)
        {
            return _functionRepository.FindAll(x => x.ParentId == parentId).ProjectTo<FunctionViewModel>();
        }
        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(FunctionViewModel functionVm)
        {

            var functionDb = _functionRepository.FindById(functionVm.Id);
            var function = _mapper.Map<Function>(functionVm);
        }

        public void ReOrder(string sourceId, string targetId)
        {

            var source = _functionRepository.FindById(sourceId);
            var target = _functionRepository.FindById(targetId);
            int tempOrder = source.SortOrder;

            source.SortOrder = target.SortOrder;
            target.SortOrder = tempOrder;

            _functionRepository.Update(source);
            _functionRepository.Update(target);

        }
        public void UpdateParentId(string sourceId, string targetId, Dictionary<string, int> items)
        {
            //Update parent id for source
            var category = _functionRepository.FindById(sourceId);
            category.ParentId = targetId;
            _functionRepository.Update(category);

            //Get all sibling
            var sibling = _functionRepository.FindAll(x => items.ContainsKey(x.Id));
            foreach (var child in sibling)
            {
                child.SortOrder = items[child.Id];
                _functionRepository.Update(child);
            }
        }
    }
}
