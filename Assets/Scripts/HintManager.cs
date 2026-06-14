using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [Header("Pengaturan Timer")]
    public float waktuTungguHint = 10f;
    public float durasiHintAktif = 10f;

    [Header("Efek Visual Hint")]
    public Color warnaGlow = new Color(1f, 0.92f, 0.016f, 1f); // Kuning
    [Range(0.5f, 5f)] public float kecepatanPulse = 2f;

    private List<Image> daftarSlotBenar = new List<Image>();

    private Dictionary<Image, Color> warnaAsliSlot = new Dictionary<Image, Color>();

    private Coroutine coroutineTimer;
    private Coroutine coroutineHint;
    private bool hintSedangAktif = false;

    //mendeteksi slot target 
    public void SetupHintUntukLevel(Transform levelContainer)
    {
        ResetHint();
        daftarSlotBenar.Clear();
        warnaAsliSlot.Clear();

        SlotTulang[] semuaSlot = levelContainer.GetComponentsInChildren<SlotTulang>();
        foreach (SlotTulang slot in semuaSlot)
        {
            Image imgSlot = slot.GetComponent<Image>();
            if (imgSlot != null)
            {
                daftarSlotBenar.Add(imgSlot);
                warnaAsliSlot[imgSlot] = imgSlot.color; 
            }
        }

        Debug.Log($"[HintManager] Setup selesai. Ditemukan {daftarSlotBenar.Count} slot.");
        MulaiTimerHint();
    }

    public void ResetTimerHint()
    {
        hintSedangAktif = false;
        if (coroutineHint != null) StopCoroutine(coroutineHint);
        MatikanSemuaGlow();
        MulaiTimerHint();
    }

    public void ResetHint()
    {
        hintSedangAktif = false;
        if (coroutineTimer != null) StopCoroutine(coroutineTimer);
        if (coroutineHint != null) StopCoroutine(coroutineHint);
        MatikanSemuaGlow();
    }

    private void MulaiTimerHint()
    {
        if (coroutineTimer != null) StopCoroutine(coroutineTimer);
        coroutineTimer = StartCoroutine(TimerSebelumHint());
    }

    //menunggu waktu aktif (10s)
    IEnumerator TimerSebelumHint()
    {
        yield return new WaitForSeconds(waktuTungguHint);
        AktifkanHint();
    }

    private void AktifkanHint()
    {
        if (daftarSlotBenar.Count == 0) return;
        hintSedangAktif = true;
        Debug.Log("[HintManager] Hint aktif!");

        if (coroutineHint != null) StopCoroutine(coroutineHint);
        coroutineHint = StartCoroutine(EfekGlowDanAutoMati());
    }

    IEnumerator EfekGlowDanAutoMati()
    {
        float elapsed = 0f;

        while (elapsed < durasiHintAktif && hintSedangAktif)
        {
            float t = Mathf.PingPong(Time.time * kecepatanPulse, 1f);

            foreach (Image slot in daftarSlotBenar)
            {
                if (slot == null) continue;

                Color asli = warnaAsliSlot.ContainsKey(slot) ? warnaAsliSlot[slot] : Color.white;
                slot.color = Color.Lerp(asli, warnaGlow, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        MatikanSemuaGlow();
        hintSedangAktif = false;
        Debug.Log("[HintManager] Hint selesai, timer mulai lagi.");
        MulaiTimerHint();
    }

    private void MatikanSemuaGlow()
    {
        foreach (Image slot in daftarSlotBenar)
        {
            if (slot == null) continue;
            if (warnaAsliSlot.ContainsKey(slot))
                slot.color = warnaAsliSlot[slot];
        }
    }
}