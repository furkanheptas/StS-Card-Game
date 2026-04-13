using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        if (InGamePanel != null) InGamePanel.SetActive(true);
        if (PausePanel != null) PausePanel.SetActive(false);
    }

    private void Update()
    {
        // ESC Tuşu Kontrolü
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (PausePanel != null && PausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (InGamePanel != null) InGamePanel.SetActive(false);
        if (PausePanel != null) PausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (InGamePanel != null) InGamePanel.SetActive(true);
        if (PausePanel != null) PausePanel.SetActive(false);
    }
    #endregion

    #region Button System
    [Header("Buttons")]
    public Button ResumeBtn;
    public Button MainMenuBtn;

    [Header("Scene")]
    public int menuSceneIndex = 0;

    private void SetupButtons()
    {
        // Sadece sahnede olan butonları kuruyoruz
        if (ResumeBtn != null)
        {
            ResumeBtn.onClick.RemoveAllListeners();
            ResumeBtn.onClick.AddListener(() => { PlayClickSound(); ResumeGame(); });
            AddHoverEventsToButton(ResumeBtn);
        }

        if (MainMenuBtn != null)
        {
            MainMenuBtn.onClick.RemoveAllListeners();
            MainMenuBtn.onClick.AddListener(() => { PlayClickSound(); GoToMenu(); });
            AddHoverEventsToButton(MainMenuBtn);
        }

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

    private void AddHoverEventsToButton(Button button)
    {
        if (button == null) return;

        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { PlayHoverSound(); });
        eventTrigger.triggers.Clear(); // Eski triggerları temizle
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