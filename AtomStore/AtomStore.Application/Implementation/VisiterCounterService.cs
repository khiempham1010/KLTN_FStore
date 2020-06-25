using AtomStore.Application.Interfaces;
using AtomStore.Data.Entities;
using AtomStore.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Implementation
{
    public class VisitorCounterService : IVisitorCounterService
    {
        private readonly IRepository<Visitor,int> _visitorRepository;
        private readonly IUnitOfWork _unitOfWork;
        public VisitorCounterService(IRepository<Visitor, int> visitorRepository, IUnitOfWork unitOfWork)
        {
            _visitorRepository = visitorRepository;
            _unitOfWork = unitOfWork;
        }

        public int GetVisitors()
        {
            var no = _visitorRepository.FindAll().FirstOrDefault().NumberOfVisitors;
            if (no == default)
                return 0;
            else return no;
        }

        public void SetVisitors()
        {
            var a = _visitorRepository.FindAll().FirstOrDefault();
            if (a == default)
            {
                _visitorRepository.Add(new Visitor(1));
                _unitOfWork.Commit();
                return;
            }
            a.NumberOfVisitors += 1;
            _visitorRepository.Update(a);
            _unitOfWork.Commit();
        }
    }
}
