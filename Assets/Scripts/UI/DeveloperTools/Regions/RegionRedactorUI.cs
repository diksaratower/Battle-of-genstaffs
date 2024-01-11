using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RegionRedactorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _regionName;
    [SerializeField] private TextMeshProUGUI _regionOwnerText;
    [SerializeField] private TMP_InputField _regionNameChange;
    [SerializeField] private Toggle _isSelectingToggle;
    [SerializeField] private Toggle _onlyInCountryOwnerToggle;
    [SerializeField] private Slider _brushRadiusSlider;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Button _addAllCountryButton;
    [SerializeField] private TextMeshProUGUI _brushSizeValueText;
    [SerializeField] private Button _saveChangesButton;
    [SerializeField] private Toggle _excluseProvinceWithRegionToggle;
    [SerializeField] private bool _drawInside;
    [SerializeField] private BrushForDeveloperTools _brush = new BrushForDeveloperTools(3f);

    private Region _region;
    private List<Province> _provinces = new List<Province>();


    private void Update()
    {
        if (_isSelectingToggle.isOn)
        {
            _brush.UpdateBrushPosition();
            var newProvinces = _brush.GetBrushProvinces(true);
            if (Input.GetKey(KeyCode.Mouse1) && !Input.GetKey(KeyCode.LeftShift))
            {
                AddProvinces(newProvinces);
            }
            if (Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftShift))
            {
                RemoveProvinces(newProvinces);
            }
        }
        var country = _region.GetRegionCountry();
        if (country != null)
        {
            _regionOwnerText.text = "Владелец: " + country.Name;
        }
        else
        {
            _regionOwnerText.text = "Владелец: " + "null";
        }

        _brush.BrushSize = _brushRadiusSlider.value;
        _brushSizeValueText.text = Math.Round(_brushRadiusSlider.value, 2).ToString();
        _regionName.text = _region.Name + " " + Map.Instance.MapRegions.IndexOf(_region);
    }

    public void RefreshUI(Region region)
    {
        _region = region;
        _regionNameChange.text = region.Name;
        _regionNameChange.onValueChanged.RemoveAllListeners();
        _regionNameChange.onValueChanged.AddListener(delegate 
        {
            _region.ChangeName(_regionNameChange.text);
        });
        SetUpButtons();
    }

    private void OnDrawGizmos()
    {
        if (_region != null)
        {
            foreach (var province in _region.Provinces)
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawSphere(province.Position + Vector3.up, 0.5f);
            }
        }
        if (_isSelectingToggle.isOn)
        {
            Gizmos.color = Color.red;
            foreach (var province in _provinces)
            {
                Gizmos.DrawSphere(province.Position + Vector3.up, 0.5f);
            }
            _brush.DrawBrushGizmos();
        }
    }

    private void SetUpButtons()
    {
        _clearButton.onClick.RemoveAllListeners();
        _clearButton.onClick.AddListener(delegate
        {
            _provinces.Clear();
        });
        _addAllCountryButton.onClick.RemoveAllListeners();
        _addAllCountryButton.onClick.AddListener(delegate
        {
            _provinces = new List<Province>();
            var country = _region.GetRegionCountry();
            var countryProvinces = Map.Instance.Provinces.FindAll(province => province.Owner == country);
            _provinces.AddRange(countryProvinces);
        });
        _saveChangesButton.onClick.RemoveAllListeners();
        _saveChangesButton.onClick.AddListener(delegate
        {
            SaveToRegion();
        });
    }

    private void SaveToRegion()
    {
        if (_region != null)
        {
            foreach (var province in _provinces)
            {
                var regions = Map.Instance.MapRegions.FindAll(reg => reg.Provinces.Contains(province));//на всякий 
                foreach (var region in regions)
                {
                    region.Provinces.Remove(province);
                }
            }
            foreach (var province in _provinces)
            {
                if (_region.Provinces.Contains(province) == false)
                {
                    _region.Provinces.Add(province);
                }
            }
        }
        _provinces.Clear();
    }

    private void AddProvinces(List<Province> newProvinces)
    {
        foreach (var province in newProvinces)
        {
            if (_excluseProvinceWithRegionToggle.isOn)
            {
                if (Map.Instance.MapRegions.Exists(reg => reg.Provinces.Contains(province)))
                {
                    continue;
                }
            }
            if (_onlyInCountryOwnerToggle.isOn)
            {
                var country = _region.GetRegionCountry();
                if (country != null)
                {
                    if (province.Owner != country)
                    {
                        continue;
                    }
                }
            }
            if (_provinces.Contains(province) == false)
            {
                _provinces.Add(province);
            }
        }
    }

    private void RemoveProvinces(List<Province> provinces)
    {
        foreach (var province in provinces)
        {
            if (_provinces.Contains(province) == true)
            {
                _provinces.Remove(province);
            }
        }
    }
}
