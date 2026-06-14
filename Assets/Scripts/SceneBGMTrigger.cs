using UnityEngine;

public class SceneBGMTrigger : MonoBehaviour
{
    public AudioClip bgmUntukScene;

    void Start()
    {
        if (AudioManager.Instance != null && bgmUntukScene != null)
        {
            AudioManager.Instance.PlayBGM(bgmUntukScene);
        }
    }
}