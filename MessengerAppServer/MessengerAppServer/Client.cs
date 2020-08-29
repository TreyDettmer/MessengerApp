using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace MessengerAppServer
{
    class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public Chatter chatter;
        public TCP tcp;

        public bool shouldDisconnect = false;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private byte[] receiveBuffer;
            private Packet receivedData;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the server!");
                

            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        
                        stream.BeginWrite(_packet.ToArray(),0,_packet.Length(),null,null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        Server.clients[id].Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);


                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);


                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine($"Error receiving TCP data: {ex}");
                    Server.clients[id].Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                //if this loop runs, it means that receivedData contains another complete packet which we can handle
                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id,_packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }

                }
                if (_packetLength <= 1)
                {
                    return true;
                }
                return false;
            }


            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }


        }

        public void SendIntoChat(string _playerName)
        {
            chatter = new Chatter(id, _playerName);


            foreach (Client _client in Server.clients.Values)
            {
                if (_client.chatter != null)
                {
                    if (_client.id != id)
                    {
                        ServerSend.AddChatter(id, _client.chatter);
                    }
                }
            }


            foreach (Client _client in Server.clients.Values)
            {
                if (_client.chatter != null)
                {
                    ServerSend.AddChatter(_client.id, chatter);
                }
            }
            ServerSend.ServerChatMessage($"{_playerName} joined the chat.");

        }

        public void Disconnect()
        {

                
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
            string _username = chatter.username;
            ThreadManager.ExecuteOnMainThread(() =>
            {
                chatter = null;
            });



            tcp.Disconnect();

            ServerSend.ChatterDisconnected(id);
            ServerSend.ServerChatMessage($"{_username} disconnected from chat.");
            

            

        }

        public void LeaveChat()
        {

        }



    }
}
