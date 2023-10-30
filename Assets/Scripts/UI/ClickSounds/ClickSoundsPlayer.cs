using UnityEngine;

public class ClickSoundsPlayer : MonoBehaviour
{
    [SerializeField] private ButtonsClickSoundsData _clickSoundsData;
    
    public void PlayClickSound()
    {
        var clickAudioSource = gameObject.AddComponent<AudioSource>();
        clickAudioSource.playOnAwake = false;
        clickAudioSource.clip = _clickSoundsData.StandartClick;
        clickAudioSource.Play();
        Destroy(clickAudioSource, clickAudioSource.clip.length + 0.05f);
    }
}
