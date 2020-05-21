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
        public readonly IChatService _chatService;
        public readonly UserManager<AppUser> _userManager;

        public MessagesViewComponent(
            IChatService chatService,
            UserManager<AppUser> userManager
            )
        {
            _chatService = chatService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync(UserClaimsPrincipal);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }
            var messages = _chatService.GetMessages();
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
