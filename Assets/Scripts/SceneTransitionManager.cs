using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Setup Animasi")]
    public Animator fadeAnimator;
    public string namaAnimasiFadeOut = "FadeOut";
    public string namaAnimasiFadeIn = "FadeIn";

    [Header("Nama Scene Tujuan")]
    public string namaSceneTujuan;

    [Header("Opsi Transisi")]
    public bool pakaiAnimasiFadeOut = true;
    public bool pakaiAnimasiFadeIn = true;

    [Header("Opsi Buka Panel")]
    public string namaPanel = ""; 

    [Header("Setup Tombol Skip (Baru)")]
    public GameObject tombolSkipDialog; 

    private void Start()
    {
        if (tombolSkipDialog != null)
        {
            tombolSkipDialog.SetActive(false);
        }

        if (pakaiAnimasiFadeIn)
            StartCoroutine(ProsesFadeIn());
    }

    // =========================================================================
    // 🔥 SAKLAR OTOMATIS: PANGGUNG DIALOG NYALA = TOMBOL NYALA, DIALOG MATI = TOMBOL MATI
    // =========================================================================
    public void SetTombolSkip(bool status)
    {
        if (tombolSkipDialog != null)
        {
            tombolSkipDialog.SetActive(status);
            Debug.Log("Saklar Tombol Skip diubah menjadi: " + status);
        }
    }

    public void KlikTombolNext()
    {
        StartCoroutine(ProsesPindahScene());
    }

    public void KlikDenganBukaPanel()
    {
        if (namaPanel != "")
        {
            PlayerPrefs.SetString("BukaPanel", namaPanel);
            PlayerPrefs.Save();
        }
        StartCoroutine(ProsesPindahScene());
    }

    public void KlikSkipDialog()
    {
        Debug.Log("DIALOG DI-SKIP! Langsung memicu transisi ke scene: " + namaSceneTujuan);
        Time.timeScale = 1f;

        if (tombolSkipDialog != null)
        {
            tombolSkipDialog.SetActive(false);
        }

        StartCoroutine(ProsesPindahScene());
    }

    public IEnumerator ProsesFadeIn()
    {
        if (fadeAnimator != null)
        {
            if (!fadeAnimator.gameObject.activeSelf)
                fadeAnimator.gameObject.SetActive(true);

            fadeAnimator.Play(namaAnimasiFadeIn);
            yield return null;

            float durasi = fadeAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(durasi);

            fadeAnimator.gameObject.SetActive(false);
        }
    }

    private IEnumerator ProsesPindahScene()
    {
        if (pakaiAnimasiFadeOut && fadeAnimator != null)
        {
            if (!fadeAnimator.gameObject.activeSelf)
                fadeAnimator.gameObject.SetActive(true);

            fadeAnimator.Play(namaAnimasiFadeOut);
            yield return null;

            float durasi = fadeAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(durasi);
        }
        SceneManager.LoadScene(namaSceneTujuan);
    }
}