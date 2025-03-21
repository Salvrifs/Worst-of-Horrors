using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject Menu;
    public GGCameraMoving cameraController;
    public GameObject infoPanel;
    [SerializeField] private GameObject SettingsPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Menu.activeSelf)
            {
                Menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                cameraController.SetControlEnabled(false);
                
            }
            else
            {
                Menu.SetActive(false);
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
    //
    //Нажать кнопку "Играть" в главном меню
    //
    public void PlayPressed()
    {
        SceneManager.LoadScene("SampleScene");
    }
    //
    //нажать кпопку "Продолжить" в esc
    //
    public void ContinuePressed()
    {
        Menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        cameraController.SetControlEnabled(true);
    }
    //
    //Нажать кпопку "Настройки" в esc
    //
    public void SettingsPressed()
    {
        SettingsPanel.SetActive(true);
        Menu.SetActive(false);
    }
    //
    //Нажать кнопку "Выход" в esc
    //
    public void ExitToMainPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
    //
    //Нажать кнопку "Выход" в главном меню
    //
    public void ExitPressed()
    {
        Application.Quit();
    }
}