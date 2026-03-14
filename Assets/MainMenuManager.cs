using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    void Start()
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
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        PlayClickSound();
    }

    public void CloseSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        PlayClickSound();
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
            {
                videoPlayer.clip = backgroundVideoClip;
            }
        }
        UpdateBackgroundVideoState();
    }

    private void UpdateBackgroundVideoState()
    {
        if (playBackgroundVideo && clip != null)
        {
            if (backgroundPanel != null) backgroundPanel.SetActive(false);
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
            if (backgroundPanel != null) backgroundPanel.SetActive(true);
            if (clip != null) clip.SetActive(false);
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
        if (musicToggle != null) musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        if (sfxToggle != null) sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
    }

    private void LoadSettings()
    {
        if (musicToggle != null)
            musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        if (sfxToggle != null)
            sfxToggle.isOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        UpdateAudioSourcesMute();
    }

    private void OnMusicToggleChanged(bool value)
    {
        PlayerPrefs.SetInt("MusicEnabled", value ? 1 : 0);
        PlayerPrefs.Save();
        UpdateAudioSourcesMute();
        PlayClickSound();
    }

    private void OnSFXToggleChanged(bool value)
    {
        PlayerPrefs.SetInt("SFXEnabled", value ? 1 : 0);
        PlayerPrefs.Save();
        UpdateAudioSourcesMute();
        PlayClickSound();
    }
    #endregion

    #region Audio
    [Header("Audio Settings")]
    public bool playHoverSound = true;
    public bool playClickSound = true;

    [Header("Audio Clips")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Audio Source")]
    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    private void SetupAudioSystem()
    {
        UpdateAudioSourcesMute();
        AddButtonListeners();
        AddHoverEvents();
    }

    private void UpdateAudioSourcesMute()
    {
        if (musicAudioSource != null && musicToggle != null)
            musicAudioSource.mute = !musicToggle.isOn;
        if (sfxAudioSource != null && sfxToggle != null)
            sfxAudioSource.mute = !sfxToggle.isOn;
    }

    private void AddButtonListeners()
    {
        if (StartButton != null) StartButton.onClick.AddListener(PlayGame);
        if (OpenSettingsButton != null) OpenSettingsButton.onClick.AddListener(OpenSettings);
        if (CloseSettingsButton != null) CloseSettingsButton.onClick.AddListener(CloseSettings);
        if (QuitButton != null) QuitButton.onClick.AddListener(QuitGame);
    }

    private void AddHoverEvents()
    {
        AddHoverEventToUI(StartButton?.gameObject);
        AddHoverEventToUI(OpenSettingsButton?.gameObject);
        AddHoverEventToUI(CloseSettingsButton?.gameObject);
        AddHoverEventToUI(QuitButton?.gameObject);
        AddHoverEventToUI(musicToggle?.gameObject);
        AddHoverEventToUI(sfxToggle?.gameObject);
    }

    private void AddHoverEventToUI(GameObject uiElement)
    {
        if (uiElement == null) return;

        EventTrigger trigger = uiElement.GetComponent<EventTrigger>();
        if (trigger == null) trigger = uiElement.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
    }

    public void PlayHoverSound()
    {
        if (playHoverSound && hoverSound != null && sfxAudioSource != null)
            sfxAudioSource.PlayOneShot(hoverSound);
    }

    public void PlayClickSound()
    {
        if (playClickSound && clickSound != null && sfxAudioSource != null)
            sfxAudioSource.PlayOneShot(clickSound);
    }
    #endregion
}