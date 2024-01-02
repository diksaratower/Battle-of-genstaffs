using System;
using System.Collections.Generic;
using UnityEngine;


public class Country : MonoBehaviour
{
    public static Action<Country, InvaderCountry> OnCountryCapitulated;
    public static Action<Country, Country> OnCountryAnnexed;
    public Action<Country> OnAnnexed;
    public Action<int> OnManpowerLosses;
    public bool IsÑapitulated { get; private set; } = false;
    public float CapitulatePercent { get; private set; }
    public Action OnCapitulated;
    public CountryEquipmentStorage EquipmentStorage;
    public CountryFabricationEquipment CountryFabrication;
    public CountryPolitics Politics;
    public CountryDiplomacy CountryDiplomacy;
    public CountryDivisionTemplates Templates = new CountryDivisionTemplates();
    public CountryArmies CountryArmies = new CountryArmies();
    public CountryBuild CountryBuild;
    public CountryCreationDivisions CreationDivisions;
    public CountryResearch Research;
    public CountryFleet Fleet;
    public CountrySO CountryPreset;
    public string Name => CountryPreset.Name;
    public string ID => CountryPreset.ID;
    public Color ColorInMap => CountryPreset.ColorInMap;
    public Sprite Flag => CountryPreset.CountryFlag;
    public int CapitalRegionID;

    private const float PercentOccupyForCapitulation = 0.33f;
    private List<Province> _nationalProvinces = new List<Province>();


    private void Awake()
    {
        CountryDiplomacy = new CountryDiplomacy(this);
        EquipmentStorage = new CountryEquipmentStorage(this);
        CreationDivisions = new CountryCreationDivisions(this);
        CountryFabrication = new CountryFabricationEquipment(this);
        CountryBuild = new CountryBuild(this);
        Research = new CountryResearch();
        Fleet = new CountryFleet();
    }

    private void Start()
    {
        _nationalProvinces = Map.Instance.Provinces.FindAll(p => p.Owner == this);
        if (CountryPreset.IsAICountry == true && GetCountryRegions().Count > 0)
        {
            gameObject.AddComponent<CountryAI>();
        }
    }

    private void Update()
    {
        if (!IsÑapitulated)
        {
            if (Diplomacy.Instance.CountryIsAtWar(this) == true)
            {
                CapitulatePercent = CalculateCapitulatedPercent();
                if (CapitulatePercent >= 1f)
                {
                    Ñapitulate();
                }
            }
        }
    }

    public List<CountryEnclave> GetCountryEnclaves()
    {
        var countryProvinces = Map.Instance.Provinces.FindAll(p => p.Owner == this);
        var result = new List<CountryEnclave>();
        while (countryProvinces.Count != 0)
        {
            var bfsProvinces = FrontPlan.BFS(countryProvinces[0], countryProvinces);
            result.Add(new CountryEnclave(bfsProvinces));
            countryProvinces.RemoveAll(pr => bfsProvinces.Contains(pr));
        }
        return result;
    }

    public struct CountryEnclave
    {
        public List<Province> Provinces;

        public CountryEnclave(List<Province> provinces)
        {
            Provinces = provinces;
        }
    }

    public List<Region> GetCountryRegions()
    {
        var result = new List<Region>();
        foreach (var region in Map.Instance.MapRegions)
        {
            if (region.GetRegionCountry() == this)
            {
                result.Add(region);
            }
        }
        return result;
    }

    public void SetPreset(CountrySO country)
    {
        CountryPreset = country;
        Politics = new CountryPolitics();
        Politics.CopyData(country.Politics);
        Politics.Setup(this);
    }

    public bool CountryDivsCanFightWithDiv(Division division)
    {
        if(division.CountyOwner != this)
        {
            if(Diplomacy.Instance.GetRelationWithCountry(this, division.CountyOwner).IsWar == true)
            {
                return true;
            }
        }
        return false;
    }

    public Region GetCountryCapitalRegion()
    {
        var regions = new List<Region>(Map.Instance.MapRegions);
        if (CapitalRegionID < regions.Count && CapitalRegionID >= 0)
        {
            var region = regions[CapitalRegionID];
            if (region.RegionCapital.CityProvince.Owner == this)
            {
                return region;
            }
            return null;
        }
        return null;
    }

    public List<Province> GetAllowedProvForCountryDivisions()
    {
       return Map.Instance.Provinces.FindAll(p => ProvinceAllowedForCountryArmy(p) == true);
    }

