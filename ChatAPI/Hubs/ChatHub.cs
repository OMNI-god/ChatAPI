using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalR_Test.ConnectionManager;

namespace SignalR_Test.Hubs
{
    // [Authorize]
    public class ChatHub(IConnectionManager _manager) : Hub<IChatClient>, IChatHub
    {
        private readonly IConnectionManager _manager = _manager;

        public override async Task OnConnectedAsync()
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            await ConnectionMessage(Context.ConnectionId.ToString());
            await base.OnConnectedAsync();
            _manager.AddConnection(userId, Context.ConnectionId.ToString());
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await base.OnDisconnectedAsync(ex);
            _manager.RemoveConnection(Context.ConnectionId.ToString());
        }

        public async Task SendMessageToGroupAsync(string jsonData)
        {
            //await Clients.Group(groupID).ReceiveGroupMessageAsync(groupID, message);
        }

        public Task AddToGroupAsync(string groupID)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromGroupAsync(string groupId)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessageToUserAsync(string jsonData)
        {
            var jdata = JSONDeserialize(jsonData);
            if (_manager.IsConnected(jdata.to.ToString()))
            {
                if (!(string.IsNullOrEmpty(jdata.to.ToString()) || string.IsNullOrEmpty(jdata.from.ToString())))
                {
                }
                await Clients.Client(jdata.to.ToString()).ReceiveMessageAsync(jdata.msg.ToString());
            }
            else if (!(string.IsNullOrEmpty(jdata.to.ToString()) || string.IsNullOrEmpty(jdata.from.ToString())))
            {
            }
        }
        public async Task SendMessageToAllAsync(string jsonData)
        {
            var jdata = JSONDeserialize(jsonData);
            await Clients.All.ReceiveMessageAsync(jsonData);
        }
        private async Task ConnectionMessage(string connID)
        {
            await Clients.Client(connID).ReceiveConnIDAsync(connID);
        }
        private dynamic JSONDeserialize(string jsonData)
        {
            return JsonConvert.DeserializeObject<dynamic>(jsonData);
        }
    }
}
