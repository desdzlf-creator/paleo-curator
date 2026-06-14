using UnityEngine;
using UnityEngine.UI;

public class PanelPengaturanUI : MonoBehaviour
{
    [Header("Slider")]
    public Slider sliderBGM;
    public Slider sliderSFX;

    [Header("Icon BGM")]
    public Image ikonBGM;
    public Sprite spriteIkonBGMNormal;
    public Sprite spriteIkonBGMMute;

    [Header("Icon SFX")]
    public Image ikonSFX;
    public Sprite spriteIkonSFXNormal;
    public Sprite spriteIkonSFXMute;

    [Header("Threshold Mute (0.0 - 0.1 dianggap mati)")]
    [Range(0f, 0.2f)] public float batasAnggapMute = 0.05f;

    void OnEnable()
    {
        float savedBGM = PlayerPrefs.GetFloat("VolumeBGM", 0.7f);
        float savedSFX = PlayerPrefs.GetFloat("VolumeSFX", 0.7f);

        Debug.Log($"[PanelPengaturanUI] OnEnable - BGM: {savedBGM}, SFX: {savedSFX}");

        if (sliderBGM != null) sliderBGM.value = savedBGM;
        if (sliderSFX != null) sliderSFX.value = savedSFX;

        UpdateIkonBGM(savedBGM);
        UpdateIkonSFX(savedSFX);
    }

    public void OnSliderBGMChanged(float value)
    {
        Debug.Log($"[PanelPengaturanUI] BGM slider berubah: {value}");
        if (AudioManager.Instance == null)
            Debug.LogWarning("[PanelPengaturanUI] AudioManager.Instance NULL!");
        AudioManager.Instance?.SetVolumeBGM(value);
        UpdateIkonBGM(value);
    }

    public void OnSliderSFXChanged(float value)
    {
        Debug.Log($"[PanelPengaturanUI] SFX slider berubah: {value}");
        if (AudioManager.Instance == null)
            Debug.LogWarning("[PanelPengaturanUI] AudioManager.Instance NULL!");
        AudioManager.Instance?.SetVolumeSFX(value);
        UpdateIkonSFX(value);
    }

    private void UpdateIkonBGM(float value)
    {
        if (ikonBGM == null)
        {
            Debug.LogWarning("[PanelPengaturanUI] ikonBGM belum diassign!");
            return;
        }

        bool isMute = value <= batasAnggapMute;
        Debug.Log($"[PanelPengaturanUI] UpdateIkonBGM - value:{value} isMute:{isMute}");

        if (spriteIkonBGMMute != null && spriteIkonBGMNormal != null)
            ikonBGM.sprite = isMute ? spriteIkonBGMMute : spriteIkonBGMNormal;
        else
            ikonBGM.color = isMute ? new Color(0.4f, 0.4f, 0.4f, 1f) : Color.white;
    }

    private void UpdateIkonSFX(float value)
    {
        if (ikonSFX == null)
        {
            Debug.LogWarning("[PanelPengaturanUI] ikonSFX belum diassign!");
            return;
        }

        bool isMute = value <= batasAnggapMute;
        Debug.Log($"[PanelPengaturanUI] UpdateIkonSFX - value:{value} isMute:{isMute}");

        if (spriteIkonSFXMute != null && spriteIkonSFXNormal != null)
            ikonSFX.sprite = isMute ? spriteIkonSFXMute : spriteIkonSFXNormal;
        else
            ikonSFX.color = isMute ? new Color(0.4f, 0.4f, 0.4f, 1f) : Color.white;
    }

    public void KlikSimpan()
    {
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }

    public void KlikBatal()
    {
        AudioManager.Instance?.LoadVolume();
        float savedBGM = PlayerPrefs.GetFloat("VolumeBGM", 0.7f);
        float savedSFX = PlayerPrefs.GetFloat("VolumeSFX", 0.7f);
        if (sliderBGM != null) sliderBGM.value = savedBGM;
        if (sliderSFX != null) sliderSFX.value = savedSFX;
        UpdateIkonBGM(savedBGM);
        UpdateIkonSFX(savedSFX);
        gameObject.SetActive(false);
    }
}