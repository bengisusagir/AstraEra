using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    public float walkSpeed = 4f;
    public float sprintSpeed = 14f;
    public float maxVelocityChange = 10f;
    [Space]
    public float airControl = 0.5f;

    public PhotonView PV;
    public float stepDelay = 0.5f; 
    private float lastStepTime;
    public float jumpDelay = 0.5f; 
    private float lastJumpTime;

    [Space]
    public float jumpHeight = 5f;

    private Vector2 input;
    private Rigidbody rb;
    public Player photonPlayer;
    public bool jumped;

    private bool sprinting;
    private bool jumping;

    public Animator animator;
    public AudioSource walk;



    private bool grounded=false;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!PV.IsMine)
            return;

        input = new Vector2(-Input.GetAxisRaw("Horizontal"), -Input.GetAxisRaw("Vertical"));
        input.Normalize();
        if (input.magnitude > 0 && input.magnitude <= 4f )
        {
            animator.SetBool("isWalking", true);
            if (Time.time - lastStepTime > stepDelay )
            {
                walk.Play();
                lastStepTime = Time.time;
                PV.RPC("PlaySoundsForAll", RpcTarget.All);

            }


        }
        else if (input.magnitude > 4f)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);

        }


        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButtonDown("Jump");
    }
       [PunRPC]
        public void PlaySoundsForAll()
        {

            walk.Play();

        }

    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ground")
            jumped = true;
    }
    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        if (grounded)
        {
                
            if (jumping)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);
 
     
            }
               else if(jumped)
                {
                    
                jumped = false;

            }
            else if (input.magnitude > 0.5f)
            {

            rb.AddForce(CalculateMovement(sprinting ? sprintSpeed :walkSpeed), ForceMode.VelocityChange);
                if (sprinting)
                    stepDelay = 0.2f;
                else
                    stepDelay = 0.3f;
             }
            else
              {

                var velocity1 = rb.linearVelocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
                rb.linearVelocity = velocity1;
                }
        }
        else
        {
            if (input.magnitude > 0.5f)
            {

                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed*airControl : walkSpeed*airControl), ForceMode.VelocityChange);
            }
            else
            {

                var velocity1 = rb.linearVelocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
                rb.linearVelocity = velocity1;
            }
        }
        grounded = false;

    }
     Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= _speed;

        Vector3 velocity = rb.linearVelocity;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);


            velocityChange.y = 0;

            return (velocityChange);
        }
        else
        {
            return new Vector3();
        }
    }
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players[id - 1] = this;

    }
}
