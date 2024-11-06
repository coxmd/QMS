using FastReport.AdvMatrix;
using Microsoft.AspNetCore.SignalR;

namespace QueueManagementSystem.MVC.Data
{
    public class FingerprintHub : Hub
    {
        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }

        public async Task LeaveSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        }
    }
}
