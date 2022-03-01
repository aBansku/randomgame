using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public class EnemyActivator : MonoBehaviour
    {
        public GameObject enemy;

        private Enemy e;
        
        private void Start()
        {
            e = enemy.GetComponent<Enemy>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<Player>())
            {
                e.isActive = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            e.isActive = false;
        }
    }
}

