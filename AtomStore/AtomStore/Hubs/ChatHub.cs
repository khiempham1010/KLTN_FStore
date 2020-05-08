using AtomStore.Application.ViewModels.System;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtomStore.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(MessageViewModel message) =>
            await Clients.All.SendAsync("receiveMessage", message);
    }
}
