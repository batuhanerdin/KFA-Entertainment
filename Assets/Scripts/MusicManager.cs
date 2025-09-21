using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;

    [Header("Music Clips")]
    [SerializeField] private AudioClip[] chillClips;
    [SerializeField] private AudioClip[] waveClips;

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f; // ✅ genel volume
    [Range(0f, 1f)] public float chillVolume = 1f;
    [Range(0f, 1f)] public float waveVolume = 1f;

    [Header("Crossfade Settings")]
    [SerializeField] private float fadeDuration = 1.5f;

    private AudioSource activeSource;
    private AudioSource inactiveSource;
    private AudioClip currentClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupSource(sourceA);
        SetupSource(sourceB);

        activeSource = sourceA;
        inactiveSource = sourceB;
    }

    private void SetupSource(AudioSource src)
    {
        if (src != null)
        {
            src.loop = true;
            src.playOnAwake = false;
            src.volume = 0f;
        }
    }

    // === PUBLIC API ===

    public void PlayChillMusic()
    {
        AudioClip nextClip = GetRandomClip(chillClips);
        if (nextClip == null) return;
        if (nextClip == currentClip) return;

        CrossfadeTo(nextClip, chillVolume);
    }

    public void PlayWaveMusic()
    {
        AudioClip nextClip = GetRandomClip(waveClips);
        if (nextClip == null) return;
        if (nextClip == currentClip) return;

        CrossfadeTo(nextClip, waveVolume);
    }

    public void StopAllMusic()
    {
        activeSource.DOFade(0f, fadeDuration).OnComplete(() => activeSource.Stop());
        inactiveSource.DOFade(0f, fadeDuration).OnComplete(() => inactiveSource.Stop());
        currentClip = null;
    }

    // === INTERNAL ===

    private void CrossfadeTo(AudioClip newClip, float targetVolume)
    {
        // aktif fade out
        if (activeSource.isPlaying)
            activeSource.DOFade(0f, fadeDuration);

        // inaktif yeni şarkı
        inactiveSource.clip = newClip;
        inactiveSource.volume = 0f;
        inactiveSource.Play();
        inactiveSource.DOFade(targetVolume * masterVolume, fadeDuration);

        // değiş tokuş
        (activeSource, inactiveSource) = (inactiveSource, activeSource);
        currentClip = newClip;
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        int idx = Random.Range(0, clips.Length);
        return clips[idx];
    }
}
