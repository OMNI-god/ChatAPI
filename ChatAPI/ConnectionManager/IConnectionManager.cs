using System.Collections.Specialized;

namespace SignalR_Test.ConnectionManager
{
    public interface IConnectionManager
    {
        void AddConnection(string userId,string connId);
        void RemoveConnection(string userId);
        bool IsConnected(string userId);
        StringDictionary Connections();
    }
}
