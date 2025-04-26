using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider Slider_Main;
    [SerializeField] private Slider Slider_Music;
    [SerializeField] private Slider Slider_Monster;
    [SerializeField] private Slider Slider_Player;
    [SerializeField] private Slider Slider_Effect;
    [SerializeField] private GameObject Main_Menu;
    [SerializeField] private GameObject Options_Menu;

    private Coroutine _musicFadeCoroutine;

    private void Start()
    {
       LoadSoundsOptions(); 
    }

    public void ToggleMusic(bool play, float fadeDuration = 1f)
    {
        float targetVolume = play ? PlayerPrefs.GetFloat("musicVolume", 50) : 0;
        StartMusicFade(targetVolume, fadeDuration);
    }

    //  
    //Корутина
    //
    private IEnumerator FadeMusic(float startVolume, float targetVolume, float duration)
    {
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            myMixer.SetFloat("music", ConvertToDecibel(volume));
            yield return null;
        }
    }
    //
    //Начинает затухание
    //
    public void StartMusicFade(float targetVolume, float duration)
    {
        if (_musicFadeCoroutine != null)
        {
            StopCoroutine(_musicFadeCoroutine);
        }
        _musicFadeCoroutine = StartCoroutine(FadeMusic(Slider_Music.value, targetVolume, duration));
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
    private float ConvertToDecibel(float value)
    {
        return Mathf.Clamp(20f * Mathf.Log10(value / 100f), -80f, 0f);
    }
    //
    //Установка громкости музыки
    //
    public void SetMusicVolume()
    {
        float volume_music = Slider_Music.value;
        myMixer.SetFloat("music", ConvertToDecibel(volume_music));
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
        myMixer.SetFloat("Monster", ConvertToDecibel(volume_monster));
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
        float volume_player = Slider_Player.value;
        myMixer.SetFloat("Player", ConvertToDecibel(volume_player));
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
        myMixer.SetFloat("Effect", ConvertToDecibel(volume_effect));
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
        myMixer.SetFloat("Master", ConvertToDecibel(volume_main));
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
            SetMainVolume();
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

    public void MuteAll(bool mute)
{
    myMixer.SetFloat("Master", mute ? -80f : ConvertToDecibel(Slider_Main.value));
}

}