using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Areas.Admin.Controllers
{
    public class MessageController : BaseController
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        public MessageController(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            var messages = _chatService.GetMessages();
            foreach (var item in messages)
            {
                if (item.ReceiverId.ToString() != "00000000-0000-0000-0000-000000000000")
                    item.ReceiverName = _userService.GetById(item.ReceiverId.ToString()).Result.Email;
            }
            return View(messages);
        }

    }
}