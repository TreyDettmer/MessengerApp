using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerAppServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient,Packet _packet)
        {
            int _clientToCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_clientToCheck].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}");
            if (_fromClient != _clientToCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientToCheck})!");
            }
            Server.clients[_fromClient].SendIntoChat(_username);
        }

        public static void ChatterMessage(int _fromClient,Packet _packet)
        {
            string _msg = _packet.ReadString();

            Server.clients[_fromClient].chatter.SetMessage(_msg);
        }

        public static void ChatterLeftChat(int _fromClient,Packet _packet)
        {
            int _id = _packet.ReadInt();
            ServerSend.ChatterDisconnected(_id);
            
        }
    }
}
