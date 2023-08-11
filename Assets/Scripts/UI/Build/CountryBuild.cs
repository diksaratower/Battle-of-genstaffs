using System;
using System.Collections.Generic;
using UnityEngine;


public class CountryBuild
{
    public float BaseBuildEfficiency { get; private set; } = 1f;

    public Action OnAddedBuildingToQueue;
    public Action OnRemovedBuildingFromQueue;
    public List<CountryBuildSlot> BuildingsQueue = new List<CountryBuildSlot>();

    private Country _country;

    public CountryBuild(Country country)
    {
        _country = country;
        GameTimer.HourEnd += CalculateBuild;
    }

    public void AddBuildingToBuildQueue(Building building, Region region, Province province = null)
    {
        var slot = new CountryBuildSlot(building, region, province);
        BuildingsQueue.Add(slot);
        OnAddedBuildingToQueue?.Invoke();
    }

    public bool CanAddBuildingToQueue(Building building, Region region)
    {
        var buildingsCount = (region.GetAllBuildingsCount() + BuildingsQueue.FindAll(slot => slot.Building == building).Count);
        return (buildingsCount < region.MaxBuildingsCount);
    }

    public void RemoveSlotFromBuildQueue(CountryBuildSlot buildingSlot)
    {
        if (BuildingsQueue.Exists(slot => slot == buildingSlot) == false)
        {
            throw new TryRemoveOutOfQueueBuilding();
        }
        BuildingsQueue.Remove(BuildingsQueue.Find(slot => slot == buildingSlot));
        OnRemovedBuildingFromQueue?.Invoke();
    }


    public List<BuildingSlotRegion> GetCountryBuildings(BuildingType building)
    {
        var result = new List<BuildingSlotRegion>();
        var regions = Player.CurrentCountry.GetCountryRegions();
        foreach (var region in regions)
        {
            result.AddRange(region.GetBuildings(building));
        }
        return result;
    }

    public float GetBuildEfficiency()
    {
        var efficiency = BaseBuildEfficiency + _country.Politics.GetPoliticCorrectionBuildEfficiency(BaseBuildEfficiency);
        if (efficiency < 0)
        {
            efficiency = 0;
        }
        return efficiency;
    }

    private void CalculateBuild()
    {
        var efficiencyToSlot = (GetBuildEfficiency() * (float)GetCountryBuildings(BuildingType.Factory).Count) / ((float)BuildingsQueue.Count);
        var forRemove = new List<CountryBuildSlot>();
        foreach (var slot in BuildingsQueue)
        {
            slot.BuildProgress += efficiencyToSlot;
            if (slot.IsBuildEnd() || Cheats.InstantBuilding)
            {
                forRemove.Add(slot);
                slot.EndBuild();
            }
        }
        forRemove.ForEach(slot => RemoveSlotFromBuildQueue(slot));
    }

    [Serializable]
    private class CountryBuildSave : SerializeForSave
    {

        public CountryBuildSave(CountryBuild build)
        {
           
        }

        public CountryBuildSave(string jsonSave)
        {
            JsonUtility.FromJsonOverwrite(jsonSave, this);
        }

        public override void Load(object objTarget)
        {
            var build = objTarget as CountryBuild;
            
        }

        public override string SaveToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}

public class CountryBuildSlot
{
    public Building Building { get; }
    public Region BuildRegion { get; }
    public Province BuildingProvince { get; }
    public float BuildProgress = 0;

    public CountryBuildSlot(Building building, Region buildRegion, Province buildingProvince)
    {
        Building = building;
        BuildRegion = buildRegion;
        BuildingProvince = buildingProvince;
    }

    public void EndBuild()
    {
        if (Building is BuildingInProvince)
        {
            BuildRegion.AddProvinceBuilding(Building, BuildingProvince);
        }
        else
        {
            BuildRegion.AddBuildingToRegion(Building);
        }
    }

    public bool IsBuildEnd()
    {
        return BuildProgress >= Building.BuildCost;
    }
}

public class TryRemoveOutOfQueueBuilding : Exception
{

}