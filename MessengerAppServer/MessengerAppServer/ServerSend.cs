using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerAppServer
{
    class ServerSend
    {
        public static void Welcome(int _toClient,string _str)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_str);
                _packet.Write(_toClient);
                SendTCPData(_toClient,_packet);

            }
        }

        private static void SendTCPData(int _toClient,Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
            
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }

        }

        #region Packets

        public static void AddChatter(int _toClient, Chatter _chatter)
        {
            using (Packet _packet = new Packet((int)ServerPackets.addChatter))
            {
                _packet.Write(_chatter.id);
                _packet.Write(_chatter.username);

                SendTCPData(_toClient, _packet);

            }
        }

        public static void ChatterDisconnected(int _clientId)
        {
            using (Packet _packet = new Packet((int)ServerPackets.chatterDisconnected))
            {
                _packet.Write(_clientId);

                SendTCPDataToAll(_packet);
            }
        }

        public static void ChatterMessage(int _clientId, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendChatterMessage))
            {
                _packet.Write(_clientId);
                _packet.Write(_msg);

                SendTCPDataToAll(_packet);
            }
        }

        public static void ServerChatMessage(string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverChatMessage))
            {
                _packet.Write(_msg);

                SendTCPDataToAll(_packet);
            }
        }

        #endregion

    }
}
