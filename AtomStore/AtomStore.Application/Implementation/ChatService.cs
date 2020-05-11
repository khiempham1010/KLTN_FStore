using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.System;
using AtomStore.Data.Entities;
using AtomStore.Infrastructure.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Implementation
{
    public class ChatService : IChatService
    {
        public readonly IRepository<Message, int> _messageRepository;
        private IUnitOfWork _unitOfWork;
        public ChatService(IRepository<Message, int> messageRepository, IUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
        }
        public ICollection<MessageViewModel> GetMessages()
        {
            var message = Mapper.Map<List< Message>,List<MessageViewModel>>( _messageRepository.FindAll().ToList());
            return message;
        }

        public MessageViewModel Add(MessageViewModel messageVM)
        {
            var message = Mapper.Map<MessageViewModel, Message>(messageVM);
            _messageRepository.Add(message);
            return messageVM;
        }
        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
