using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

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
            WriteTextToConsole("user: " + _consoleInputField.text);
            OnConsoleEnterText?.Invoke(_consoleInputField.text);
            _consoleInputField.text = "";
        }
    }

    private void ProcessConsoleInput(string text)
    {
        if (text.StartsWith("help"))
        {
            WriteTextToConsole(@"
echo - эхо
date - поставить стандрт. дату
annex - аннексировать
delete divs - удалить дивизию 
manpower - чуваков нарожать
prom - промышленность
pp_all_zero - всем полит власть на ноль
");
        }
        if (text.StartsWith("echo"))
        {
            WriteTextToConsole(text.Remove(0, 5));
        }
        if (text.StartsWith("date"))
        {
            GameTimer.Instance.DTime = new DateTime(1936, 1, 1);
            WriteTextToConsole("Дата устовлена на стандартную");
        }
        if (text.StartsWith("rp"))
        {
            if ("rp".Length == 2)
            {
                Player.CurrentCountry.Research.ResearchPointCount += 10000;
            }
            else
            {
                Player.CurrentCountry.Research.ResearchPointCount += int.Parse(text.Remove(0, 3));
            }
            WriteTextToConsole("Добавили очки исследований.");
        }
        if (text.StartsWith("rpClear"))
        {
            foreach (var country in Map.Instance.Countries)
            {
                country.Research.ResearchPointCount = 0;
            }
            WriteTextToConsole("Добавили очки исследований.");
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
        if (text == "divisions")
        {
            foreach (var division in UnitsManager.Instance.Divisions)
            {
                var r = new System.Random();
                if (division.GetEquipmentProcent(equipment => equipment == EquipmentType.Rifle) < 0.15f)
                {
                    var coof = (float)r.Next(40, 50) / 100f; 
                    var count = Mathf.RoundToInt((float)Math.Abs(division.GetDefficit(EquipmentType.Rifle)) * coof);
                    division.CountyOwner.EquipmentStorage.AddEquipment("ww1_rifle_equipment", count);
                }
            }
            WriteTextToConsole("Добавлено оснащение в дивизии.");
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
        if (text == "pp_all_zero")
        {
            foreach(var country in Map.Instance.Countries)
            {
                country.Politics.PolitPower = 0f;
            }
            WriteTextToConsole("У всех стран обнулена политка");
        }
        if (text == "event")
        {
            var panel = FindObjectOfType<EventsViewUI>().ViewNewsEvent("Ивент вызван из консоли для проверки этой системы.");
            panel.AddCloseButton("Закрыть").CloseButton.onClick.AddListener(delegate
            {
                panel.gameObject.SetActive(false);
            });
            WriteTextToConsole("Отображен тестовый ивент");
        }
        if (text == "marine_reg")
        {
            Map.Instance.MarineRegions.RecalculateMarineRegions();
            WriteTextToConsole("Рассчитаны морские регионы.");
        }
        if (text == "view_sea")
        {
            Map.Instance.MarineRegions.ViewSelectedRegionProvinces = (!Map.Instance.MarineRegions.ViewSelectedRegionProvinces);
            WriteTextToConsole($"Просмотр берегов {Map.Instance.MarineRegions.ViewSelectedRegionProvinces}.");
        }
        if (text == "view_sea_cont")
        {
            Map.Instance.MarineRegions.ViewSelectedRegionContacts = (!Map.Instance.MarineRegions.ViewSelectedRegionContacts);
            WriteTextToConsole($"Просмотр контактов морей {Map.Instance.MarineRegions.ViewSelectedRegionContacts}.");
        }
    }

    private void WriteTextToConsole(string text)
    {
        Instantiate(_textPrefab, _textsParent).text = text;
    }
}
