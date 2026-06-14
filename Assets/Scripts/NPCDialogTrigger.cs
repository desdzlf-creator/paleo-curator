using UnityEngine;

public class NPCDialogTrigger : MonoBehaviour
{
    public DialogManager dialogManager;
    private bool dialogHasPlayed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player") && !dialogHasPlayed)
        {
            PlayerMovement movement = collision.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.bisaGerak = false;
            }

            if (dialogManager != null)
            {
                dialogHasPlayed = true;
                dialogManager.StartDialog();
            }
        }
    }
}