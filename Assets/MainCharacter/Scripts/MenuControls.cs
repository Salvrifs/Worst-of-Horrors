using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject panel;
    public GGCameraMoving cameraController;
    public GameObject infoPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!panel.activeSelf)
            {
                panel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                cameraController.SetControlEnabled(false);
                
            }
            else
            {
                panel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                cameraController.SetControlEnabled(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            infoPanel.SetActive(!infoPanel.activeSelf);
        }
    }

    public void PlayPressed()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ContinuePressed()
    {
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        cameraController.SetControlEnabled(true);
    }

    public void ExitToMainPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}