using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Server.Net;
using RiptideNetworking;
using RiptideNetworking.Utils;

public class Gun : MonoBehaviour
{
    [MessageHandler((ushort)ClientToServerId.weapon)]
    private static void ReceiveWeapon(ushort fromClient, Message message)
    {
        Message msg = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.weapon);
        msg.AddUShort(message.GetUShort());
        msg.AddFloat(message.GetFloat());
        NetworkManager.Singleton.Server.SendToAll(msg);
    }
    
}
