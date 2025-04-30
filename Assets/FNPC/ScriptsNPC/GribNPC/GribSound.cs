using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GribSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] Step;
    [SerializeField] private AudioClip[] intimidation;
    [SerializeField] private AudioClip[] idle;
    [SerializeField] private AudioClip Slap;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }
    
    

    void Step_Grib()
    {
        if (audioSource.isPlaying)
        {
            return;
        }
        audioSource.PlayOneShot(Step[Random.Range(0, idle.Length)]);
    }

    void Slap_Grib()
    {
        audioSource.PlayOneShot(Slap);
    }

    void Intimidation()
    {
        if (audioSource.isPlaying)
        {
            return;
        }
        audioSource.PlayOneShot(intimidation[Random.Range(0, idle.Length)]);
    }

    void Idle() 
    {
        if (audioSource.isPlaying)
        {
            return;
        }
        audioSource.PlayOneShot(idle[Random.Range(0, idle.Length)]);
    }

}
