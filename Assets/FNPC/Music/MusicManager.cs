using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] public AudioClip[] menuMusic;
    private AudioSource audioSource;

    void Awake()
    {
        // Реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        PlayMusic(menuMusic[Random.Range(0, menuMusic.Length)]);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip && audioSource.isPlaying) return;
        
        audioSource.clip = clip;
        audioSource.Play();
    }
}