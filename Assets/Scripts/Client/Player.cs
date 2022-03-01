using System;
using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;
using Client.Net;

namespace Client
{
    public class Player : MonoBehaviour
    {
        public float currentWeapon = 1f;

        public GameObject sword;
        public GameObject musket;
        
        public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

        public ushort Id { get; private set; }
        public bool IsLocal { get; private set; }

        private string username;

        private void OnDestroy()
        {
            list.Remove(Id);
        }

        private void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentWeapon = 1f;
                sword.SetActive(true);
                musket.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentWeapon = 2f;
                musket.SetActive(true);
                sword.SetActive(false);
            }

            Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.weapon);
            message.AddUShort(NetworkManager.Singleton.Client.Id);
            message.AddFloat(currentWeapon);
            NetworkManager.Singleton.Client.Send(message);
        }

        public static void Spawn(ushort id, string username, Vector3 position)
        {
            Player player;
            if (id == NetworkManager.Singleton.Client.Id)
            {
                player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
                player.IsLocal = true;
            }
            else
            {
                player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
                player.IsLocal = false;
            }

            player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
            player.Id = id;

            player.username = username;

            list.Add(id, player);
        }

        [MessageHandler((ushort)ServerToClientId.playerSpawned)]
        private static void SpawnPlayer(Message message)
        {
            Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
        }

    }

}
