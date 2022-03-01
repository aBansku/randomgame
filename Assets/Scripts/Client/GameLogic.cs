using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class GameLogic : MonoBehaviour
    {
        private static GameLogic _singleton;

        public static GameLogic Singleton
        {
            get => _singleton;
            set
            {
                if (_singleton == null)
                    _singleton = value;
                else if (_singleton != value)
                {
                    Debug.Log($"{nameof(GameLogic)} Instance already exists, destroying duplicate!");
                    Destroy(value);
                }

            }
        }

        public GameObject LocalPlayerPrefab => localPlayerPrefab;
        public GameObject PlayerPrefab => playerPrefab;

        [Header("Prefabs")]
        [SerializeField] private GameObject localPlayerPrefab;
        [SerializeField] private GameObject playerPrefab;

        private void Awake()
        {
            Singleton = this;
        }
    }
}

