using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Region
{
    public readonly int MaxBuildingsCount = 40;
    [HideInInspector] public List<Province> Provinces = new List<Province>();
    public List<City> Cities = new List<City>();
    public List<BuildingSlot> Buildings = new List<BuildingSlot>();
    [HideInInspector] public City RegionCapital;
    public string Name { get; }
    public int Population;

    private Vector3 _cashedAveragePosition = Vector3.zero;

    public Region(string name, int population)
    { 
        Name = name;
        Population = population;
    }

    public static Region LoadRegion(RegionSave regionSave)
    {
        var mapRegion = new Region(regionSave.Name, regionSave.Population);
        foreach (var city in regionSave.CitiesSave)
        {
            mapRegion.Cities.Add(new City(city.Name, Map.Instance.GetProvinceById(city.ProvinceID)));
        }
        if (regionSave.CapitalID != -1)
        {
            mapRegion.RegionCapital = mapRegion.Cities[regionSave.CapitalID];
        }
        foreach (var provId in regionSave.ProvincesID)
        {
            mapRegion.Provinces.Add(Map.Instance.Provinces[provId]);
        }
        foreach (var buildingSave in regionSave.BuildingSaves)
        {
            mapRegion.Buildings.Add(buildingSave.Load(mapRegion));
        }
        return mapRegion;
    }

    public static RegionSave SaveRegion(Region mapReg)
    {
        var regSave = new RegionSave() { Name = mapReg.Name };
        regSave.Population = mapReg.Population;
        foreach (var city in mapReg.Cities)
        {
            regSave.CitiesSave.Add(new CitySave(city.Name, city.CityProvince.ID));
        }

        if (mapReg.RegionCapital != null)
        {
            regSave.CapitalID = mapReg.Cities.IndexOf(mapReg.RegionCapital);
        }
        else
        {
            regSave.CapitalID = -1;
        }

        foreach (var building in mapReg.Buildings)
        {
            regSave.BuildingSaves.Add(new RegionBuildingSave(building));
        }

        foreach (var prov in mapReg.Provinces)
        {
            regSave.ProvincesID.Add(Map.Instance.Provinces.IndexOf(prov));
        }
        return regSave;
    }

    public Country GetRegionCountry()
    {
        if (Provinces.Count == 0)
        {
            return null;
        }
        if (RegionCapital != null)
        {
            return RegionCapital.CityProvince.Owner;
        }
        return Provinces[0].Owner;
    }

    public void AnnexRegion(Country newOwner, Country lastOwner)
    {
        var teleportProvince = Map.Instance.Provinces.Find(province => province.Owner == lastOwner);
        foreach (var province in Provinces) 
        {
            var divisions = new List<Division>(province.DivisionsInProvince);
            foreach (var division in divisions)
            {
                division.TeleportDivision(teleportProvince);
            }
        }
        foreach (var province in Provinces)
        {
            province.SetOwner(newOwner);
        }
    }

    public bool IsProvinceAttachToRegion(Province province)
    {
        return Provinces.Contains(province);
    }

    public int GetAllBuildingsCount()
    {
        return Buildings.Count;
    }

    public int GetBuildingsCount(Building building)
    {
        return Buildings.FindAll(slot => slot.TargetBuilding == building).Count;
    }

    public List<BuildingSlot> GetBuildings(BuildingType buildingType)
    {
        return Buildings.FindAll(slot => slot.TargetBuilding.BuildingType == buildingType);
    }

    public void AddBuildingToRegion(Building building, Province buildingProvince = null)
    {
        Buildings.Add(new BuildingSlot(building, this, buildingProvince));
    }

    public Vector3 GetProvincesAveragePostion()
    {
        if (_cashedAveragePosition != Vector3.zero)
        {
            return _cashedAveragePosition;
        }
        var provincesPositions = new List<Vector3>();
        foreach (var province in Provinces)
        {
            provincesPositions.Add(province.Position);
        }
        _cashedAveragePosition = GetMeanVector(provincesPositions.ToArray());
        return _cashedAveragePosition;
    }

    public static List<Province> GetRegionBoard(Region region)
    {
        return region.Provinces.FindAll(prov => prov.Contacts.Exists(con => GetProvinceRegion(con) != GetProvinceRegion(prov)));
    }

    public static List<Province> GetProvincesBoard(List<Province> provinces)
    {
        if (provinces.Count == 1)
        {
            return provinces;
        }
        return provinces.FindAll(prov => prov.Contacts.Exists(con => GetProvinceRegion(con) != GetProvinceRegion(prov)));
    }

    public static Region GetProvinceRegion(Province province)
    {
        foreach (var reg in Map.Instance.MapRegions)
        {
            if (reg.Provinces.Contains(province))
            {
                return reg;
            }
        }
        return null;
    }

    private Vector3 GetMeanVector(Vector3[] positions)
    {
        if (positions.Length == 0)
            return Vector3.zero;
        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach (Vector3 pos in positions)
        {
            x += pos.x;
            y += pos.y;
            z += pos.z;
        }
        return new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
    }
}

[Serializable]
public class RegionSave
{
    public string Name;
    public int Population;
    public List<int> ProvincesID = new List<int>();
    public int CapitalID = -1;
    public List<CitySave> CitiesSave = new List<CitySave>();
    public List<RegionBuildingSave> BuildingSaves = new List<RegionBuildingSave>();
}

[Serializable]
public class RegionBuildingSave
{
    public string BuildingID;
    public int ProvinceID = -1;

    public RegionBuildingSave(BuildingSlot buildingSlot)
    {
        BuildingID = buildingSlot.TargetBuilding.ID;
        if (buildingSlot.TargetBuilding is BuildingInProvince)
        {
            ProvinceID = Map.Instance.Provinces.IndexOf(buildingSlot.Province);
        }
        else
        {
            ProvinceID = -1;
        }
    }

    public BuildingSlot Load(Region region)
    {
        Province province = null;
        if (ProvinceID != -1)
        {
            province = Map.Instance.Provinces[ProvinceID];
        }
        else
        {
            province = null;
        }
        return new BuildingSlot(BuildingsManagerSO.GetInstance().AvalibleBuildings.Find(building => building.ID == BuildingID), region, province);
    }
}

[Serializable]
public class CitySave
{
    public string Name;
    public int ProvinceID;
    public CitySave(string name, int provinceID)
    {
        Name = name;
        ProvinceID = provinceID;
    }
}

[Serializable]
public class City
{
    public string Name;
    public Province CityProvince;

    public City(string name, Province province)
    {
        Name = name;
        CityProvince = province;
    }
}

public enum CityType
{
    BigCity,
    MiddleCity,
    MicroCity
}

[Serializable]
public class BuildingSlot
{
    public Region Region { get; }
    public Province Province => GetProvince();
    public Building TargetBuilding { get; }

    private Province _province;

    public BuildingSlot(Building targetBuilding, Region region, Province province = null)
    {
        TargetBuilding = targetBuilding;
        Region = region;
        _province = province;
    }

    private Province GetProvince()
    {
        if (_province != null)
        {
            return _province;
        }
        else
        {
            return Region.RegionCapital.CityProvince;
        }
    }
}