using UnityEngine;
using UnityEngine.Audio;


public class SoundSettingsLoader : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    private void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        var settingsSave = SettingsSave.LoadSettingsSave();
        _audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, settingsSave.MasterVolume));
        _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, settingsSave.MusicVolume));
    }
}
