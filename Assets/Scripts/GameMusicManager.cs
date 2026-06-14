using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager Instance { get; private set; }

    [Header("Scene Settings")]
    [SerializeField] private int mainMenuSceneIndex = 0;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip battleMusic;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.35f;

    private AudioSource musicSource;
    private AudioClip currentClip;

    private bool MusicEnabled => PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        musicSource.mute = !MusicEnabled;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        UpdateMusicForCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMusicForScene(scene.buildIndex);
    }

    private void UpdateMusicForCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        UpdateMusicForScene(activeScene.buildIndex);
    }

    private void UpdateMusicForScene(int sceneIndex)
    {
        AudioClip targetClip;

        if (sceneIndex == mainMenuSceneIndex)
            targetClip = mainMenuMusic;
        else
            targetClip = battleMusic;

        PlayMusic(targetClip);
    }

    private void PlayMusic(AudioClip clip)
    {
        musicSource.mute = !MusicEnabled;
        musicSource.volume = musicVolume;

        if (clip == null)
        {
            musicSource.Stop();
            currentClip = null;
            return;
        }

        if (currentClip == clip && musicSource.isPlaying)
            return;

        currentClip = clip;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void RefreshMusicSetting()
    {
        if (musicSource == null)
            return;

        musicSource.mute = !MusicEnabled;

        if (MusicEnabled && !musicSource.isPlaying && currentClip != null)
            musicSource.Play();
    }
}