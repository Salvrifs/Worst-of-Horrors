using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GGCameraMoving : MonoBehaviour
{
    public float mouse_sens = 5f;
    public float max_angle_Y = 90.0f;

    private float rotationX = 0.0f;

    private bool isControlEnabled = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isControlEnabled)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.parent.Rotate(Vector3.up * mouseX * mouse_sens);

        rotationX -= mouseY * mouse_sens;
        rotationX = Mathf.Clamp(rotationX, -max_angle_Y, max_angle_Y);
        transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
    }

    public void SetControlEnabled(bool isEnabled)
    {
        isControlEnabled = isEnabled;
    }
}
