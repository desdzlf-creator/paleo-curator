using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    [Header("Setup Level Container")]
    public Transform levelContainer;

    [Header("Daftar Level Game")]
    public List<GameObject> daftarLevelPrefab;

    private int levelSekarangIdx = 0;
    private GameObject levelAktifObj;
    private int jumlahTulangTotal = 0;
    private int jumlahTulangSukses = 0;

    [Header("UI Reward Panel Setup")]
    public GameObject panelRewardPopUp;

    [Header("Sistem Reward Satu-Satu (Baru!)")]
    [Tooltip("Masukin Objek Induk Reward per dino (Isinya Aura + Fosil yang udah lu pasin ukurannya)")]
    public GameObject[] listGroupRewardDino;

    [Header("Setup Gambar Peti")]
    public Image uiObjekPetiImage;
    public Sprite spritePetiTertutup;
    public Sprite spritePetiTerbuka;

    [Header("Setup Kertas Fun Fact")]
    public Button uiTombolBukaPeti;
    public Image uiKertasFunFact;
    [Range(0.1f, 2f)] public float durasiZoomFunFact = 0.5f;

    [Header("Tombol Baru Alur Koleksi")]
    public GameObject tombolLihatKoleksiUI;

    [Header("Efek Cinematic Setup")]
    [Range(0.1f, 3f)] public float durasiZoomIn = 1.2f;
    [Range(0.1f, 3f)] public float durasiAuraGlow = 0.8f;

    [System.Serializable]
    public class DataRewardLevel
    {
        public string namaDinosaurus;
        public Sprite spriteKertasFunFactDariFigma;
    }
    [Header("Database Reward Tiap Level")]
    public List<DataRewardLevel> databaseReward;

    [Header("Setup Panel Pause/Jeda")]
    public GameObject panelPauseMenu;
    public GameObject tombolPauseGameplay;

    [Header("Sistem Hint")]
    public HintManager hintManager;

    // ==========================================
    // SFX GAMEPLAY
    // ==========================================
    [Header("SFX Gameplay")]
    public AudioClip sfxTulangBenar;    // Tulang berhasil dikunci di posisi yang benar
    public AudioClip sfxTulangSalah;    // Tulang dilepas di posisi yang salah
    public AudioClip sfxPopupDino;      // Popup tulang dino utuh muncul
    public AudioClip sfxBukaPeti;       // Tombol buka peti diklik
    public AudioClip sfxKertasFunFact;  // Kertas reward fun fact muncul
    public AudioClip sfxKlikTombol;     // Tombol pause, resume, dll

    private void PlaySFX(AudioClip clip)
    {
        AudioManager.Instance?.PlaySFX(clip);
    }

    //game mulai, script baca data level awal
    void Start()
    {
        if (panelRewardPopUp != null) panelRewardPopUp.SetActive(false);
        MatikanSemuaGroupReward();
        if (panelPauseMenu != null) panelPauseMenu.SetActive(false);
        if (tombolPauseGameplay != null) tombolPauseGameplay.SetActive(true);

        levelSekarangIdx = PlayerPrefs.GetInt("LevelTerakhir", 0);
        MuatLevel(levelSekarangIdx);
    }

    //file prefab dipanggil
    public void MuatLevel(int index)
    {
        if (levelAktifObj != null) Destroy(levelAktifObj);

        //kalau index melebihi jumlah game = game tamat
        if (index >= daftarLevelPrefab.Count)
        {
            Debug.Log("TAMAT! Semua level dinosaurus udah beres disusun!");
            PlayerPrefs.DeleteKey("LevelTerakhir");
            SceneManager.LoadScene("Scene00_MainMenu");
            return;
        }

        levelAktifObj = Instantiate(daftarLevelPrefab[index], levelContainer);
        levelAktifObj.transform.localPosition = Vector3.zero;

        Transform trrayObj = levelAktifObj.transform.Find("Trray");
        if (trrayObj != null)
            jumlahTulangTotal = trrayObj.childCount;

        // jumlah tulang yg berhasil di susun
        jumlahTulangSukses = 0;
        levelSekarangIdx = index;
        Debug.Log("Level Baru Dimuat. Total tulang: " + jumlahTulangTotal);

        // ✅ Kasih tau HintManager level baru udah dimuat
        if (hintManager != null)
            hintManager.SetupHintUntukLevel(levelAktifObj.transform);
    }

    // kondisi menang, Dipanggil dari script drag-drop tulang saat posisi BENAR
    public void TulangBerhasilDisusun()
    {
        PlaySFX(sfxTulangBenar);
        jumlahTulangSukses++;

        // ✅ Reset timer hint setiap ada progress
        if (hintManager != null)
            hintManager.ResetTimerHint();

        if (jumlahTulangSukses >= jumlahTulangTotal)
        {
            if (hintManager != null) hintManager.ResetHint(); // Matiin hint saat level selesai
            StartCoroutine(EfekZoomDanCahayaSatuSatu());
        }
    }

    // ✅ Dipanggil dari script drag-drop tulang saat posisi SALAH
    public void TulangPosisiSalah()
    {
        PlaySFX(sfxTulangSalah);
    }

    void LanjutLevelBerikutnya()
    {
        MuatLevel(levelSekarangIdx + 1);
    }

    //jika tulang berhasil disusun
    void MunculkanFasePeti()
    {
        MatikanSemuaGroupReward();

        if (uiObjekPetiImage != null)
        {
            uiObjekPetiImage.sprite = spritePetiTertutup;
            uiObjekPetiImage.gameObject.SetActive(true);
        }

        uiTombolBukaPeti.gameObject.SetActive(true);
        uiKertasFunFact.gameObject.SetActive(false);
        if (tombolLihatKoleksiUI != null) tombolLihatKoleksiUI.SetActive(false);
    }

    public void KlikBukaPeti()
    {
        PlaySFX(sfxBukaPeti); // ✅ SFX buka peti
        if (uiObjekPetiImage != null && spritePetiTerbuka != null)
            uiObjekPetiImage.sprite = spritePetiTerbuka;

        uiTombolBukaPeti.gameObject.SetActive(false);
        StartCoroutine(DelayDanZoomFunFact());
    }

    //untuk aniamsi menampilkan funfact
    IEnumerator DelayDanZoomFunFact()
    {
        yield return new WaitForSeconds(0.6f);

        if (levelSekarangIdx < databaseReward.Count)
            uiKertasFunFact.sprite = databaseReward[levelSekarangIdx].spriteKertasFunFactDariFigma;

        uiKertasFunFact.gameObject.SetActive(true);
        if (tombolLihatKoleksiUI != null) tombolLihatKoleksiUI.SetActive(true);

        PlaySFX(sfxKertasFunFact); // ✅ SFX kertas fun fact muncul

        Transform transformFunFact = uiKertasFunFact.transform;
        transformFunFact.localScale = Vector3.zero;

        Vector3 skalaAwal = Vector3.zero;
        Vector3 skalaTarget = Vector3.one;
        float elapsed = 0f;

        while (elapsed < durasiZoomFunFact)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiZoomFunFact);
            transformFunFact.localScale = Vector3.Lerp(skalaAwal, skalaTarget, t);
            yield return null;
        }
        transformFunFact.localScale = skalaTarget;
    }

    public void KlikLihatKoleksi()
    {
        PlaySFX(sfxKlikTombol);
        panelRewardPopUp.SetActive(false);
        Time.timeScale = 1f;

        int levelBerikutnya = levelSekarangIdx + 1;
        PlayerPrefs.SetInt("LevelTerakhir", levelBerikutnya);
        PlayerPrefs.SetInt("BukaKoleksiOtomatis", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Scene00_MainMenu");
    }

    public void KlikNextLevel()
    {
        PlaySFX(sfxKlikTombol);
        panelRewardPopUp.SetActive(false);
        int levelBerikutnya = levelSekarangIdx + 1;
        PlayerPrefs.SetInt("LevelTerakhir", levelBerikutnya);
        PlayerPrefs.Save();

        LanjutLevelBerikutnya();
    }

    // ==========================================
    // LOGIKA PANEL PAUSE
    // ==========================================
    public void KlikTombolPause()
    {
        PlaySFX(sfxKlikTombol);
        if (panelPauseMenu != null)
        {
            panelPauseMenu.SetActive(true);
            if (tombolPauseGameplay != null) tombolPauseGameplay.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    public void KlikTombolResume()
    {
        PlaySFX(sfxKlikTombol);
        if (panelPauseMenu != null)
        {
            panelPauseMenu.SetActive(false);
            if (tombolPauseGameplay != null) tombolPauseGameplay.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    public void KlikTombolReset()
    {
        PlaySFX(sfxKlikTombol);
        Time.timeScale = 1f;
        if (panelPauseMenu != null) panelPauseMenu.SetActive(false);
        if (tombolPauseGameplay != null) tombolPauseGameplay.SetActive(true);
        MuatLevel(levelSekarangIdx);
    }

    public void KlikTombolMainMenu()
    {
        PlaySFX(sfxKlikTombol);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene00_MainMenu");
    }

    private void MatikanSemuaGroupReward()
    {
        if (listGroupRewardDino != null)
            foreach (GameObject go in listGroupRewardDino)
                if (go != null) go.SetActive(false);
    }

    IEnumerator EfekZoomDanCahayaSatuSatu()
    {
        PlaySFX(sfxPopupDino); // ✅ SFX popup dino utuh muncul

        panelRewardPopUp.SetActive(true);
        if (uiObjekPetiImage != null) uiObjekPetiImage.gameObject.SetActive(false);
        uiTombolBukaPeti.gameObject.SetActive(false);
        uiKertasFunFact.gameObject.SetActive(false);
        if (tombolLihatKoleksiUI != null) tombolLihatKoleksiUI.SetActive(false);

        GameObject grupAktif = null;
        CanvasGroup canvasGroupAura = null;

        if (listGroupRewardDino != null && levelSekarangIdx < listGroupRewardDino.Length)
        {
            grupAktif = listGroupRewardDino[levelSekarangIdx];
            if (grupAktif != null)
            {
                grupAktif.SetActive(true);
                canvasGroupAura = grupAktif.GetComponentInChildren<CanvasGroup>();
            }
        }

        if (canvasGroupAura != null) canvasGroupAura.alpha = 0f;

        if (grupAktif != null)
        {
            Transform objekZoom = grupAktif.transform;
            Vector3 skalaAwal = Vector3.one;
            Vector3 skalaZoom = new Vector3(1.4f, 1.4f, 1.4f);
            float elapsed = 0f;

            while (elapsed < durasiZoomIn)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiZoomIn);
                objekZoom.localScale = Vector3.Lerp(skalaAwal, skalaZoom, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < durasiAuraGlow)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiAuraGlow);
                if (canvasGroupAura != null) canvasGroupAura.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.6f);
            objekZoom.localScale = skalaAwal;
        }
        else
        {
            yield return new WaitForSeconds(durasiZoomIn + durasiAuraGlow + 0.6f);
        }

        MunculkanFasePeti();
    }
}