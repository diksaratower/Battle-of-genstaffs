using System.Collections.Generic;
using UnityEngine;


public class CitiesUIViewer : MonoBehaviour
{
    [SerializeField] private CityNameUI _capitalCityUIPrefab;
    [SerializeField] private CityNameUI _justCityUIPrefab;
    [SerializeField] private Transform _cityUIParent;

    private List<CityNameUI> _citiesUI = new List<CityNameUI>();


    private void Start()
    {
        Country.OnCountryCapitulated += delegate 
        {
            RedrawCities();
        };
        Country.OnCountryAnnexed += delegate
        {
            RedrawCities();
        };
        ProvinceChangeTool.OnCitiesUpdated += delegate
        {
            RedrawCities();
        };
        RedrawCities();
    }

    private void RedrawCities()
    {
        _citiesUI.ForEach(city => { Destroy(city.gameObject); });
        _citiesUI.Clear();
        foreach (var country in Map.Instance.Countries)
        {
            var capitalRegion = country.GetCountryCapitalRegion();
            if (capitalRegion != null && capitalRegion.RegionCapital != null) 
            {
                var cityUi = Instantiate(_capitalCityUIPrefab, _cityUIParent);
                cityUi.Target = capitalRegion.RegionCapital;
                _citiesUI.Add(cityUi);
            }
        }
        foreach (var region in Map.Instance.MapRegions)
        {
            foreach (var city in region.Cities)
            {
                if (_citiesUI.Exists(cityUI => cityUI.Target == city) == false)
                {
                    var cityUi = Instantiate(_justCityUIPrefab, _cityUIParent);
                    cityUi.Target = city;
                    _citiesUI.Add(cityUi);
                }
            }
        }
    }
}
