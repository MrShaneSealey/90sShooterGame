using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    #region Controllers & Contexts
    private PlayerBindings playerInput;
    private InputAction movement;
    private InputAction dash;
    private InputAction jump;
    //contex = ctx
    private InputAction.CallbackContext movementContex;
    private InputAction.CallbackContext jumpContex;
    private InputAction.CallbackContext dashContex;

    //controllers
    private Rigidbody rb;
    #endregion

    #region All Varibles
    [Header("Speed")]
    private Vector3 playerVelocity;
    public float speed = 100f;

    [Header("Dash")]
    public float dashSpeed = 50f;
    public bool dashOn = true;
    public float dashTime = 20f;
    public float maxDashTime = 20f;

    [Header("Jump")]
    public float jumpHeight = 10f;
    private float distToGround;
    //public bool isGrounded;
    [SerializeField]private Collider collider_;
    //[SerializeField] private LayerMask layerMask_;
    #endregion

    private void Awake()
    {
        playerInput = new PlayerBindings();
        rb = GetComponent<Rigidbody>();
        collider_ = GetComponentInChildren<Collider>();

        distToGround = collider_.bounds.extents.y;
    }

    private void OnEnable()
    {
        movement = playerInput.Gameplay.Move;
        dash = playerInput.Gameplay.Dash;
        jump = playerInput.Gameplay.Jump;

        //enable
        playerInput.Enable();

        #region subscribe
        //movement
        movement.performed += onMove;
        dash.performed += onDash;
        jump.performed += onJump;
        #endregion

    }

    private void OnDisable()
    {
        //disable
        playerInput.Disable();

        #region unsubscribe
        //movement
        movement.performed -= onMove;
        dash.performed -= onDash;
        jump.performed -= onJump;

        #endregion
    }

    private void onMove(InputAction.CallbackContext context)
    {
        movementContex = context;

        playerVelocity = Vector3.zero;
        playerVelocity = movement.ReadValue<Vector3>();
        playerVelocity.Normalize();

        rb.AddRelativeForce(playerVelocity * speed * Time.deltaTime);

    }

    private void onDash(InputAction.CallbackContext context)
    {
        dashContex = context;
        //zaza

        if (dash.triggered && dashOn)
        {
            rb.AddForce(Vector3.forward * dashSpeed * Time.deltaTime, ForceMode.Impulse);
            dashTime = 0;
            dashOn = false;
        }

        //CDR
        if (dashTime < maxDashTime && !dashOn)
        {
            //wait a few seconds then renable
            dashTime += Time.deltaTime;
        }
        else
        {
            dashOn = true;
        }


    }
    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f);
    }

    private void onJump(InputAction.CallbackContext context)
    {
        jumpContex = context;

        if(jump.triggered && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpHeight * Time.deltaTime, ForceMode.Impulse);
        }
        
    }


    private void FixedUpdate()
    {
        onMove(movementContex);
        onJump(jumpContex);
        onDash(dashContex);
    }
}
