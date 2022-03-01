using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using System;
using Client;

namespace Client.Net
{
    public enum ServerToClientId : ushort
    {
        playerSpawned = 1,
        playerPosition = 2,
        crouch = 3,
        weapon = 4,
    }

    public enum ClientToServerId : ushort
    {
        name = 1,
        position = 2,
        weapon = 3,
        shoot = 4,
    }

    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _singleton;

        public static NetworkManager Singleton
        {
            get => _singleton;
            set
            {
                if (_singleton == null)
                    _singleton = value;
                else if (_singleton != value)
                {
                    Debug.Log($"{nameof(NetworkManager)} Instance already exists, destroying duplicate!");
                    Destroy(value);
                }

            }
        }

        [SerializeField] private string ip;
        [SerializeField] private ushort port;

        public RiptideNetworking.Client Client { get; private set; }

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

            Client = new RiptideNetworking.Client();
            Client.Connected += DidConnect;
            Client.ConnectionFailed += FailedToConnect;
            Client.ClientDisconnected += PlayerLeft;
            Client.Disconnected += DidDisconnect;
        }

        private void FixedUpdate()
        {
            Client.Tick();
        }

        private void OnApplicationQuit()
        {
            Client.Disconnect();
        }

        public void Connect()
        {
            Client.Connect($"{ip}:{port}");
        }

        private void DidConnect(object sender, EventArgs e)
        {
            UIManager.Singleton.SendName();
        }

        private void FailedToConnect(object sender, EventArgs e)
        {
            UIManager.Singleton.BackToMain();
        }

        private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            Destroy(Player.list[e.Id].gameObject);
        }

        private void DidDisconnect(object sender, EventArgs e)
        {
            UIManager.Singleton.BackToMain();
        }
    }

}
