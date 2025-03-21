using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private GameObject Settings_Menu;
    [SerializeField] private GameObject MainMenu;

    Resolution[] resolutions;

    void Start()
    {
        resolutionsDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; ++i)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRateRatio + "Hz";
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
            options.Add(option);
        }
        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    //
    //установка разрешения
    //
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    //
    //Установка качества графики
    //
    public void Setquality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }   
    //
    //Полноэкранный режим
    //
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    //
    //выход из меню настроек
    //
    public void QuitButton()
    {
        Settings_Menu.SetActive(false);
        MainMenu.SetActive(true);
    } 
    //
    //Сохранение настроек
    //
    public void SaveSettingsButton()
    {
        PlayerPrefs.SetInt("QualitySettingsPref", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionSettingsPref", resolutionsDropdown.value);
        PlayerPrefs.SetInt("FullScreenSettingsPref", Convert.ToInt32(Screen.fullScreen));
        Settings_Menu.SetActive(false);
        MainMenu.SetActive(true);
    }
    //
    //Применение/загрузка настроек
    //
    private void LoadSettings(int ResolutionIndex)
    {
        //Качество
        if (PlayerPrefs.HasKey("QualitySettingsPref"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingsPref");
        }

        else 
        {
            qualityDropdown.value = 3;
        }

        //Разрешение
        if (PlayerPrefs.HasKey("ResolutionSettingsPref"))
        {
            resolutionsDropdown.value = PlayerPrefs.GetInt("ResolutionSettingsPref");
        }
        else
        {
            resolutionsDropdown.value = ResolutionIndex;
        }

        //Полноэкранный режим
        if (PlayerPrefs.HasKey("FullScreenSettingsPref"))
        {
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenSettingsPref"));
        }
        else 
        {
            Screen.fullScreen = true;
        }


    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
