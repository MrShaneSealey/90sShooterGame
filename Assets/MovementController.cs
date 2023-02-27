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
    private PlayerBindings playerInput;
    private InputAction movement;
    //contex = ctx
    private InputAction.CallbackContext movementContex;

    [Header("Varibles")]
    private Rigidbody rb;
    private Vector3 vMovement;
    private Camera cam;

    [Header("Speed Control")]
    [SerializeField] private float speed = 100f;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        playerInput = new PlayerBindings();
    }

    private void OnEnable()
    {
        movement = playerInput.Gameplay.Move;

        //enable
        playerInput.Enable();

        #region subscribe
        //movement
        movement.performed += onMove;
        #endregion

    }

    private void OnDisable()
    {
        //disable
        playerInput.Disable();

        #region unsubscribe
        //movement
        movement.performed -= onMove;

        #endregion
    }

    private void onMove(InputAction.CallbackContext context)
    {
        movementContex = context;
        vMovement = movement.ReadValue<Vector3>();
        vMovement.Normalize();

        rb.AddForce(vMovement * speed * Time.deltaTime);

    }

    private void FixedUpdate()
    {
        onMove(movementContex);
    }

}
