using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class Sounds : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider Slider_Main;
    [SerializeField] private Slider Slider_Music;
    [SerializeField] private Slider Slider_Monster;
    [SerializeField] private Slider Slider_Player;
    [SerializeField] private Slider Slider_Effect;
    [SerializeField] private GameObject Main_Menu;
    [SerializeField] private GameObject Options_Menu;

    

    private void Start()
    {
        //AudioListener.pause = true;
       LoadSoundsOptions(); 
    }

    //Загрузка данных звуков
    public void LoadSoundsOptions()
    {
        LoadMainVolume();
        LoadVolumeMusic();
        LoadVolumeMonster();
        LoadPlayerVolume();
        LoadEffectVolume();
    }

    //
    //Установка громкости музыки
    //
    public void SetMusicVolume()
    {
        float volume_music = Slider_Music.value;
        myMixer.SetFloat("music", volume_music);
        PlayerPrefs.SetFloat("musicVolume", volume_music);
    }
    //
    //Установка звуков музыки
    //
    private void LoadVolumeMusic()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            Slider_Music.value = PlayerPrefs.GetFloat("musicVolume");
            SetMusicVolume();
        }

        else
        {
            Slider_Music.value = 50;
            SetMusicVolume();
        }
    }
    //
    //Громкость монстра
    //
    public void SetMonsterVolume()
    {
        float volume_monster = Slider_Monster.value;
        myMixer.SetFloat("Monster", volume_monster);
        PlayerPrefs.SetFloat("MonsterVolume", volume_monster);
    }
    //
    //Установка звуков монстра
    //
    private void LoadVolumeMonster()
    {
        if (PlayerPrefs.HasKey("MonsterVolume"))
        {
            Slider_Monster.value = PlayerPrefs.GetFloat("MonsterVolume");
            SetMonsterVolume();
        }

        else
        {
            Slider_Monster.value = 50;
            SetMonsterVolume();
        }
    }
    //
    //Громкость игрока
    //
    public void SetPlayerVolume()
    {
        float volume_player = Slider_Monster.value;
        myMixer.SetFloat("Player", volume_player);
        PlayerPrefs.SetFloat("PlayerVolume", volume_player);
    } 
    //
    //Установка звуков игрока
    //
    private void LoadPlayerVolume()
    {
        if (PlayerPrefs.HasKey("PlayerVolume"))
        {
            Slider_Player.value = PlayerPrefs.GetFloat("PlayerVolume");
            SetPlayerVolume();
        }
        
        else
        {
            Slider_Player.value = 50;
            SetPlayerVolume();
        }
    }
    //
    //Громкость звуков эффектов
    //
    public void SetEffectVolume()
    {
        float volume_effect = Slider_Effect.value;
        myMixer.SetFloat("Effect", volume_effect);
        PlayerPrefs.SetFloat("EffectVolume", volume_effect);
    } 
    //
    //Установка звуков эффектов
    //
    private void LoadEffectVolume()
    {
        if (PlayerPrefs.HasKey("EffectVolume"))
        {
            Slider_Effect.value = PlayerPrefs.GetFloat("EffectVolume");
            SetEffectVolume();
        }
        
        else
        {
            Slider_Effect.value = 50;
            SetEffectVolume();
        }
    }
    //
    //Громкость звуков основная
    //
    public void SetMainVolume()
    {
        float volume_main = Slider_Main.value;
        myMixer.SetFloat("Master", volume_main);
        PlayerPrefs.SetFloat("MainVolume", volume_main);
    } 
    //
    //Установка главных звуков 
    //
    private void LoadMainVolume()
    {
        if (PlayerPrefs.HasKey("MainVolume"))
        {
            Slider_Main.value = PlayerPrefs.GetFloat("MainVolume");
            SetPlayerVolume();
        }

        else
        {
            Slider_Main.value = 50;
            SetPlayerVolume();
        }
    }

    //
    //Кнопка сохранения
    //
    public void Save_Button()
    {
        SetMainVolume();
        SetMusicVolume();
        SetEffectVolume();
        SetMonsterVolume();
        SetPlayerVolume();

        gameObject.SetActive(false);
        Main_Menu.SetActive(true);
    }

    //
    //Кнопка выхода в паузу
    //
    public void Quit_Button()
    {
        gameObject.SetActive(false);
        Main_Menu.SetActive(true);
    }

    //
    //Кнопка перехода к настройкам
    //
    public void To_Options_Button()
    {
        gameObject.SetActive(false);
        Options_Menu.SetActive(true);
    }

}
