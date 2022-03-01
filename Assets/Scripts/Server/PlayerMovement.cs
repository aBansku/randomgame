using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using Server.Net;

namespace Server
{
    public class PlayerMovement : MonoBehaviour
    {
        [MessageHandler((ushort)ClientToServerId.position)]
        private static void ReceivePlayerPositions(ushort fromClient, Message message)
        {
            Message msg = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerPosition);
            msg.AddUShort(message.GetUShort());
            msg.AddVector3(message.GetVector3());
            msg.AddQuaternion(message.GetQuaternion());
            msg.AddVector3(message.GetVector3());
            NetworkManager.Singleton.Server.SendToAll(msg);    
        }
    }
}

