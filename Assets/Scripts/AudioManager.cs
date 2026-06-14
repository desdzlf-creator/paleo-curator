using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM Clips")]
    public AudioClip bgmMainMenu;
    public AudioClip bgmGameplay;

    //mencegah suara menjadi double
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadVolume();
    }

    // ================================
    // FUNGSI BGM
    // ================================
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();

    // ================================
    // FUNGSI SFX
    // ================================
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // ================================
    // FUNGSI VOLUME
    // ================================
    public void SetVolumeBGM(float value)
    {
        bgmSource.volume = value;
        PlayerPrefs.SetFloat("VolumeBGM", value);
    }

    public void SetVolumeSFX(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("VolumeSFX", value);
    }

    public void LoadVolume()
    {
        bgmSource.volume = PlayerPrefs.GetFloat("VolumeBGM", 0.7f);
        sfxSource.volume = PlayerPrefs.GetFloat("VolumeSFX", 0.7f);
    }
}