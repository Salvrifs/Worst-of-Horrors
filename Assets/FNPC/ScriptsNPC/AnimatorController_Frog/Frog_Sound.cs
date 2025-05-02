using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog_Sound : MonoBehaviour
{
    [SerializeField] private AudioClip Jump;
    [SerializeField] private AudioClip idle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] float chanceToQuack = 0.5f;

    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    void Strike_Frog()
    {
        audioSource.PlayOneShot(Jump);
    }

    void Jump_Frog()
    {
        audioSource.PlayOneShot(Jump);
    }

    //void Intimidation()
    //{
    //    if (audioSource.isPlaying)
    //    {
    //        return;
    //    }

    //    else
    //    {
    //        audioSource.PlayOneShot(intimidation);
    //    }
    //}

    void Idle() 
    {
        if (audioSource.isPlaying)
        {
            return;
        }

        if (Random.value < chanceToQuack)
        {
            audioSource.PlayOneShot(idle);
        }
    }

}
