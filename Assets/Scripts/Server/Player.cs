using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Server.Net;

namespace Server
{
   public class Player : MonoBehaviour
   {
       public int warnings;
   
       public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
   
       public ushort Id { get; private set; }
       public string username { get; private set; }
   
       public Rigidbody rb;
       public CharacterController controller;
   
       public LayerMask ground;
       public Transform groundCheck;
   
       private void Update()
       {
           if (warnings == 3)
           {
               NetworkManager.Singleton.Server.DisconnectClient(Id);
           }
       }
   
       private void OnDestroy()
       {
           list.Remove(Id);
       }
   
       public static void Spawn(ushort id, string username)
       {
           foreach (Player otherPlayer in list.Values)
               otherPlayer.SendSpawned(id);
   
           Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector3(500f, 800f, 300f), Quaternion.identity).GetComponent<Player>();
           player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
           player.Id = id;
           player.username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;
   
           player.SendSpawned();
           list.Add(id, player);
       }
   
       #region Messages
       private void SendSpawned()
       {
           NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned)));
       }
   
       private void SendSpawned(ushort toClientId)
       {
           NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned)), toClientId);
       }
   
       private Message AddSpawnData(Message message)
       {
           message.AddUShort(Id);
           message.AddString(username);
           message.AddVector3(new Vector3(0,3,0));
           return message;
       }
   
       [MessageHandler((ushort)ClientToServerId.name)]
       private static void Name(ushort fromClientId, Message message)
       {
           Spawn(fromClientId, message.GetString());
       }
   
       #endregion
   } 
}

