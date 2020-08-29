using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    #region Packets

    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(Client.instance.username);

            SendTCPData(_packet);
        }
    }

    public static void ChatterMessage(string msg)
    {
        using (Packet _packet = new Packet((int)ClientPackets.chatterMessage))
        {
            _packet.Write(msg);
            SendTCPData(_packet);
        }
    }

    public static void ChatterLeftChat(int _fromClient)
    {
        using (Packet _packet = new Packet((int)ClientPackets.chatterLeftChat))
        {
            _packet.Write(_fromClient);
            SendTCPData(_packet);
        }
    }

    #endregion


}
