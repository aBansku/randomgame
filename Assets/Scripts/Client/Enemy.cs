using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace Client
{
    public class Enemy : MonoBehaviour
    {
    
        public float health = 100f;
    
        public GameObject enemy;
        public GameObject ground;
        public Transform player;

        public bool isActive;

        public bool isInRange;
    
        public void TakeDamage(float amount)
        {
            health -= amount;
            if (health <= 0f)
            {
                Die();
            }
        }
    
        private void Die()
        {
            Destroy(gameObject);
    
        }
    
        private void Respawn()
        {
            Vector3 pos = new Vector3(Random.Range(ground.transform.position.x + 50, ground.transform.position.x - 50), 2.67f, Random.Range(ground.transform.position.z + 35, ground.transform.position.z - 50));
            Instantiate(enemy, pos, Quaternion.identity);
        }
    
        #region Shooting

        private void Start()
        {
            StartCoroutine(FindPlayer(1.5f));
        }
    
        private void Attack()
        {
            RaycastHit hit;
    
    
            if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
            {
                Debug.Log(hit.transform.name + "dsadsa");
                
            }
        }
        
        private IEnumerator FindPlayer(float time)
        {
            while (isActive)
            {
                transform.LookAt(player);
                if (!isInRange)
                {
                    transform.position += transform.forward * 21;
                }
                else
                {
                    Attack();
                }
                yield return new WaitForSeconds(time);
    
            }
        }
        #endregion
    }

}
