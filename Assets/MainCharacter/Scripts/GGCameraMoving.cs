using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GGCameraMoving : MonoBehaviour
{
    public float mouse_sens = 5f;
    public float max_angle_Y = 90.0f;

    private float rotationX = 0.0f;

    public GameObject panel;
    public GameObject infoPanel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!panel.activeSelf)
            {
                panel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
            }
            else
            {
                panel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            infoPanel.SetActive(!infoPanel.activeSelf);
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.parent.Rotate(Vector3.up * mouseX * mouse_sens);

        rotationX -= mouseY * mouse_sens;
        rotationX = Mathf.Clamp(rotationX, -max_angle_Y, max_angle_Y);
        transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
    }
}
