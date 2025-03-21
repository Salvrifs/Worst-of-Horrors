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
    

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolumeMusic();
            LoadVolumeMonster();
            LoadPlayerVolume();
            LoadEffectVolume();
            LoadMainVolume();
        }

        else
        {
            SetMusicVolume();
            SetMonsterVolume();
            SetPlayerVolume();
            SetEffectVolume();
            SetMainVolume();
        } 
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
        Slider_Music.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
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
        Slider_Monster.value = PlayerPrefs.GetFloat("MonsterVolume");
        SetMonsterVolume();
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
        Slider_Player.value = PlayerPrefs.GetFloat("PlayerVolume");
        SetPlayerVolume();
    }
    //
    //Громкость звуков эффектов
    //
    public void SetEffectVolume()
    {
        float volume_effect = Slider_Monster.value;
        myMixer.SetFloat("Effect", volume_effect);
        PlayerPrefs.SetFloat("EffectVolume", volume_effect);
    } 
    //
    //Установка звуков эффектов
    //
    private void LoadEffectVolume()
    {
        Slider_Main.value = PlayerPrefs.GetFloat("EffectVolume");
        SetEffectVolume();
    }
    //
    //Громкость звуков основная
    //
    public void SetMainVolume()
    {
        float volume_player = Slider_Monster.value;
        myMixer.SetFloat("Main", volume_player);
        PlayerPrefs.SetFloat("MainVolume", volume_player);
    } 
    //
    //Установка главных звуков 
    //
    private void LoadMainVolume()
    {
        Slider_Player.value = PlayerPrefs.GetFloat("MainVolume");
        SetPlayerVolume();
    }
}
