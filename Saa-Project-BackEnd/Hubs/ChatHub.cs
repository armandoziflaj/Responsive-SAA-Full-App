using Microsoft.AspNetCore.SignalR;

namespace Saa_Project_BackEnd.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }
}