using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;


    private void Start()
    {
        LoadSettings();
        _masterVolumeSlider.onValueChanged.AddListener(delegate 
        {
            _audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, _masterVolumeSlider.value));
            SaveSettings();
        });
        _musicVolumeSlider.onValueChanged.AddListener(delegate
        {
            _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, _musicVolumeSlider.value));
            SaveSettings();
        });
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    private void SaveSettings()
    {
        var settingsSave = new SettingsSave(_masterVolumeSlider.value, _musicVolumeSlider.value);
        File.WriteAllText("./Saves/settings.json", JsonUtility.ToJson(settingsSave));
    }

    private void LoadSettings()
    {
        var settingsSave = JsonUtility.FromJson<SettingsSave>(File.ReadAllText("./Saves/settings.json"));
        _masterVolumeSlider.value = settingsSave.MasterVolume;
        _musicVolumeSlider.value = settingsSave.MusicVolume;
        _audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, _masterVolumeSlider.value));
        _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, _musicVolumeSlider.value));
    }
}


public class SettingsSave
{
    public float MasterVolume = 1f;
    public float MusicVolume = 1f;

    public SettingsSave(float masterVolume, float musicVolume)
    {
        MasterVolume = masterVolume;
        MusicVolume = musicVolume;
    }
}