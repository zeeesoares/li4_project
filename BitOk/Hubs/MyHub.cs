using Microsoft.AspNetCore.SignalR;

namespace BitOk.Hubs
{
    public class MyHub : Hub
    {
        public async Task NotifyClients(string message)
        {
            await Clients.All.SendAsync("ReceiveUpdate", message);
        }
    }
}
