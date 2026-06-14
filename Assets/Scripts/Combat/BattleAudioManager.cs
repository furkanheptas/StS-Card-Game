using UnityEngine;

public class BattleAudioManager : MonoBehaviour
{
    public static BattleAudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip battleMusic;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.35f;

    [Header("Combat SFX")]
    [SerializeField] private AudioClip playerAttackSound;
    [SerializeField] private AudioClip enemyAttackSound;
    [SerializeField] private AudioClip playerDamageSound;
    [SerializeField] private AudioClip enemyDamageSound;

    [Header("SFX Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.7f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private bool MusicEnabled => PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
    private bool SfxEnabled => PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    private void Start()
    {
        PlayBattleMusic();
    }

    private void PlayBattleMusic()
    {
        if (battleMusic == null)
            return;

        musicSource.clip = battleMusic;
        musicSource.mute = !MusicEnabled;
        musicSource.Play();
    }

    private void PlaySfx(AudioClip clip)
    {
        if (!SfxEnabled)
            return;

        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayPlayerAttack()
    {
        PlaySfx(playerAttackSound);
    }

    public void PlayEnemyAttack()
    {
        PlaySfx(enemyAttackSound);
    }

    public void PlayPlayerDamage()
    {
        PlaySfx(playerDamageSound);
    }

    public void PlayEnemyDamage()
    {
        PlaySfx(enemyDamageSound);
    }
}