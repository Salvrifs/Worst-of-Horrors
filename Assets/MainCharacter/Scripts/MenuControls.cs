using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] GameObject MainMenu;
    public GameObject Menu;
    public GGCameraMoving cameraController;
    public GameObject infoPanel;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject SoundPanel;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!Menu.activeSelf && !SettingsPanel.activeSelf && !SoundPanel.activeSelf)
                {
                    MenuActivate(!Menu.activeSelf);
                    AudioListener.pause = true;
                }

                else if (!Menu.activeSelf && SettingsPanel.activeSelf && !SoundPanel.activeSelf)
                {
                    SettingsPanel.SetActive(false);
                    MenuActivate(!Menu.activeSelf);
                    AudioListener.pause = true;
                }

                else if (!Menu.activeSelf && !SettingsPanel.activeSelf && SoundPanel.activeSelf)
                {
                    SoundPanel.SetActive(false);
                    MenuActivate(!Menu.activeSelf);
                    AudioListener.pause = true;
                }

                else 
                {
                    Menu.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Time.timeScale = 1f;
                    cameraController.SetControlEnabled(true);
                    AudioListener.pause = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            infoPanel.SetActive(!infoPanel.activeSelf);
        }
    }

    //
    //Управление меню
    //
    private void MenuActivate(bool activeOrNot)
    {
        Menu.SetActive(activeOrNot);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        cameraController.SetControlEnabled(false);
    }

    //
    //Нажать кнопку "Играть" в главном меню
    //
    public void PlayPressed()
    {
        Menu.SetActive(false);
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