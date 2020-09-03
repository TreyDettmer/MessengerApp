using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles sending data to the server.
/// </summary>
public class ClientSend : MonoBehaviour
{
    /// <summary>
    /// Sends TCP data to server
    /// </summary>
    /// <param name="_packet"></param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    #region Packets

    /// <summary>
    /// Send client info once we connect to server
    /// </summary>
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(Client.instance.username);

            SendTCPData(_packet);
        }
    }

    /// <summary>
    /// Send a request to add a message to the chat
    /// </summary>
    /// <param name="msg"></param>
    public static void ChatterMessage(string msg)
    {
        using (Packet _packet = new Packet((int)ClientPackets.chatterMessage))
        {
            _packet.Write(msg);
            SendTCPData(_packet);
        }
    }



    #endregion


}
