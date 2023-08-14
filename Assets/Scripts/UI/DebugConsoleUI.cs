using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
        if (text == "delete divs")
        {
            var forRemove = new List<Division>();
            foreach (var division in UnitsManager.Instance.Divisions)
            {
                if (division.CountyOwner != Player.CurrentCountry)
                {
                    forRemove.Add(division);
                }
            }
            foreach (var division in forRemove)
            {
                division.KillDivision();
            }
            WriteTextToConsole("Уничтожены дивизии не игрока");
        }
        if (text == "manpower")
        {
            foreach (var country in Map.Instance.Countries)
            {
                country.EquipmentStorage.SetManpowerCount(Mathf.RoundToInt(((float)country.CountryPreset.Population / 100f) * country.Politics.GetConscriptionPercent()));
            }
            WriteTextToConsole("Добавлены люди.");
        }
        if (text == "prom")
        {
            var factory = BuildingsManagerSO.GetInstance().AvalibleBuildings.Find(building => building.BuildingType == BuildingType.Factory);
            var militaryFactory = BuildingsManagerSO.GetInstance().AvalibleBuildings.Find(building => building.BuildingType == BuildingType.MilitaryFactory);
            var random = new System.Random();
            foreach (var region in Map.Instance.MapRegions)
            {
                if (region.GetAllBuildingsCount() == 0)
                {
                    region.AddBuildingToRegion(factory);
                    for (int i = 0; i < random.Next(0, 4); i++)
                    {
                        region.AddBuildingToRegion(militaryFactory);
                    }
                }
            }
            WriteTextToConsole("Добавлена промка");
        }
    }

    private void WriteTextToConsole(string text)
    {
        Instantiate(_textPrefab, _textsParent).text = text;
    }
}
