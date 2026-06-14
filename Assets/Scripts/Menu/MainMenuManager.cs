using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        InitializePanels();
        SetupBackgroundVideo();
        SetupAudioSystem();
        SetupToggleSystem();
    }

    #region Panels

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject backgroundPanel;

    private void InitializePanels()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        PlayClickSound();

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    #endregion

    #region Background

    [Header("Background Video")]
    public bool playBackgroundVideo = true;
    public GameObject clip;
    public VideoClip backgroundVideoClip;
    public bool videoMute = false;
    [Range(0f, 1f)] public float videoVolume = 1f;
    [Range(0.1f, 2f)] public float playbackSpeed = 1f;

    private VideoPlayer videoPlayer;

    private void SetupBackgroundVideo()
    {
        if (clip != null)
        {
            videoPlayer = clip.GetComponent<VideoPlayer>();

            if (videoPlayer != null && backgroundVideoClip != null)
                videoPlayer.clip = backgroundVideoClip;
        }

        UpdateBackgroundVideoState();
    }

    private void UpdateBackgroundVideoState()
    {
        if (playBackgroundVideo && clip != null)
        {
            if (backgroundPanel != null)
                backgroundPanel.SetActive(false);

            clip.SetActive(true);

            if (videoPlayer != null)
            {
                videoPlayer.SetDirectAudioMute(0, videoMute);
                videoPlayer.SetDirectAudioVolume(0, videoVolume);
                videoPlayer.playbackSpeed = playbackSpeed;
                videoPlayer.Play();
            }
        }
        else
        {
            if (backgroundPanel != null)
                backgroundPanel.SetActive(true);

            if (clip != null)
                clip.SetActive(false);
        }
    }

    #endregion

    #region Buttons

    [Header("Buttons")]
    public Button StartButton;
    public Button OpenSettingsButton;
    public Button CloseSettingsButton;
    public Button QuitButton;

    [Header("Scene")]
    public int gameSceneIndex = 1;

    public void PlayGame()
    {
        PlayClickSound();
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void QuitGame()
    {
        PlayClickSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Toggles

    [Header("Toggles")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void SetupToggleSystem()
    {
        LoadSettings();

        if (musicToggle != null)
        {
            musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }

        if (sfxToggle != null)
        {
            sfxToggle.onValueChanged.RemoveListener(OnSFXToggleChanged);
            sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
        }
    }

    private void LoadSettings()
    {
        bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        bool sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        if (musicToggle != null)
            musicToggle.isOn = musicEnabled;

        if (sfxToggle != null)
            sfxToggle.isOn = sfxEnabled;

        UpdateAudioSourcesMute();
    }

    private void OnMusicToggleChanged(bool value)
    {
        PlayerPrefs.SetInt("MusicEnabled", value ? 1 : 0);
        PlayerPrefs.Save();

        UpdateAudioSourcesMute();

        if (GameMusicManager.Instance != null)
            GameMusicManager.Instance.RefreshMusicSetting();

        PlayClickSound();
    }

    private void OnSFXToggleChanged(bool value)
    {
        PlayerPrefs.SetInt("SFXEnabled", value ? 1 : 0);
        PlayerPrefs.Save();

        UpdateAudioSourcesMute();

        if (value)
            PlayClickSound();
    }

    #endregion

    #region Audio

    [Header("Audio Settings")]
    public bool playHoverSound = true;
    public bool playClickSound = true;

    [Header("Music Clip")]
    public AudioClip mainMenuMusic;
    [Range(0f, 1f)] public float mainMenuMusicVolume = 0.35f;

    [Header("SFX Clips")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    [Range(0f, 1f)] public float sfxVolume = 0.75f;

    [Header("Audio Sources")]
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;

    private void SetupAudioSystem()
    {
        CreateAudioSourcesIfNeeded();
        SetupMainMenuMusic();
        UpdateAudioSourcesMute();

        AddButtonListeners();
        AddHoverEvents();
    }

    private void CreateAudioSourcesIfNeeded()
    {
        if (musicAudioSource == null)
        {
            GameObject musicObject = new GameObject("MainMenuMusicSource");
            musicObject.transform.SetParent(transform);

            musicAudioSource = musicObject.AddComponent<AudioSource>();
        }

        if (sfxAudioSource == null)
        {
            GameObject sfxObject = new GameObject("MainMenuSFXSource");
            sfxObject.transform.SetParent(transform);

            sfxAudioSource = sfxObject.AddComponent<AudioSource>();
        }

        musicAudioSource.playOnAwake = false;
        musicAudioSource.loop = true;
        musicAudioSource.volume = mainMenuMusicVolume;

        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.loop = false;
        sfxAudioSource.volume = sfxVolume;
    }

    private void SetupMainMenuMusic()
    {
        if (musicAudioSource == null)
            return;

        musicAudioSource.clip = mainMenuMusic;
        musicAudioSource.volume = mainMenuMusicVolume;
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;

        if (mainMenuMusic != null && !musicAudioSource.isPlaying)
            musicAudioSource.Play();
    }

    private void UpdateAudioSourcesMute()
    {
        bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        bool sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        if (musicAudioSource != null)
            musicAudioSource.mute = !musicEnabled;

        if (sfxAudioSource != null)
            sfxAudioSource.mute = !sfxEnabled;
    }

    private void AddButtonListeners()
    {
        if (StartButton != null)
        {
            StartButton.onClick.RemoveListener(PlayGame);
            StartButton.onClick.AddListener(PlayGame);
        }

        if (OpenSettingsButton != null)
        {
            OpenSettingsButton.onClick.RemoveListener(OpenSettings);
            OpenSettingsButton.onClick.AddListener(OpenSettings);
        }

        if (CloseSettingsButton != null)
        {
            CloseSettingsButton.onClick.RemoveListener(CloseSettings);
            CloseSettingsButton.onClick.AddListener(CloseSettings);
        }

        if (QuitButton != null)
        {
            QuitButton.onClick.RemoveListener(QuitGame);
            QuitButton.onClick.AddListener(QuitGame);
        }
    }

    private void AddHoverEvents()
    {
        AddHoverEventToUI(StartButton != null ? StartButton.gameObject : null);
        AddHoverEventToUI(OpenSettingsButton != null ? OpenSettingsButton.gameObject : null);
        AddHoverEventToUI(CloseSettingsButton != null ? CloseSettingsButton.gameObject : null);
        AddHoverEventToUI(QuitButton != null ? QuitButton.gameObject : null);
        AddHoverEventToUI(musicToggle != null ? musicToggle.gameObject : null);
        AddHoverEventToUI(sfxToggle != null ? sfxToggle.gameObject : null);
    }

    private void AddHoverEventToUI(GameObject uiElement)
    {
        if (uiElement == null)
            return;

        EventTrigger trigger = uiElement.GetComponent<EventTrigger>();

        if (trigger == null)
            trigger = uiElement.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { PlayHoverSound(); });

        trigger.triggers.Add(entry);
    }

    public void PlayHoverSound()
    {
        bool sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        if (!sfxEnabled)
            return;

        if (!playHoverSound)
            return;

        if (hoverSound == null || sfxAudioSource == null)
            return;

        sfxAudioSource.PlayOneShot(hoverSound, sfxVolume);
    }

    public void PlayClickSound()
    {
        bool sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        if (!sfxEnabled)
            return;

        if (!playClickSound)
            return;

        if (clickSound == null || sfxAudioSource == null)
            return;

        sfxAudioSource.PlayOneShot(clickSound, sfxVolume);
    }

    #endregion
}