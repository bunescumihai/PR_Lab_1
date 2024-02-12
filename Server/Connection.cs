using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Connection
    {
        private Socket connection;
        private bool inServing = false;

        public Connection(Socket connection)
        {
            this.connection = connection;
        }

        public Socket Conn
        {
            get { return connection; }
        }

        public bool InServing
        { 
            get { return  inServing; }
            set { inServing = value; }
        }

    }
}
