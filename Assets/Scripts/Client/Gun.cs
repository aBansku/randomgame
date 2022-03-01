using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using Client.Net;

namespace Client
{
    public class Gun : MonoBehaviour
    {
        public ParticleSystem muzzleFlash;
        public Camera cam;

        // Update is called once per frame


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                muzzleFlash.Play();
                Shoot();
            }
        }

        private void Shoot()
        {
            Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.shoot);
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 152)) 
            {
                
                StartCoroutine(Reload());
            }
        }

        IEnumerator Reload()
        {
            yield return new WaitForSeconds(15);
        }

    }
}

