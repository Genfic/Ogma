using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Ogma3.Services.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task Notify(string text)
        {
            await Clients.All.SendAsync("ReceiveNotification", $"Received: {text}");
        }
    }
}