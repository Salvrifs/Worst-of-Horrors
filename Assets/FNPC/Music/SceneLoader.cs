// В скрипте, который отвечает за загрузку сцен
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        MusicManager.Instance.PlayMusic(null); // Остановить музыку
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
    void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if (scene.name == "MenuScene")
    {
        MusicManager.Instance.PlayMusic(MusicManager.Instance.menuMusic[Random.Range(0, MusicManager.Instance.menuMusic.Length)]);
    }
}
}