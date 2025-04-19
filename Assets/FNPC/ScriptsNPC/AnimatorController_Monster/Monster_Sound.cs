using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Sound : MonoBehaviour
{
    [SerializeField] private AudioClip Attack;
    [SerializeField] private AudioClip Step;
    [SerializeField] private AudioClip intimidation;
    [SerializeField] private AudioClip[] idle;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    void Strike_Monster()
    {
        audioSource.PlayOneShot(Attack);
    }

    void Step_Monster()
    {
        audioSource.PlayOneShot(Step);
    }

    void Intimidation()
    {
        if (audioSource.isPlaying)
        {
            return;
        }

        else
        {
            audioSource.PlayOneShot(intimidation);
        }
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
