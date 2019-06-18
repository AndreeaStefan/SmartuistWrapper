using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Connection
{
    public class DataForwarder
    {
        private const string Host = "127.0.0.1";
        private const int Port = 7000;
        private Socket clientSocket;

        public DataForwarder()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAdd = IPAddress.Parse(Host);
            var remoteEP = new IPEndPoint(ipAdd, Port);
            try
            {
                clientSocket.Connect(remoteEP);
            }
            catch
            {
                Debug.Log("Could not connect");
            }
        }

        public void Send(string data)
        {
            var byData = System.Text.Encoding.UTF8.GetBytes(data);
            clientSocket.Send(byData);
        }

        public bool IsConnected()
        {
            return clientSocket.Connected;
        }



    }
}