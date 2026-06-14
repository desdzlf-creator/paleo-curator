using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject tutorPanel;
    public NPCDialogTrigger quizManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Munculin panel soal
            tutorPanel.SetActive(true);

            // Stop gerak player
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
                player.MulaiJawab();
        }
    }
}
