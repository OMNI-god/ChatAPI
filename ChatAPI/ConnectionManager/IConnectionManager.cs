namespace SignalR_Test.ConnectionManager
{
    public interface IConnectionManager
    {
        void AddConnection(string connID);
        void RemoveConnection(string connID);
        bool IsConnected(string connID);
        HashSet<string> Connections();
    }
}
