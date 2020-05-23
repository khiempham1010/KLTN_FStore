using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.System;
using AtomStore.Data.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtomStore.Controllers.Components
{
    [Authorize]
    public class MessagesViewComponent : ViewComponent
    {
        private readonly IChatService _chatService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserService _userService;

        public MessagesViewComponent(
            IChatService chatService,
            UserManager<AppUser> userManager,
            IUserService userService
            )
        {
            _chatService = chatService;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync(UserClaimsPrincipal);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }
            var messages = _chatService.GetMessages();
            foreach (var item in messages)
            {
                if (item.ReceiverId.ToString() != "00000000-0000-0000-0000-000000000000")
                    item.ReceiverName = _userService.GetById(item.ReceiverId.ToString()).Result.Email;
            }
            return View(messages);
        }
        public async Task<IActionResult> Create(MessageViewModel message)
        {
            if (ModelState.IsValid)
            {
                message.Name = User.Identity.Name;
                var appUser = Mapper.Map<AppUser, AppUserViewModel>(await _userManager.GetUserAsync(UserClaimsPrincipal));
                message.UserId = appUser.Id.Value;
                message.When = DateTime.Now;
                _chatService.Add(message);
                _chatService.Save();
                return new OkResult();
            }
            return new BadRequestResult();
        }
    }
}
