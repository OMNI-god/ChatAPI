namespace SignalR_Test.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessageAsync(string message);
        Task ReceiveConnIDAsync(string message);
        Task ReceiveGroupMessageAsync(string groupID, string groupName);
    }
}
