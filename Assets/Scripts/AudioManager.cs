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

    [Header("Attack & Hit Clips")]
    [SerializeField] private AudioClip playerAttackSfx;    // ✅ Player saldırı sesi
    [SerializeField] private AudioClip[] hitSfxClips;      // ✅ Düşman hasar sesleri

    [Header("General Volume")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Individual Volumes")]
    [Range(0f, 1f)] public float hitVolume = 1f;
    [Range(0f, 1f)] public float dieVolume = 1f;
    [Range(0f, 1f)] public float footstepVolume = 1f;
    [Range(0f, 1f)] public float playerAttackVolume = 1f;

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
        if (musicSource == null || musicClip == null) return;

        musicSource.clip = musicClip;
        musicSource.volume = musicVolume * volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    // === Internal SFX helper ===
    private void PlaySfxInternal(AudioClip clip, float finalVolume)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, finalVolume);
    }

    // === Specific SFX ===
    public void PlayHit(float volume = 1f)
    {
        if (hitSfxClips.Length == 0) return;

        int index = Random.Range(0, hitSfxClips.Length);
        float finalVolume = sfxVolume * hitVolume * volume;
        PlaySfxInternal(hitSfxClips[index], finalVolume);
    }

    public void PlayDie(float volume = 1f)
    {
        float finalVolume = sfxVolume * dieVolume * volume;
        PlaySfxInternal(dieSfx, finalVolume);
    }

    public void PlayFootstep(float volume = 1f)
    {
        if (footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        float finalVolume = sfxVolume * footstepVolume * volume;
        PlaySfxInternal(footstepClips[index], finalVolume);
    }

    public void PlayPlayerAttack(float volume = 1f)
    {
        float finalVolume = sfxVolume * playerAttackVolume * volume;
        PlaySfxInternal(playerAttackSfx, finalVolume);
    }

    // Footstep dizisine dışarıdan erişim (sıralı çalmak için PlayerMovement’te kullanılıyor olabilir)
    public AudioClip[] GetFootstepClips() => footstepClips;
}
