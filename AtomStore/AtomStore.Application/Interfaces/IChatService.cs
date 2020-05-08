using AtomStore.Application.ViewModels.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface IChatService
    {
        ICollection<MessageViewModel> GetMessages();
        MessageViewModel Add(MessageViewModel messageVM);
        void Save();
    }
}
