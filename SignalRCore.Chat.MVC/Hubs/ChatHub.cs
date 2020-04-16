using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalRCore.Chat.MVC.Data;
using SignalRCore.Chat.MVC.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRCore.Chat.MVC.Hubs
{
    public class ChatHub:Hub
    {
        private readonly UserManager<ChatUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context, UserManager<ChatUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task BroadcastFromClient(string message)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(Context.User);

                Message m = new Message()
                {
                    MessageBody = message,
                    MessageDtTm = DateTime.Now,
                    FromUSer = currentUser
                };

                _context.Messages.Add(m);
                await _context.SaveChangesAsync();

                await Clients.All.SendAsync(
                    "Broadcast",
                    new
                    {
                        messageBody = m.MessageBody,
                        fromUser = currentUser.Email,
                        messageDtTm = m.MessageDtTm.ToString(
                            "hh:mm tt MMM dd", CultureInfo.InvariantCulture
                            )
                    });
            }
            catch (Exception ex)
            {

                await Clients.Caller.SendAsync("HubError", new { error = ex.Message });
            }
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync(
                "USerConnected",
                new
                {
                    connectionId = Context.ConnectionId,
                    connectionDtTm = DateTime.Now,
                    messageDtTm = DateTime.Now.ToString(
                            "hh:mm tt MMM dd", CultureInfo.InvariantCulture
                            )
                });
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("UserDisconnected",
                $"User disconnected, ConnectionId:{Context.ConnectionId}");
        }
    }
}
