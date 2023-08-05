using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DebugConsoleUI : MonoBehaviour
{
    public Action<string> OnConsoleEnterText;

    [SerializeField] private TMP_InputField _consoleInputField;
    [SerializeField] private TextMeshProUGUI _textPrefab;
    [SerializeField] private Transform _textsParent;
    [SerializeField] private Toggle _activeProvinceChangeTool;
    [SerializeField] private ProvinceChangeTool _provinceChageTool;


    private void Start()
    {
        _activeProvinceChangeTool.onValueChanged.AddListener(delegate 
        {
            _provinceChageTool.enabled = _activeProvinceChangeTool.isOn;
        });
        OnConsoleEnterText += ProcessConsoleInput;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnConsoleEnterText?.Invoke(_consoleInputField.text);
            _consoleInputField.text = "";
        }
    }

    private void ProcessConsoleInput(string text)
    {
        if (text.StartsWith("echo"))
        {
            WriteTextToConsole(text.Remove(0, 5));
        }
        if (text.StartsWith("date"))
        {
            GameTimer.Instance.DTime = new DateTime(1936, 1, 1);
            WriteTextToConsole("Дата устовлена на стандартную");
        }
        if (text.StartsWith("annex"))
        {
            var countryID = text.Remove(0, 6);
            Map.Instance.GetCountryFromId(countryID).AnnexCountry(Player.CurrentCountry);
            WriteTextToConsole($"country {countryID} annexed");
        }
    }

    private void WriteTextToConsole(string text)
    {
        Instantiate(_textPrefab, _textsParent).text = text;
    }
}
