using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCountryMenu : MonoBehaviour
{
    public Action<CountrySO> OnChangedSelectedCountry;

    [SerializeField] private CountriesDataSO _countriesData;
    [SerializeField] private List<CountrySO> _majorCountries = new List<CountrySO>();
    [SerializeField] private ChooseCountryMinorViewSlot _minorCountryViewSlotPrefab;
    [SerializeField] private GridLayoutGroup _minorCountryViewsLayout;
    [SerializeField] private ChooseCountryMajorViewSlot _majorCountryViewSlotPrefab;
    [SerializeField] private GridLayoutGroup _majorCountryViewsLayout;
    [SerializeField] private ChooseCountrySelectedView _chooseCountrySelectedView;

    private CountrySO _selectedCountry;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (var country in _countriesData.Countries)
        {
            if (country.IsAvailableForPlayer == false || country.ID == "null")
            {
                continue;
            }

            if (_majorCountries.Contains(country) == true)
            {
                var slot = Instantiate(_majorCountryViewSlotPrefab, _majorCountryViewsLayout.transform);
                slot.RefreshUI(country, this);
            }
            else
            {
                var slot = Instantiate(_minorCountryViewSlotPrefab, _minorCountryViewsLayout.transform);
                slot.RefreshUI(country, this);
            }
        }
    }

    public void SetSelectedCountry(CountrySO newSelectedCountry)
    {
        _selectedCountry = newSelectedCountry;
        _chooseCountrySelectedView.gameObject.SetActive(true);
        _chooseCountrySelectedView.RefreshUI(newSelectedCountry);
        OnChangedSelectedCountry?.Invoke(newSelectedCountry);
    }
}
