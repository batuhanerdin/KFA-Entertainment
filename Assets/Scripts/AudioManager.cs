using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip dieSfx;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioClip playerAttackSfx;
    [SerializeField] private AudioClip[] hitSfxClips;

    [Header("Shop / Cart Clips")]
    [SerializeField] private AudioClip cartMovementSfx;   // Cart hareketi
    [SerializeField] private AudioClip shopSetupSfx;      // Market sahneye kuruldu/kapatıldı
    [SerializeField] private AudioClip shopOpenSfx;       // Market UI açıldı (E tuşu)

    [Header("General Volume")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Individual Volumes")]
    [Range(0f, 1f)] public float hitVolume = 1f;
    [Range(0f, 1f)] public float dieVolume = 1f;
    [Range(0f, 1f)] public float footstepVolume = 1f;
    [Range(0f, 1f)] public float playerAttackVolume = 1f;
    [Range(0f, 1f)] public float cartMovementVolume = 1f;
    [Range(0f, 1f)] public float shopSetupVolume = 1f;
    [Range(0f, 1f)] public float shopOpenVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    // === Music ===
    public void PlayMusic(float volume = 1f)
    {
        if (!musicSource || !musicClip) return;
        musicSource.clip = musicClip;
        musicSource.volume = musicVolume * volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource) musicSource.Stop();
    }

    // === Internal helper ===
    private void PlaySfxOneShot(AudioClip clip, float mult = 1f)
    {
        if (!sfxSource || !clip) return;
        sfxSource.PlayOneShot(clip, sfxVolume * mult);
    }

    // === Public SFX ===
    public void PlayHit(float volume = 1f)
    {
        if (hitSfxClips == null || hitSfxClips.Length == 0) return;
        int i = Random.Range(0, hitSfxClips.Length);
        PlaySfxOneShot(hitSfxClips[i], hitVolume * volume);
    }

    public void PlayDie(float volume = 1f)
        => PlaySfxOneShot(dieSfx, dieVolume * volume);

    public void PlayPlayerAttack(float volume = 1f)
        => PlaySfxOneShot(playerAttackSfx, playerAttackVolume * volume);

    public void PlayFootstep(float volume = 1f)
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        int i = Random.Range(0, footstepClips.Length);
        PlaySfxOneShot(footstepClips[i], footstepVolume * volume);
    }

    // === Cart & Shop Sesleri ===
    public void StartCartMovement()
    {
        if (!sfxSource || !cartMovementSfx) return;
        sfxSource.clip = cartMovementSfx;
        sfxSource.volume = sfxVolume * cartMovementVolume;
        sfxSource.loop = true; // hareket boyunca devam etsin
        sfxSource.Play();
    }

    public void StopCartMovement()
    {
        if (!sfxSource) return;
        if (sfxSource.isPlaying && sfxSource.clip == cartMovementSfx)
            sfxSource.Stop();
        sfxSource.loop = false;
        sfxSource.clip = null;
    }

    public void PlayShopSetup()
        => PlaySfxOneShot(shopSetupSfx, shopSetupVolume);

    public void PlayShopOpen()
        => PlaySfxOneShot(shopOpenSfx, shopOpenVolume);

    // === Footstep Sequential Helper ===
    public AudioClip[] GetFootstepClips() => footstepClips;

    public int PlayFootstepSequential(int currentIndex, float volume = 1f)
    {
        if (footstepClips == null || footstepClips.Length == 0) return 0;
        if (currentIndex < 0 || currentIndex >= footstepClips.Length) currentIndex = 0;

        PlaySfxOneShot(footstepClips[currentIndex], footstepVolume * volume);

        int next = currentIndex + 1;
        if (next >= footstepClips.Length) next = 0;
        return next;
    }
}
