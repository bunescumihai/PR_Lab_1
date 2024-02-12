using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ConnectionsHandler
    {
        private List<Connection> connections = new List<Connection>();
        private int i = 0;
        private Connection conn;

        public ConnectionsHandler()
        {
            Thread connectionCheckerThread = new Thread(new ThreadStart(ConnectionsChecker));
            connectionCheckerThread.Start();
        }

        public void Add(Connection connection)
        {
            this.connections.Add(connection);
            Console.WriteLine("Server: New connection");
            Console.WriteLine($"Server: Connections at the moment - {connections.Count}");
        }

        public void Remove(Connection connection)
        {
            this.connections.Remove(connection);
        }

        public Connection NeedToServe()
        {
            while (true)
            {
                try
                {
                    conn = connections.ElementAt(i);
                    i++;
                }
                catch (ArgumentOutOfRangeException)
                {

                    i = 0;
                    Thread.Sleep(100);
                    continue;
                }

                if(conn.Conn.Available > 0)
                    return conn;
            }
        }

        private void ConnectionsChecker()
        {
            Connection conn;

            while (true)
            {
                Thread.Sleep(100);

                for (int i = 0; i < connections.Count; i++)
                {
                    conn = connections.ElementAt(i);
                    if (!conn.InServing)
                    {
                        if(!(conn.Conn.Available > 0) && conn.Conn.Poll(1000, SelectMode.SelectRead))
                        {
                            connections.Remove(conn);
                            Console.WriteLine("Server: A connection has interupted");
                            Console.WriteLine($"Server: Connections at the moment - {connections.Count}");
                            i--;
                        }
                    }
                }
            }
        }
    }
}
