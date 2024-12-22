namespace SignalR_Test.ConnectionManager
{
    public class ConnectionManager : IConnectionManager
    {
        private HashSet<string> connections = new HashSet<string>();
        
        public void AddConnection(string connID)
        {
            connections.Add(connID);
        }

        public HashSet<string> Connections()
        {
            return connections;
        }

        public bool IsConnected(string connID)
        {
            return connections.Any(x=>x.Equals(connID));
        }

        public void RemoveConnection(string connID)
        {
            connections.Remove(connID);
        }
    }
}
