using System;
using UnityEngine;


public class ProvinceChangeTool : MonoBehaviour
{
    public static Action OnCitiesUpdated;

    [SerializeField] private GameIU _gameIU;

    private Province Province = null;
    private Rect _windowRect = new Rect(10, 10, 175, 210);
    private string _cityName;

    private void Update()
    {
        _gameIU.BlockDivisionSelecting = true;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (GameCamera.Instance.ChekHitToProvinceWithMousePosition(out Province province))
            {
                Province = province;
            }
        }

    }

    private void OnDisable()
    {
        _gameIU.BlockDivisionSelecting = false;
    }

    private void OnGUI()
    {
        if (Province != null)
        {
            _windowRect = GUI.Window(0, _windowRect, DoMyWindow, "Province edit");
        }
    }

    private void DoMyWindow(int windowID)
    {
        float Y = 0;
        float elementHeight = 20;
        var provinceRegion = Map.Instance.MapRegions.Find(reg => reg.Provinces.Contains(Province));
        Y += elementHeight;
        GUI.Label(new Rect(10, Y, 100, 20), "Province: " + Map.Instance.Provinces.IndexOf(Province));
        Y += elementHeight;
        GUI.Label(new Rect(10, Y, 100, 20), "Country ID: " + Province.Owner.ID);
        Y += elementHeight;
        if (GUI.Button(new Rect(10, Y, 110, 20), "Set reg. capital"))
        {
            if (provinceRegion != null)
            {
                if (provinceRegion.Cities.Find(c => c.CityProvince == Province) != null)
                {
                    provinceRegion.RegionCapital = provinceRegion.Cities.Find(c => c.CityProvince == Province);
                    OnCitiesUpdated?.Invoke();
                }
            }
        }
        Y += elementHeight;
        var provinceCityName = "null";
        if(provinceRegion != null)
        {
            if(provinceRegion.Cities.Find(c => c.CityProvince == Province) != null) 
            {
                provinceCityName = provinceRegion.Cities.Find(c => c.CityProvince == Province).Name;
            }
        }
        GUI.Label(new Rect(10, Y, 100, 20), "City:" + provinceCityName);
        Y += elementHeight;
        _cityName = GUI.TextField(new Rect(10, Y, 100, 20), _cityName);
        Y += elementHeight;
        if (GUI.Button(new Rect(10, Y, 110, 20), "Add city"))
        {
            if (provinceRegion != null)
            {
                if (provinceRegion.Cities.Find(c => c.CityProvince == Province) == null)
                {
                    provinceRegion.Cities.Add(new City(_cityName, Province));
                    OnCitiesUpdated?.Invoke();
                }
            }
        }
        var region = Map.Instance.MapRegions.Find(region => region.Provinces.Contains(Province));
        if (region != null)
        {
            Y += elementHeight;

            GUI.Label(new Rect(10, Y, 100, 20), "Region: " + region.Name);
        }
        Y += elementHeight;
        if (GUI.Button(new Rect(10, Y, 125, 20), "Set countr. capital"))
        {
            region.GetRegionCountry().CapitalRegionID = Map.Instance.MapRegions.IndexOf(region);
            OnCitiesUpdated?.Invoke();
        }
        Y += elementHeight;
        if (GUI.Button(new Rect(10, Y, 125, 20), "Remove all cities"))
        {
            foreach (var mapRegion in Map.Instance.MapRegions)
            {
                var provinceCities = mapRegion.Cities.FindAll(city => city.CityProvince == Province);
                if (provinceCities.Count > 0)
                {
                    mapRegion.Cities.RemoveAll(city => provinceCities.Contains(city));
                    if (provinceCities.Contains(mapRegion.RegionCapital))
                    {
                        mapRegion.RegionCapital = null;
                    }
                }
            }
        }
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    private void OnDrawGizmos()
    {
        if (Province != null)
        {
            Gizmos.DrawSphere(Province.Position + Vector3.up, 0.5f);
        }
    }
}
