using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ChatikSDavidom.Components.Client
{
    public class Server
    {
        public Server()
        {
            
        }

        private TcpListener m_Listener;
        private List<Client> m_ConnectedClients;
        
    }
}