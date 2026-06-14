using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Nama Scene Tujuan")]
    public string namaSceneDialog = "Scene01_OverWorld";

    [Header("Setup Tombol Mulai/Lanjut (Murni Gambar)")]
    public Image komponenGambarBtnMulai;
    public Sprite spriteTombolMulai;
    public Sprite spriteTombolLanjut;

    [Header("Panel Utama UI")]
    public GameObject panelMainMenu;
    public GameObject panelKoleksi;

    [Header("Sub Panel Koleksi")]
    public GameObject bgDisplay;
    public GameObject panelLemariArsipPopup;

    [Header("Sistem Carousel Fosil (Urutan Wajib Sama Dengan Level!)")]
    [InspectorName("Daftar Fosil (0:Tri, 1:Ty, 2:Stego)")]
    public List<GameObject> daftarFosil;

    private List<GameObject> fosilYangSudahTerbuka = new List<GameObject>();
    private int indexFosilSekarang = 0;

    [Header("Sistem Peti Lemari Arsip")]
    public GameObject panelBgGelap;
    public List<Button> daftarTombolPeti;
    public List<GameObject> daftarPopupFunFact;

    [Header("Panel Pengaturan")]
    public GameObject panelPengaturan;
    public Slider sliderBGM;
    public Slider sliderSFX;

    // ==========================================
    // SFX MAIN MENU
    // ==========================================
    [Header("SFX Main Menu")]
    public AudioClip sfxKlikTombol;      // Suara klik tombol umum (Mulai, Koleksi, dll)
    public AudioClip sfxBukaPeti;        // Suara pas peti fun fact dibuka
    public AudioClip sfxTutupPanel;      // Suara pas panel ditutup

    // Helper 
    private void PlaySFX(AudioClip clip)
    {
        AudioManager.Instance?.PlaySFX(clip);
    }

    void Start()
    {
        if (panelMainMenu != null) panelMainMenu.SetActive(true);
        if (panelKoleksi != null) panelKoleksi.SetActive(false);
        if (panelPengaturan != null) panelPengaturan.SetActive(false);

        if (sliderBGM != null) sliderBGM.value = PlayerPrefs.GetFloat("VolumeBGM", 0.7f);
        if (sliderSFX != null) sliderSFX.value = PlayerPrefs.GetFloat("VolumeSFX", 0.7f);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.LoadVolume();
            AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmMainMenu);
        }

        UpdateTampilanTombolMulai();

        if (panelBgGelap != null) panelBgGelap.SetActive(false);
        foreach (GameObject funFact in daftarPopupFunFact)
        {
            if (funFact != null) funFact.SetActive(false);
        }

        if (PlayerPrefs.GetInt("BukaKoleksiOtomatis", 0) == 1)
        {
            PlayerPrefs.DeleteKey("BukaKoleksiOtomatis");
            PlayerPrefs.Save();
            KlikKoleksi();
        }

        SceneTransitionManager transisi = FindFirstObjectByType<SceneTransitionManager>();
        if (transisi != null && transisi.pakaiAnimasiFadeIn)
        {
            StartCoroutine(transisi.ProsesFadeIn());
        }
    }

    private void UpdateTampilanTombolMulai()
    {
        int levelTersimpan = PlayerPrefs.GetInt("LevelTerakhir", 0);
        if (levelTersimpan > 0 && komponenGambarBtnMulai != null && spriteTombolLanjut != null)
            komponenGambarBtnMulai.sprite = spriteTombolLanjut;
        else if (komponenGambarBtnMulai != null && spriteTombolMulai != null)
            komponenGambarBtnMulai.sprite = spriteTombolMulai;
    }

    // ==========================================
    // 1. FUNGSI TOMBOL MULAI / LANJUT
    // ==========================================
    public void KlikMulaiGame()
    {
        PlaySFX(sfxKlikTombol);
        if (FindFirstObjectByType<SceneTransitionManager>() != null)
            FindFirstObjectByType<SceneTransitionManager>().KlikTombolNext();
        else
            SceneManager.LoadScene(namaSceneDialog);
    }

    // ==========================================
    // 2. FUNGSI UNTUK MENU KOLEKSI & LEMARI ARSIP
    // ==========================================
    public void KlikKoleksi()
    {
        PlaySFX(sfxKlikTombol);
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelKoleksi != null) panelKoleksi.SetActive(true);
        if (bgDisplay != null) bgDisplay.SetActive(true);
        if (panelLemariArsipPopup != null) panelLemariArsipPopup.SetActive(false);

        FilterFosilTerbuka();
        indexFosilSekarang = 0;
        UpdateTampilanCarousel();
    }

    private void FilterFosilTerbuka()
    {
        fosilYangSudahTerbuka.Clear();
        int levelTersimpan = PlayerPrefs.GetInt("LevelTerakhir", 0);

        for (int i = 0; i < daftarFosil.Count; i++)
        {
            if (daftarFosil[i] == null) continue;
            if (levelTersimpan > i)
                fosilYangSudahTerbuka.Add(daftarFosil[i]);
            else
                daftarFosil[i].SetActive(false);
        }
    }

    private void UpdateTampilanCarousel()
    {
        if (fosilYangSudahTerbuka.Count == 0) return;
        for (int i = 0; i < fosilYangSudahTerbuka.Count; i++)
        {
            if (fosilYangSudahTerbuka[i] != null)
                fosilYangSudahTerbuka[i].SetActive(i == indexFosilSekarang);
        }
    }

    public void KlikTombolKanan()
    {
        if (fosilYangSudahTerbuka.Count <= 1) return;
        PlaySFX(sfxKlikTombol);
        indexFosilSekarang = (indexFosilSekarang + 1) % fosilYangSudahTerbuka.Count;
        UpdateTampilanCarousel();
    }

    public void KlikTombolKiri()
    {
        if (fosilYangSudahTerbuka.Count <= 1) return;
        PlaySFX(sfxKlikTombol);
        indexFosilSekarang = (indexFosilSekarang - 1 + fosilYangSudahTerbuka.Count) % fosilYangSudahTerbuka.Count;
        UpdateTampilanCarousel();
    }

    public void KlikBukaLemariArsip()
    {
        PlaySFX(sfxKlikTombol);
        if (panelLemariArsipPopup != null) panelLemariArsipPopup.SetActive(true);
        if (panelBgGelap != null) panelBgGelap.SetActive(false);
        CekStatusPetiLemariArsip();
    }

    private void CekStatusPetiLemariArsip()
    {
        int levelTersimpan = PlayerPrefs.GetInt("LevelTerakhir", 0);
        for (int i = 0; i < daftarTombolPeti.Count; i++)
        {
            if (daftarTombolPeti[i] == null) continue;
            Image gambarPeti = daftarTombolPeti[i].GetComponent<Image>();
            if (levelTersimpan > i)
            {
                daftarTombolPeti[i].interactable = true;
                if (gambarPeti != null) { Color c = gambarPeti.color; c.a = 1.0f; gambarPeti.color = c; }
            }
            else
            {
                daftarTombolPeti[i].interactable = false;
                if (gambarPeti != null) { Color c = gambarPeti.color; c.a = 0.65f; gambarPeti.color = c; }
            }
        }
    }

    public void KlikBukaPetiFunFact(int indeksDino)
    {
        if (indeksDino >= 0 && indeksDino < daftarPopupFunFact.Count)
        {
            if (daftarPopupFunFact[indeksDino] != null)
            {
                PlaySFX(sfxBukaPeti); // SFX khusus buka peti
                if (panelBgGelap != null) panelBgGelap.SetActive(true);
                daftarPopupFunFact[indeksDino].SetActive(true);
            }
        }
    }

    public void KlikTutupFunFact(int indeksDino)
    {
        if (indeksDino >= 0 && indeksDino < daftarPopupFunFact.Count)
        {
            if (daftarPopupFunFact[indeksDino] != null)
            {
                PlaySFX(sfxTutupPanel);
                daftarPopupFunFact[indeksDino].SetActive(false);
                if (panelBgGelap != null) panelBgGelap.SetActive(false);
            }
        }
    }

    public void KlikTutupLemariArsip()
    {
        PlaySFX(sfxTutupPanel);
        if (panelBgGelap != null) panelBgGelap.SetActive(false);
        if (panelLemariArsipPopup != null) panelLemariArsipPopup.SetActive(false);
    }

    public void KlikKembaliKeMainMenu()
    {
        PlaySFX(sfxTutupPanel);
        if (panelKoleksi != null) panelKoleksi.SetActive(false);
        if (panelMainMenu != null) panelMainMenu.SetActive(true);
    }

    // ==========================================
    // 3. FUNGSI TOMBOL PENGATURAN
    // ==========================================
    public void KlikPengaturan()
    {
        PlaySFX(sfxKlikTombol);
        if (panelPengaturan != null) panelPengaturan.SetActive(true);
        if (sliderBGM != null) sliderBGM.value = PlayerPrefs.GetFloat("VolumeBGM", 0.7f);
        if (sliderSFX != null) sliderSFX.value = PlayerPrefs.GetFloat("VolumeSFX", 0.7f);
    }

    public void OnSliderBGMChanged(float value)
    {
        AudioManager.Instance?.SetVolumeBGM(value);
    }

    public void OnSliderSFXChanged(float value)
    {
        AudioManager.Instance?.SetVolumeSFX(value);
    }

    public void KlikSimpanPengaturan()
    {
        PlaySFX(sfxKlikTombol);
        PlayerPrefs.Save();
        if (panelPengaturan != null) panelPengaturan.SetActive(false);
    }

    public void KlikBatalPengaturan()
    {
        PlaySFX(sfxTutupPanel);
        if (AudioManager.Instance != null) AudioManager.Instance.LoadVolume();
        if (sliderBGM != null) sliderBGM.value = PlayerPrefs.GetFloat("VolumeBGM", 0.7f);
        if (sliderSFX != null) sliderSFX.value = PlayerPrefs.GetFloat("VolumeSFX", 0.7f);
        if (panelPengaturan != null) panelPengaturan.SetActive(false);
    }

    public void KlikResetGame()
    {
        PlaySFX(sfxKlikTombol);
        PlayerPrefs.DeleteKey("LevelTerakhir");
        PlayerPrefs.Save();

        UpdateTampilanTombolMulai();
        fosilYangSudahTerbuka.Clear();
        foreach (GameObject fosil in daftarFosil)
        {
            if (fosil != null) fosil.SetActive(false);
        }

        CekStatusPetiLemariArsip();
        if (panelBgGelap != null) panelBgGelap.SetActive(false);
        if (panelPengaturan != null) panelPengaturan.SetActive(false);
    }

    public void KlikKeluarGame()
    {
        PlaySFX(sfxKlikTombol);
        Application.Quit();
    }
}