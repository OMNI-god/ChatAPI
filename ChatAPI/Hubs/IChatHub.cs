namespace SignalR_Test.Hubs
{
    public interface IChatHub
    {
        Task SendMessageToUserAsync(string jsonData);
        Task SendMessageToGroupAsync(string jsonData);
        Task AddToGroupAsync(string groupID);
        Task RemoveFromGroupAsync(string groupId);
    }
}
