using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Sound : MonoBehaviour
{
    [SerializeField] private AudioClip Attack;
    [SerializeField] private AudioClip Step;
    [SerializeField] private AudioClip[] intimidation;
    [SerializeField] private AudioClip Music_Chase;
    [SerializeField] private AudioClip[] intimidateChaseSound;
    [SerializeField] private AudioClip[] idle;
    [SerializeField] private AudioSource footStepSource;
    [SerializeField] private AudioSource IntimidateSource;
   [SerializeField] private AudioSource musicSource;
    private Coroutine musicFadeCoroutine;

    void Awake()
    {
        musicSource = GameObject.Find("Music_Chase").GetComponent<AudioSource>();
    }

    void Strike_Monster()
    {
        footStepSource.PlayOneShot(Attack);
    }

    void Step_Monster()
    {
        if(!footStepSource.isPlaying)
        {
            footStepSource.pitch = Random.Range(0.9f, 1.1f);
            footStepSource.PlayOneShot(Step);
        }
    }

    public IEnumerator PlayIntimidation()
    {
       IntimidateSource.PlayOneShot(intimidation[Random.Range(0, intimidation.Length)]);
       yield return new WaitForSeconds(1.5f); 
    }

    void Idle() 
    {
        if (IntimidateSource.isPlaying)
        {
            return;
        }
        IntimidateSource.PlayOneShot(idle[Random.Range(0, idle.Length)]);
    }

    


    private IEnumerator PlayChase_Sound()
    {
        Debug.Log("Intimidate");
        IntimidateSource.clip = intimidateChaseSound[Random.Range(0, intimidateChaseSound.Length)];
        IntimidateSource.Play();
        yield return new WaitForSeconds(2.5f);
    }

    public void PlayChaseMusic(bool instant = false)
{
    if(musicSource == null)
{
    Debug.LogError("Music Source не назначен!");
    return;
}

if(!musicSource.gameObject.activeSelf)
    musicSource.gameObject.SetActive(true);

    if(musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
    
    
    musicSource.clip = Music_Chase;
    musicSource.loop = true;

    if(instant)
    {
        musicSource.volume = 1f;
        musicSource.Play();
        return;
    }

    musicFadeCoroutine = StartCoroutine(FadeAudio(musicSource, 3f, 0.7f));
}

    public void StopChaseMusic(bool instant = false)
{
    if(musicSource == null) return;

    if(musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
    
    if(instant)
    {
        StartCoroutine(FadeAudio(musicSource, 0.5f, 0f, true));
        return;
    }

    musicFadeCoroutine = StartCoroutine(FadeAudio(musicSource, 2f, 0f, true));
}

private IEnumerator FadeAudio(AudioSource source, float duration, float targetVolume, bool stopAfter = false)
{
    // Добавляем проверку на активность источника
    if(source == null || !source.gameObject.activeInHierarchy) yield break;
    
    float startVolume = source.volume;
    float timer = 0f;

    // Включаем источник если выключен
    if(!source.gameObject.activeSelf)
        source.gameObject.SetActive(true);

    if(!source.isPlaying)
        source.Play();

    while(timer < duration)
    {
        // Дополнительная проверка на уничтожение объекта
        if(source == null) 
            yield break;
        
        source.volume = Mathf.Lerp(startVolume, targetVolume, timer/duration);
        timer += Time.deltaTime;
        yield return null;
    }

    if(source != null)
    {
        source.volume = targetVolume;
        if(stopAfter) 
        {
            source.Stop();
            
        }
    }
}


    private IEnumerator FadeMusic(float duration, float targetVolume)
    {
        float startVolume = musicSource.volume;
        musicSource.clip = Music_Chase;
        musicSource.loop = true;
        musicSource.Play();
    
        float timer = 0f;
        while(timer < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, timer/duration);
            timer += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = targetVolume;
    }

    public void StopChaseMusic()
    {
        musicSource.Stop();
    }

    public Coroutine StartChaseIntimidate()
    {
        return StartCoroutine(ChaseIntimidateRoutine());
    }

    public void StopChaseIntimidate(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }

    private IEnumerator ChaseIntimidateRoutine()
    {
        while(true)
        {
            PlayChase_Sound();
            yield return new WaitForSeconds(Random.Range(4f, 7f));
        }
    }

}
