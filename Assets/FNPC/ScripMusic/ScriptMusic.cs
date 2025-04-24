using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMusic : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] MenuMusics;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(MenuMusics[Random.Range(0, MenuMusics.Length)]);
        audioSource.PlayOneShot(MenuMusics[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(MenuMusics[Random.Range(0, MenuMusics.Length)]);
        }
    }
}
