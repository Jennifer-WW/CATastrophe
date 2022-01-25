using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class CatController2 : MonoBehaviour
{
    //cat integration (lol)
    private GameObject catBody;

    //cam integration
    private Transform cam;

    //animation
    private Animator anim;
    public float animWalkTune = 10f;

    //locomotion
    private CharacterController controller;
    public float walkSpeed = 5f;
    public float moveDeadzone = 0.1f;
    public float turnSmoothTime = 0.1f;
    private float smoothVelocity;
    private float curspeed;
    private Vector3 thisPos;
    private Vector3 lastPos;

    //imported from Limina
    public LayerMask groundMask;
    public Transform groundCheck;
    public float gravity = -9.81f;
    Vector3 velocity;
    public float groundDistance = 0f;
    private bool isGrounded;
    public float jumpHeight = 1;

    //pickup system
    private PickupManager pm;

    void OnDrawGizmos()
    {

        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

    void Start()
    {
        //clean but annoying to change, may revisit
        cam = gameObject.transform.Find("Main Camera");
        controller = GetComponent<CharacterController>();
        curspeed = 0;
        lastPos = gameObject.transform.position;
        catBody = gameObject.transform.Find("cat").gameObject;
        pm = catBody.GetComponent<PickupManager>();
        anim = catBody.GetComponent<Animator>();

    }
    void Update()
    {
        //gather input
        bool interact = Input.GetButtonDown("Interact");
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //get cur pos and calculate speed this frame (useful for animations)
        thisPos = gameObject.transform.position;
        curspeed = Vector3.Distance(new Vector3(thisPos.x, 0, thisPos.z), new Vector3(lastPos.x, 0, lastPos.z));

        //get input and move
        Vector3 direction = new Vector3(h, 0, v);

        //handle gravity / jumping
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jumped");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (direction.magnitude > moveDeadzone)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(Time.deltaTime * walkSpeed * moveDir.normalized);
        }

        //handle interact
        if (interact)
        {
            Debug.Log("Try Interact");
            pm.Interact();
        }
        //manage animator state
        anim.SetFloat("speed", curspeed * animWalkTune);

        //housekeeping
        lastPos = thisPos;
    }
}