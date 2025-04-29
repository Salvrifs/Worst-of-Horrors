using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioClip[] Musics; 

    void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    else
    {
        Destroy(gameObject); // Уничтожаем дубликаты
    }
}

   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene") PlayMenuMusic();
        else StopAllMusic();
    }

    public void PlayMenuMusic()
{
    gameMusic.Stop();
    menuMusic.clip = Musics[Random.Range(0, Musics.Length)];
    menuMusic.Play(); 
}

    public void PlayGameMusic()
    {
        menuMusic.Stop();
        gameMusic.PlayOneShot(Musics[Random.Range(0, Musics.Length)]);
    }

    public void StopAllMusic()
    {
        menuMusic.Stop();
        gameMusic.Stop();
    }

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("Master", ConvertToDecibel(value));
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("music", ConvertToDecibel(value));
    }
    public void SetMonsterVolume(float value)
    {
        mixer.SetFloat("Monster", ConvertToDecibel(value));
    }
    public void SetEffectVolume(float value)
    {
        mixer.SetFloat("Effect", ConvertToDecibel(value));
    }
    public void SetPlayerVolume(float value)
    {
        mixer.SetFloat("Player", ConvertToDecibel(value));
    }

    private float ConvertToDecibel(float value)
    {
        return Mathf.Clamp(20f * Mathf.Log10(value / 100f), -80f, 0f);
    }
}