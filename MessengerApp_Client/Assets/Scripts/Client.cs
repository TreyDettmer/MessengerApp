using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Text;
using System.Text.RegularExpressions;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 32137;
    public int myId = 0;
    public TCP tcp;

    public string username;
    private float lastMessageSentTime = 0f;
    public float messageSendDelay = 5f;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    /// <summary>
    /// Initialize Singleton
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
    }

    private void OnApplicationQuit()
    {
        Disconnect(false);
    }

    public void ConnectToServer(string _username,string ipAddress = "",string _port = "")
    {


        username = _username;
        
        if (ipAddress != "")
        {
            try
            {
                ip = ipAddress;
                port = int.Parse(_port);
            }
            catch
            {
                port = 1;
            }
        }

        InitializeClientData();

        isConnected = true;
        tcp.Connect();
        
    }



    public class TCP
    {
        public TcpClient socket;
        public bool connected = false;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;


        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize

            };
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            instance.StartConnectionDelayRoutine();
            

        }

        /// <summary>
        /// Called once we connected to the server or failed to connect
        /// </summary>
        /// <param name="_result"></param>
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }
            connected = true;
            stream = socket.GetStream();
            receivedData = new Packet();

            // Start listening for information from the server
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

        }

        /// <summary>
        /// Send packet to server
        /// </summary>
        /// <param name="_packet">packet to send</param>
        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {

                Debug.Log($"Error sending data to server via TCP: {ex}");
            }
        }

        /// <summary>
        /// Called when we receive information from the server
        /// </summary>
        /// <param name="_result"></param>
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);
                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);


            }
            catch
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Determines whether a full packet was received
        /// </summary>
        /// <param name="_data"></param>
        /// <returns></returns>
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
                        packetHandlers[_packetId](_packet);
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

        private void Disconnect()
        {
            if (instance)
            {
                instance.Disconnect();
            }
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }




    }



    /// <summary> Initalize the packet handlers so that we can properly handle information sent from server. </summary>
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome,ClientHandle.Welcome },
            {(int)ServerPackets.addChatter,ClientHandle.AddChatter },
            {(int)ServerPackets.sendChatterMessage,ClientHandle.SendMessage },
            {(int)ServerPackets.chatterDisconnected,ClientHandle.ChatterDisconnected },
            {(int)ServerPackets.serverChatMessage,ClientHandle.ServerChatMessage },
        };
        Debug.Log("Initialized packets.");
    }

    /// <summary>
    /// Send packet to server requesting to add a message to the chat
    /// </summary>
    /// <param name="msg"> the message to send</param>
    /// <returns></returns>
    public bool SendMessageToChat(string msg)
    {
        if (Time.time - lastMessageSentTime >= messageSendDelay)
        {
            
            ClientSend.ChatterMessage(msg);
            lastMessageSentTime = Time.time;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Start the connection delay routine.
    /// </summary>
    public void StartConnectionDelayRoutine()
    {
        StopAllCoroutines();
        StartCoroutine(ConnectionDelayRoutine());
    }

    /// <summary>
    /// If we haven't connected to the server after two seconds, we probably will never connect
    /// </summary>
    /// <returns></returns>
    public IEnumerator ConnectionDelayRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (tcp != null)
        {
            if (tcp.socket != null)
            {
                if (!tcp.socket.Connected)
                {
                    
                    
                    SceneManager.LoadScene(0);
                    MainMenuManager.errorMessage = "Error: Failed to find/connect to server.";
                }

            }
        }

    }

    public void LeaveChat()
    {
        if (isConnected)
        {
            
            Disconnect();
            AppManager.chatters.Clear();
        }
    }


    /// <summary>
    /// Disconnect from the server
    /// </summary>
    /// <param name="loadMainMenu">whether we should load the main menu</param>
    private void Disconnect(bool loadMainMenu = true)
    {
        if (isConnected)
        {
            
            isConnected = false;
            tcp.socket.Close();
            
            Debug.Log("Disconnected from the server.");
            if (loadMainMenu)
            {
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    SceneManager.LoadScene(0);
                });
                
            }

            

        }
    }

}
