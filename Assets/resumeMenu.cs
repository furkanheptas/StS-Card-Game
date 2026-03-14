using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

public class ResumeMenu : MonoBehaviour 
{
    #region Panel System
    [Header("Panels")]
    public GameObject PausePanel;
    public GameObject InGamePanel;

    private void Start()
    {
        InitializePanels();
        SetupButtons();
    }

    private void InitializePanels() 
    {
        InGamePanel.SetActive(true);
        PausePanel.SetActive(false);
    }

    public void PauseGame() 
    {
        Time.timeScale = 0f;
        InGamePanel.SetActive(false);
        PausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        InGamePanel.SetActive(true);
        PausePanel.SetActive(false);
    }
    #endregion

    #region Button System
    [Header("Buttons")]
    public Button PauseBtn;
    public Button ResumeBtn;
    public Button MainMenuBtn;

    [Header("Scene")]
    public int menuSceneIndex = 0;

    private void SetupButtons()
    {
        PauseBtn.onClick.RemoveAllListeners();
        ResumeBtn.onClick.RemoveAllListeners();
        MainMenuBtn.onClick.RemoveAllListeners();

        PauseBtn.onClick.AddListener(() => { PlayClickSound(); PauseGame(); });
        ResumeBtn.onClick.AddListener(() => { PlayClickSound(); ResumeGame(); });
        MainMenuBtn.onClick.AddListener(() => { PlayClickSound(); GoToMenu(); });

        SetupAudioSystem();
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneIndex);
    }
    #endregion

    #region Audio System
    [Header("Audio")]
    public bool useClickSound = true;
    public bool useHoverSound = true;

    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip hoverSound;

    [Header("Audio Sources")]
    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    private bool sfxEnabled;
    private bool musicEnabled;

    private void SetupAudioSystem()
    {
        LoadAudioSettings();
        AddHoverEvents();
    }

    private void LoadAudioSettings()
    {
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        UpdateMusicStates();
    }

    private void UpdateMusicStates()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.mute = !musicEnabled;
        }
    }

    private void AddHoverEvents()
    {
        AddHoverEventsToButton(PauseBtn);
        AddHoverEventsToButton(ResumeBtn);
        AddHoverEventsToButton(MainMenuBtn);
    }

    private void AddHoverEventsToButton(Button button)
    {
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { PlayHoverSound(); });
        eventTrigger.triggers.Add(entry);
    }

    private void PlayHoverSound()
    {
        if (useHoverSound && sfxEnabled && hoverSound != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(hoverSound);
        }
    }

    private void PlayClickSound()
    {
        if (useClickSound && sfxEnabled && clickSound != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(clickSound);
        }
    }
    #endregion
}