using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Implementation;
using AtomStore.Application.ViewModels.System;
using AtomStore.Data.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Controllers
{
    public class ChatController : Controller
    {
        public readonly UserService _userService;
        public readonly ChatService _chatService;
        public readonly UserManager<AppUser> _userManager;

        public ChatController(
            UserService userService,
            ChatService chatService,
            UserManager<AppUser> userManager
            )
        {
            _userService = userService;
            _chatService = chatService;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = currentUser.UserName;
            var messages = _chatService.GetMessages();
            return View();
        }

        public async Task<IActionResult> Create(MessageViewModel message)
        {
            if (ModelState.IsValid)
            {
                message.Name = User.Identity.Name;
                var appUser = Mapper.Map<AppUser,AppUserViewModel >(await _userManager.GetUserAsync(User));
                message.UserId = appUser.Id;
                _chatService.Add(message);
                _chatService.Save();
                return Ok();

            }
            return BadRequest();
        }
    }
}