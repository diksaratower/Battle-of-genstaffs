using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _effectsVolumeSlider;
    [SerializeField] private Slider _cameraMoveSpeedFactor;
    [SerializeField] private TextMeshProUGUI _cameraSpeedFactorText;
    [SerializeField] private GameCamera _gameCamera;

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
        _effectsVolumeSlider.onValueChanged.AddListener(delegate
        {
            _audioMixer.SetFloat("EffectsVolume", Mathf.Lerp(-80, 0, _effectsVolumeSlider.value));
            SaveSettings();
        });
        _cameraMoveSpeedFactor.onValueChanged.AddListener(delegate
        {
            if (_gameCamera != null)//для меню
            {
                _gameCamera.CameraSpeedFactor = _cameraMoveSpeedFactor.value;
            }
            _cameraSpeedFactorText.text = Math.Round(_cameraMoveSpeedFactor.value, 2).ToString();
            SaveSettings();
        });
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    private void SaveSettings()
    {
        var settingsSave = new SettingsSave(_masterVolumeSlider.value, _musicVolumeSlider.value, _cameraMoveSpeedFactor.value);
        File.WriteAllText("./Saves/settings.json", JsonUtility.ToJson(settingsSave));
    }

    private void LoadSettings()
    {
        var settingsSave = SettingsSave.LoadSettingsSave();
        _masterVolumeSlider.value = settingsSave.MasterVolume;
        _musicVolumeSlider.value = settingsSave.MusicVolume;
        _cameraMoveSpeedFactor.value = settingsSave.CameraSpeedFactor;
        _cameraSpeedFactorText.text = Math.Round(settingsSave.CameraSpeedFactor, 2).ToString();
        _audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, _masterVolumeSlider.value));
        _audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, _musicVolumeSlider.value));
    }
}


public class SettingsSave
{
    public float MasterVolume = 1f;
    public float MusicVolume = 1f;
    public float CameraSpeedFactor = 1f;

    public SettingsSave(float masterVolume, float musicVolume, float cameraSpeedFactor)
    {
        MasterVolume = masterVolume;
        MusicVolume = musicVolume;
        CameraSpeedFactor = cameraSpeedFactor;
    }

    public static SettingsSave LoadSettingsSave()
    {
        return JsonUtility.FromJson<SettingsSave>(File.ReadAllText("./Saves/settings.json"));
    }
}