    public bool ProvinceAllowedForCountryArmy(Province p)
    {
        return (p.Owner == this || Diplomacy.Instance.GetRelationWithCountry(this, p.Owner).IsWar == true);
    }

    public void AnnexCountry(Country annexer)
    {
        if (this == annexer)
        {
            throw new ArgumentException();
        }
        var countryDivisions = UnitsManager.Instance.Divisions.FindAll(division => division.CountyOwner == this);
        foreach (var division in countryDivisions)
        {
            division.KillDivision();
        }
        var provinces = _nationalProvinces.FindAll(p => p.Owner == this);
        foreach (var province in provinces)
        {
            province.SetOwner(annexer);
        }
        OnAnnexed?.Invoke(annexer);
        OnCountryAnnexed?.Invoke(this, annexer);
    }

    public CountrySerialize GetSerialize()
    {
        return new CountrySerialize(this);
    }

    public void LoadFromSerialize(CountrySerialize ser)
    {
        ser.Load(this);
    }

    private float CalculateCapitulatedPercent()
    {
        var occupationCount = (float)_nationalProvinces.FindAll(p => p.Owner != this).Count;
        var nationalCount = (float)_nationalProvinces.Count;
        var result = (occupationCount / nationalCount) / PercentOccupyForCapitulation;
        if (result > 1f)
        {
            result = 1f;
        }
        return result;
    }

    
    private void RecalculatePopulation()
    {
        var regions = GetCountryRegions();
        foreach (var region in regions)
        {
            region.Population = (CountryPreset.Population / regions.Count);
        }
    }

    private void Ñapitulate()
    {
        var invader = _nationalProvinces.Find(p => p.Owner != this).Owner;
        var countryDivisions = UnitsManager.Instance.Divisions.FindAll(division => division.CountyOwner == this);
        foreach (var division in countryDivisions)
        {
            division.KillDivision();
        }
        var provs = Map.Instance.Provinces.FindAll(p => p.Owner == this);
        foreach (var province in provs)
        {
            province.SetOwner(invader);
        }
        OnCapitulated?.Invoke();
        OnCountryCapitulated?.Invoke(this, new InvaderCountry(invader));
        IsÑapitulated = true;
    }

    [Serializable]
    public class CountrySerialize
    {
        public string ID;
        public string Name;
        public int MilitaryFactoriesCount;
        public int RegionCapitalID;
        public CountryDivisionTemplates.TemplateConstrSerialize CountryTemplates;
        public CountryEquipmentStorage.CountryEquipmentStorageSave EquipmentStorage;
        public CountryArmies.CountryArmiesSerialize CountryArmies;
        public CountryResearch.CountryResearchSerialize CountryResearch;
        public CountryPolitics.CountryPoliticsSerialize CountryPolitics;

        public CountrySerialize(Country country)
        {
            ID = country.ID;
            Name = country.Name;
            CountryTemplates = country.Templates.GetSerialize();
            EquipmentStorage = country.EquipmentStorage.GetSerialize();
            CountryArmies = country.CountryArmies.GetSerialize();
            CountryResearch = country.Research.GetSerialize();
            CountryPolitics = country.Politics.GetSerialize();
            RegionCapitalID = country.CapitalRegionID;
        }

        private CountrySerialize() { }

        public void Load(Country country)
        {
            if(country.ID != ID)
            {
                throw new Exception("load id not equal country id");
            }
            country.CapitalRegionID = RegionCapitalID;
            country.Templates.LoadFromSerialize(CountryTemplates);
            country.EquipmentStorage.LoadFromSerialize(EquipmentStorage);
            country.CountryArmies.LoadFromSerialize(CountryArmies);
            country.Research.LoadFromSerialize(CountryResearch);
            country.Politics.LoadFromSerialize(CountryPolitics);
        }

        public static string Save(Country country) 
        {
            var ser = new CountrySerialize(country);
            return JsonUtility.ToJson(ser);
        }
    }
}

public enum Ideology
{
    Authoritarianism,
    Democracy,
    Fascism,
    Monarchy,
    Communism,
    Anarchy
}

public enum FormOfGovernment
{
    Dictatorship,
    Democracy,
    Monarchy
}

public enum CuntryElectionsType
{
    NoElections,
    Constantly
}

public class InvaderCountry
{
    public readonly Country Country;

    public InvaderCountry(Country country)
    {
        Country = country;
    }
}