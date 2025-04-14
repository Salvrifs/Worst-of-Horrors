using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shalu_Sounds : MonoBehaviour 
{

    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private AudioClip[] TakeItemSounds;
    [SerializeField] private AudioClip Whistling;
    [SerializeField] private AudioClip Fear;

    private AudioSource audioSource;

    void Start() 
    {
        audioSource = GetComponent<AudioSource>();
    }
    //
    //Шаг
    //
    void PlayStep() 
    {
        audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
    }
    //
    //Взять предмет
    //
    void PlayTakeItem() 
    {
        audioSource.PlayOneShot(TakeItemSounds[Random.Range(0, TakeItemSounds.Length)]);
    }
    //
    //Свист
    //
    void PlayWhistling()
    {
        audioSource.PlayOneShot(Whistling);
    }
    //
    //Страх
    //
    void PlayFear()
    {
        audioSource.PlayOneShot(Fear);
    }
    
}

