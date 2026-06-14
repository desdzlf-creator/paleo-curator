using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("Wadah UI (Untuk Diaktifkan)")]
    public GameObject dimBackground;
    public GameObject wadahTheo;
    public GameObject wadahMary;

    [Header("UI Reference (Theo)")]
    public TextMeshProUGUI nameTextTheo;
    public TextMeshProUGUI dialogTextTheo;
    public Image imageTheo;

    [Header("UI Reference (Mary)")]
    public TextMeshProUGUI nameTextMary;
    public TextMeshProUGUI dialogTextMary;
    public Image imageMary;

    [Header("Konten Dialog")]
    public Sprite[] panelSprites;
    public string[] characterNames;
    [TextArea(3, 10)] public string[] sentences;
    public int[] expressionIndices;

    [Header("Setup Transisi Scene")]
    public SceneTransitionManager transitionManager;

    [Header("Setup Android Controller & Player")]
    public GameObject canvasGameplayUI;
    public PlayerMovement playerMovement;


    [Header("SFX Dialog")]
    public AudioClip sfxNextDialog; 

    private int index = 0;

    //mulai dialog
    public void StartDialog()
    {
        index = 0;
        if (dimBackground != null) dimBackground.SetActive(true);

        if (transitionManager != null)
            transitionManager.SetTombolSkip(true);

        if (canvasGameplayUI != null)
            canvasGameplayUI.SetActive(false);

        if (playerMovement != null)
            playerMovement.MulaiJawab();

        DisplaySentence();
    }

    //logika gantian ngomong
    public void DisplaySentence()
    {
        string currentName = characterNames[index];
        string currentSentence = sentences[index];
        int spriteIndex = expressionIndices[index];

        if (transitionManager != null)
            transitionManager.SetTombolSkip(true);

        if (currentName == "Theo")
        {
            wadahTheo.SetActive(true);
            wadahMary.SetActive(false);
            nameTextTheo.text = currentName;
            dialogTextTheo.text = currentSentence;
            if (spriteIndex >= 0 && spriteIndex < panelSprites.Length)
                imageTheo.sprite = panelSprites[spriteIndex];
        }
        else
        {
            wadahTheo.SetActive(false);
            wadahMary.SetActive(true);
            nameTextMary.text = currentName;
            dialogTextMary.text = currentSentence;
            if (spriteIndex >= 0 && spriteIndex < panelSprites.Length)
                imageMary.sprite = panelSprites[spriteIndex];
        }
    }

    //selesai dialog, diarahkan ke scene gameplay
    public void NextSentence()
    {
        // ✅ SFX klik next dialog
        AudioManager.Instance?.PlaySFX(sfxNextDialog);

        if (index < sentences.Length - 1)
        {
            index++;
            DisplaySentence();
        }
        else
        {
            Debug.Log("Dialog tamat! Menjalankan transisi...");

            if (transitionManager != null)
            {
                transitionManager.SetTombolSkip(false);
                transitionManager.KlikTombolNext();
            }
            else
            {
                if (dimBackground != null) dimBackground.SetActive(false);
                wadahTheo.SetActive(false);
                wadahMary.SetActive(false);

                if (canvasGameplayUI != null)
                    canvasGameplayUI.SetActive(true);

                if (playerMovement != null)
                    playerMovement.SelesaiJawab();

                Debug.LogWarning("Slot Transition Manager belum diisi! Player dilepas manual.");
            }
        }
    }
}