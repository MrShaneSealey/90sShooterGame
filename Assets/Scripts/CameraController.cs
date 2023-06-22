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
    public float sensX = 5f;
    public float sensY = 5f;

    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float lerpTime = 0.01f;
 

    private Vector2 vRotation;
    private float vKeyRotaion;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        playerInput = new PlayerBindings();
    }

    class CameraRotation
    {
        public float yaw, pitch, roll;

        public void InitializeFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
        }

        public void LerpTowards(CameraRotation target, float rotationLerpPct)
        {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
        }
    }

    CameraRotation targetRotation = new CameraRotation();
    CameraRotation currentRotation = new CameraRotation();


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


        targetRotation.InitializeFromTransform(transform);
        currentRotation.InitializeFromTransform(transform);

        // This makes the cursor hidden -- hit Escape to get your cursor back so you can exit play mode
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void onLook(InputAction.CallbackContext context)
    {
        vRotation = look.ReadValue<Vector2>();

        // Update the target rotation based on mouse input
        var mouseInput = new Vector2(vRotation.x, vRotation.y * -1);
        targetRotation.yaw += mouseInput.x * sensX;
        targetRotation.pitch += mouseInput.y * sensY;

        // Don't allow the camera to flip upside down
        targetRotation.pitch = Mathf.Clamp(targetRotation.pitch, -90, 90);

        // Calculate the new rotation using framerate-independent interpolation
        var lerpPct = 1f - Mathf.Exp(Mathf.Log(0.01f) / lerpTime * Time.deltaTime);
        currentRotation.LerpTowards(targetRotation, lerpPct);

        // Commit the rotation changes to the transform
        currentRotation.UpdateTransform(transform);
    }

    void FixedUpdate()
    {
        onLook(lookContex);
    }
}
