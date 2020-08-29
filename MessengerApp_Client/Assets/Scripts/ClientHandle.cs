using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _id = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _id;
        ClientSend.WelcomeReceived();
    }

    public static void AddChatter(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();

        AppManager.instance.AddChatter(_id, _username);
        if (MainManager.instance != null)
        {
            MainManager.instance.UpdateLobbyPanel();
        }
    }

    public static void SendMessage(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _message = _packet.ReadString();
        ChatterManager _chatter = AppManager.chatters[_id];
        if (_chatter != null)
        {
            if (MainManager.instance != null)
            {
                MainManager.instance.AddMessageToChatPanel(_id, _message, _chatter);
            }
        }
    }

    public static void ChatterDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Destroy(AppManager.chatters[_id].gameObject);
        AppManager.chatters.Remove(_id);
        if (MainManager.instance != null)
        {
            MainManager.instance.UpdateLobbyPanel();
        }
       
    }

    public static void ServerChatMessage(Packet _packet)
    {
        int _id = 0;
        string _message = _packet.ReadString();
        if (MainManager.instance != null)
        {
            MainManager.instance.AddMessageToChatPanel(_id, _message, null);
        }
    }
}
