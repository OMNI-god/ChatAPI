using System.Collections.Generic;
using System.Collections.Specialized;

namespace SignalR_Test.ConnectionManager
{
    public class ConnectionManager : IConnectionManager
    {
        private StringDictionary connections = new StringDictionary();
        
        public void AddConnection(string userId,string connId)
        {
            if(IsConnected(userId))
            {
                //connections[userId] = connId;
                return;
            }
            connections.Add(userId,connId);
            Console.WriteLine(connections.Count);
        }

        public StringDictionary Connections()
        {
            return connections;
        }

        public bool IsConnected(string userId)
        {
            return connections.ContainsKey(userId);
        }

        public void RemoveConnection(string userId)
        {
            connections.Remove(userId);
        }
    }
}
