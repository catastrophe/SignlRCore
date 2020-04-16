using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SignalRCore.Chat.MVC.Models
{
    public class ChatUser:IdentityUser
    {
        public virtual ICollection<Message> Messages { get; set; }
    }
}