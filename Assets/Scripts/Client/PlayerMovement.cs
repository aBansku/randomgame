using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using Client.Net;

namespace Client
{
    public class PlayerMovement : MonoBehaviour
    {
        public float jumpForce;
        public float speed = 3500;
        public float maxSpeed = 25;

        public Rigidbody rb;
        public Camera cam;
        public LayerMask ground;
        public Transform groundCheck;

        public float crouchingSize;
        private bool isGrounded = true;
        private Vector3 size;

        private float counterMovement = 0.175f;
        private float multiplier;
        private float multiplierV;

        private float x, z;
        private float originalMaxSpeed;

        private Vector3 jump;

        private void Start()
        {
            originalMaxSpeed = maxSpeed;
            size = transform.localScale;
            jump = new Vector3(0, jumpForce, 0) * jumpForce;
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, ground);
            //Get the input
            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");
            bool sprint = Input.GetKey(KeyCode.LeftShift);

            //Crouching
            if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.localScale = new Vector3(1, crouchingSize, 1);
            }

            else
            {
                transform.localScale = size;

            }

            //Jumping
            if (Input.GetButton("Jump") && isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce * 1.5f);
                rb.AddForce(Vector3.up * jumpForce * 0.5f);

                Vector3 vel = rb.velocity;
                if (rb.velocity.y < 0.5f)
                    rb.velocity = new Vector3(vel.x, 0, vel.z);
                else if (rb.velocity.y > 0)
                    rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            }
            

            if (!isGrounded && !sprint)
            {
                multiplier = 0.3f;
                multiplierV = 0.3f;
                cam.fieldOfView = 60;
            }
            else if(!isGrounded && sprint)
            {
                multiplier = 0.5f;
                multiplierV = 0.5f;
                cam.fieldOfView = 65;
            }
            else if (isGrounded && sprint)
            {
                multiplier = 1.5f;
                multiplierV = 1.5f;
                cam.fieldOfView = 65;
            }
            else if (isGrounded && !sprint)
            {
                multiplier = 1;
                multiplierV = 1;
                cam.fieldOfView = 60;
            }
            
            Debug.Log(multiplier);
            Debug.Log(multiplierV);

            
            Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.position);
            message.AddUShort(NetworkManager.Singleton.Client.Id);
            message.AddVector3(transform.position);
            message.AddQuaternion(transform.rotation);
            message.AddVector3(transform.localScale);
            NetworkManager.Singleton.Client.Send(message);
        }

        public Vector2 FindVelRelativeToLook()
        {
            float lookAngle = transform.eulerAngles.y;
            float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

            float u = Mathf.DeltaAngle(lookAngle, moveAngle);
            float v = 90 - u;

            float magnitue = rb.velocity.magnitude;
            float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
            float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

            return new Vector2(xMag, yMag);
        }

        private void CounterMovement(Vector2 mag)
        {
            if (!isGrounded) return;

            if (Mathf.Abs(mag.x) > 0.01f && Mathf.Abs(x) < 0.05f || (mag.x < -0.01f && x > 0) || (mag.x > 0.01f && x < 0))
            {
                rb.AddForce(transform.right * speed * Time.deltaTime * -mag.x * counterMovement);
            }

            if (Mathf.Abs(mag.y) > 0.01f && Mathf.Abs(z) < 0.05f || (mag.y < -0.01f && z > 0) || (mag.y > 0.01f && z < 0))
            {
                rb.AddForce(transform.forward * speed * Time.deltaTime * -mag.y * counterMovement);
            }

            if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
            {
                float fallspeed = rb.velocity.y;
                Vector3 n = rb.velocity.normalized * maxSpeed;
                rb.velocity = new Vector3(n.x, fallspeed, n.z);
            }
        }

        private void FixedUpdate()
        {
            if (rb.velocity.magnitude > maxSpeed) return;

            Vector2 mag = FindVelRelativeToLook();

            //Stop the player
            if (x == 0f || z == 0f)
            {
                CounterMovement(mag);
            }

            //Extra gravity
            rb.AddForce(Vector3.down);

            //Actual movement
            rb.AddForce(transform.forward * z * speed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(transform.right * x * speed * Time.deltaTime * multiplier);

        }
        
        [MessageHandler((ushort)ServerToClientId.playerPosition)]
        private static void ReceivePositions(Message message)
        {
            foreach (Player player in Player.list.Values)
            {
                if (!player.IsLocal && player.Id == message.GetUShort())
                {
                    player.transform.position = message.GetVector3();
                    player.transform.rotation = message.GetQuaternion();
                    player.transform.localScale = message.GetVector3();
                }
            }
        }
    }
}

