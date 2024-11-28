using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject panel;

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