using System;
using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

namespace Server.Net
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
    
        public RiptideNetworking.Server Server { get; private set; }
    
        [SerializeField] private ushort port;
        [SerializeField] private ushort maxClientCount;
    
        private void Awake()
        {
            Singleton = this;
        }
    
        public void StartServer()
        {
            Server.Start(port, maxClientCount);
            Server.ClientDisconnected += PlayerLeft;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
    
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
    
            Server = new RiptideNetworking.Server();
        }

        private void FixedUpdate()
        {
            Server.Tick();
        }
    
        private void OnApplicationQuit()
        {
            Server.Stop();
        }
    
        private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            Destroy(Player.list[e.Id].gameObject);
        }
    }
}


