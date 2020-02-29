using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Common;
using AtomStore.Data.Entities;
using AtomStore.Infrastructure.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Implementation
{
    public class CommonService : ICommonService
    {
        IRepository<SystemConfig, string> _systemConfigRepository;
        IUnitOfWork _unitOfWork;
        public CommonService(
            IRepository<SystemConfig, string> systemConfigRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _systemConfigRepository = systemConfigRepository;
        }


        public SystemConfigViewModel GetSystemConfig(string code)
        {
            return Mapper.Map<SystemConfig, SystemConfigViewModel>(_systemConfigRepository.FindSingle(x => x.Id == code));
        }
    }
}
