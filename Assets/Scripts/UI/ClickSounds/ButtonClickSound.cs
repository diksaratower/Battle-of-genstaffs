using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private Button _targetButton;

    private AudioSource _clickAudioSource;

    private void Start()
    {
        var soundsPlayer = FindObjectOfType<ClickSoundsPlayer>();
        if (_targetButton == null)
        {
            _targetButton = GetComponent<Button>();
        }
        //var data = Resources.Load<ButtonsClickSoundsData>("ClickSoundsData");
        //_clickAudioSource = gameObject.AddComponent<AudioSource>();
        //_clickAudioSource.playOnAwake = false;
        //_clickAudioSource.clip = data.StandartClick;
        _targetButton.onClick.AddListener(delegate 
        {
            soundsPlayer.PlayClickSound();
            //_clickAudioSource.Play();
        });
    }
}
