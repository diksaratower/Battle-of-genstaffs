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
echo - ���
date - ��������� �������. ����
annex - �������������
delete divs - ������� ������� 
manpower - ������� ��������
prom - ��������������
pp_all_zero - ���� ����� ������ �� ����
");
        }
        if (text.StartsWith("echo"))
        {
            WriteTextToConsole(text.Remove(0, 5));
        }
        if (text.StartsWith("date"))
        {
            GameTimer.Instance.DTime = new DateTime(1936, 1, 1);
            WriteTextToConsole("���� ��������� �� �����������");
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
            WriteTextToConsole("�������� ���� ������������.");
        }
        if (text.StartsWith("rpClear"))
        {
            foreach (var country in Map.Instance.Countries)
            {
                country.Research.ResearchPointCount = 0;
            }
            WriteTextToConsole("�������� ���� ������������.");
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
            WriteTextToConsole("���������� ������� �� ������");
        }
        if (text == "manpower")
        {
            foreach (var country in Map.Instance.Countries)
            {
                country.EquipmentStorage.SetManpowerCount(Mathf.RoundToInt(((float)country.CountryPreset.Population / 100f) * country.Politics.GetConscriptionPercent()));
            }
            WriteTextToConsole("��������� ����.");
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
            WriteTextToConsole("��������� ��������� � �������.");
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
            WriteTextToConsole("��������� ������");
        }
        if (text == "pp_all_zero")
        {
            foreach(var country in Map.Instance.Countries)
            {
                country.Politics.PolitPower = 0f;
            }
            WriteTextToConsole("� ���� ����� �������� �������");
        }
        if (text == "event")
        {
            var panel = FindObjectOfType<EventsViewUI>().ViewNewsEvent("����� ������ �� ������� ��� �������� ���� �������.");
            panel.AddCloseButton("�������").CloseButton.onClick.AddListener(delegate
            {
                panel.gameObject.SetActive(false);
            });
            WriteTextToConsole("��������� �������� �����");
        }
        if (text == "marine_reg")
        {
            Map.Instance.MarineRegions.RecalculateMarineRegions();
            WriteTextToConsole("���������� ������� �������.");
        }
        if (text == "view_sea")
        {
            Map.Instance.MarineRegions.ViewSelectedRegionProvinces = (!Map.Instance.MarineRegions.ViewSelectedRegionProvinces);
            WriteTextToConsole($"�������� ������� {Map.Instance.MarineRegions.ViewSelectedRegionProvinces}.");
        }
        if (text == "view_sea_cont")
        {
            Map.Instance.MarineRegions.ViewSelectedRegionContacts = (!Map.Instance.MarineRegions.ViewSelectedRegionContacts);
            WriteTextToConsole($"�������� ��������� ����� {Map.Instance.MarineRegions.ViewSelectedRegionContacts}.");
        }
    }

    private void WriteTextToConsole(string text)
    {
        Instantiate(_textPrefab, _textsParent).text = text;
    }
}
