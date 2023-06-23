using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotFollower : MonoBehaviour
{
    [SerializeField]private Transform cam_Transform;
    [SerializeField]private Transform model_Transform;

    float pitch, yaw, roll;
    // Start is called before the first frame update
    void Start()
    {
        cam_Transform = GetComponentInChildren<Transform>();
        model_Transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        pitch = model_Transform.localEulerAngles.x;
        yaw = cam_Transform.localEulerAngles.y;
        roll = model_Transform.localEulerAngles.z;

        model_Transform.eulerAngles = new Vector3(pitch, yaw, roll);

        model_Transform.SetPositionAndRotation(model_Transform.position,model_Transform.rotation);
    }
}
