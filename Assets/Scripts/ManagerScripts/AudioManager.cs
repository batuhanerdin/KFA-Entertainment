using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip playerHitSfx;
    [SerializeField] private AudioClip dieSfx;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioClip playerAttackSfx;
    [SerializeField] private AudioClip[] hitSfxClips;
    [SerializeField] private AudioClip coinPickupSfx;
    [SerializeField] private AudioClip obeliskHitSfx;
    
    [Header("Shop / Cart Clips")]
    [SerializeField] private AudioClip cartMovementSfx;
    [SerializeField] private AudioClip shopSetupSfx;
    [SerializeField] private AudioClip shopOpenSfx;

    [Header("General Volume")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Individual Volumes")]
    [Range(0f, 1f)] public float playerHitVolume = 1f;
    [Range(0f, 1f)] public float hitVolume = 1f;
    [Range(0f, 1f)] public float dieVolume = 1f;
    [Range(0f, 1f)] public float footstepVolume = 1f;
    [Range(0f, 1f)] public float playerAttackVolume = 1f;
    [Range(0f, 1f)] public float cartMovementVolume = 1f;
    [Range(0f, 1f)] public float shopSetupVolume = 1f;
    [Range(0f, 1f)] public float shopOpenVolume = 1f;
    [Range(0f, 1f)] public float coinPickupVolume = 1f;
    [Range(0f, 1f)] public float obeliskHitVolume = 1f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }
    public void MuteAll()
    {
        foreach (var source in GetComponentsInChildren<AudioSource>())
            source.mute = true;
    }

    public void UnmuteAll()
    {
        foreach (var source in GetComponentsInChildren<AudioSource>())
            source.mute = false;
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

    public void PlayPlayerHit(float volume = 1f)
    {
        if (playerHitSfx != null)
            PlaySfxOneShot(playerHitSfx, playerHitVolume * volume);
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
    public void PlayObeliskHit()
    {
        PlaySfxOneShot(obeliskHitSfx, obeliskHitVolume);
    }

    // === Cart & Shop Sesleri ===
    public void PlayCartMovement(float volume = 1f)
    {
        if (!sfxSource || !cartMovementSfx) return;

        sfxSource.clip = cartMovementSfx;
        sfxSource.volume = sfxVolume * cartMovementVolume * volume;
        sfxSource.loop = true;
        sfxSource.Play();
    }

    public void StopCartMovement()
    {
        if (!sfxSource) return;

        if (sfxSource.isPlaying && sfxSource.clip == cartMovementSfx)
            sfxSource.Stop();

        sfxSource.loop = false;
        sfxSource.clip = null;
        sfxSource.volume = sfxVolume; // reset
    }

    public void PlayCoinPickup()
    => PlaySfxOneShot(coinPickupSfx, coinPickupVolume);
    public void PlayShopSetup(float volume = 1f)
        => PlaySfxOneShot(shopSetupSfx, shopSetupVolume * volume);

    public void PlayShopOpen(float volume = 1f)
        => PlaySfxOneShot(shopOpenSfx, shopOpenVolume * volume);

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
