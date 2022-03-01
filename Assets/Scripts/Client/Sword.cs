using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using Client.Net;

public class Sword : MonoBehaviour
{
    public GameObject sword;

    public Camera cam;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Hit();
        }
    }

    private void Hit()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5))
        {
            StartCoroutine(HitDelay());
        }
    }

    IEnumerator HitDelay()
    {
        yield return new WaitForSeconds(1);
    }
}
