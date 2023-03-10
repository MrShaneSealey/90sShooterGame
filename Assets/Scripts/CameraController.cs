using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private PlayerBindings playerInput;
    private InputAction look;
    private InputAction mouseLock;

    //contex = ctx
    private InputAction.CallbackContext lookContex;

    [Header("Objects")]
    private Rigidbody rb;
    private Camera cam;

    [Header("Camera Control")]
    public float sensX = 35f;
    public float sensY = 35f;

    private Vector3 vRotation;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        playerInput = new PlayerBindings();
    }

    private void OnEnable()
    {
        look = playerInput.Gameplay.Look;
        mouseLock = playerInput.Gameplay.LockMouse;

        //enable
        playerInput.Enable();

        #region subscribe

        //look
        look.performed += onLook;
        #endregion

    }

    private void OnDisable()
    {
        //disable
        playerInput.Disable();

        #region unsubscribe

        //look
        look.performed -= onLook;
        #endregion
    }

    private void onLook(InputAction.CallbackContext context)
    {
        lookContex = context;

        if (mouseLock.triggered)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        vRotation = look.ReadValue<Vector2>();
        //vRotation.Normalize();
        //inverted y and x cause of mouse delta being the wrong way around
        float mouseX = vRotation.y * Time.deltaTime * sensX;
        float mouseY = vRotation.x * Time.deltaTime * sensY;

        transform.Rotate(mouseX, 0, 0);
        cam.transform.Rotate(mouseX, mouseY, 0);
        
        //cam.transform.rotation = Quaternion.Euler(mouseX,rb.transform.rotation.y,0);
        //rb.transform.rotation = Quaternion.Euler(0,mouseY,0);
        

    }

    private void FixedUpdate()
    {
        onLook(lookContex);
    }

}
