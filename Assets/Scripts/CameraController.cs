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
    [SerializeField]private Rigidbody rb;
    [SerializeField]private Camera cam;

    [Header("Camera Control")]
    public float sensX = 5f;
    public float sensY = 5f;

    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float lerpTime = 0.01f;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        playerInput = new PlayerBindings();
    }


    void OnDisable() {

        playerInput.Disable();

        #region unsubscribe

        //look
        look.performed -= onLook;
        #endregion

        Cursor.lockState = CursorLockMode.None;
    }

    void OnEnable()
    {
        look = playerInput.Gameplay.Look;
        mouseLock = playerInput.Gameplay.LockMouse;

        //enable
        playerInput.Enable();

        #region subscribe

        //look
        look.performed += onLook;
        #endregion

        //targetRotation.InitializeFromTransform(transform);
        //currentRotation.InitializeFromTransform(transform);

        // This makes the cursor hidden -- hit Escape to get your cursor back so you can exit play mode
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void onLook(InputAction.CallbackContext context)
    {
        Vector3 mouseInput = look.ReadValue<Vector2>();

        float x = mouseInput.x * sensX * Time.deltaTime;
        float y = mouseInput.y * sensY * Time.deltaTime;
        float z = 0;

        cam.transform.rotation = Quaternion.Euler(x, y, z);
        rb.transform.rotation = Quaternion.Euler(0, y, 0);

        //i need push
    }

    void FixedUpdate()
    {
        onLook(lookContex);
    }
}
