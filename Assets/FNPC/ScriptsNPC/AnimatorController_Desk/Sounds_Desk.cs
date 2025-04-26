using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds_Desk : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] Steps;
    [SerializeField] AudioClip[] fright;
    [SerializeField] AudioClip[] escapes;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Play_StepDesk()
    {
        audioSource.PlayOneShot(Steps[Random.Range(0, Steps.Length)]);
    }

    void Play_fright()
    {
        audioSource.PlayOneShot(fright[Random.Range(0, fright.Length)]);
    }

    void play_escape()
    {
        audioSource.PlayOneShot(escapes[Random.Range(0, escapes.Length)]);
    }


}
