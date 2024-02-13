using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class MessagesSender
    {
        private ConnectionsHandler connectionsHandler;

        public MessagesSender(ConnectionsHandler connectionsHandler) {
            this.connectionsHandler = connectionsHandler;
        }

        public void SendToOne(Connection conn, string message)
        {
        }

        public void SendToAll(Connection connection, string message)
        {
            foreach(Connection conn in connectionsHandler.Connections){
                if (conn.Equals(connection))
                {
                    try
                    {
                        conn.Conn.Send(Encoding.UTF8.GetBytes("Message has sended"));
                    }
                    catch
                    {
                        throw new SendException();
                    }
                    continue;
                }

                try
                {
                    conn.Conn.Send(Encoding.UTF8.GetBytes(message));
                }
                catch
                {
                    throw new SendException();
                }
            }
        }

    }
}
