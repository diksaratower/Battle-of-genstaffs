using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CountryFabricationEquipmentSlot
{
    public List<BuildingSlotRegion> Factories = new List<BuildingSlotRegion>();
    public IFabricatable Fabricatable;

    private float _equipmentCountFabricated;
    private CountryFabricationEquipment _countryFabrication;

    public CountryFabricationEquipmentSlot(IFabricatable fabricatable, List<BuildingSlotRegion> factories, CountryFabricationEquipment countryFabrication)
    {
        _countryFabrication = countryFabrication;
        Factories = factories;
        Fabricatable = fabricatable;
    }

    public float GetEquipmentFabricationCountPerHour()
    {
        var newEquipmentCount = GetFabricationCostPerHour() / Fabricatable.FabricationCost;
        return newEquipmentCount;
    }

    public float GetFabricationPercent()
    {
        return _equipmentCountFabricated / Fabricatable.FabricationCost;
    }

    public int Fabricate()
    {
        var newEquipmenCount = 0;
        var productionCheetFactor = 1f;
        if (Cheats.InstantBuildFleet == true && (Fabricatable is ShipSO))
        {
            productionCheetFactor = 100f;
        }
        
        _equipmentCountFabricated += GetFabricationCostPerHour() * productionCheetFactor;
        if ((_equipmentCountFabricated / Fabricatable.FabricationCost) > 1)
        {
            newEquipmenCount += Mathf.RoundToInt(_equipmentCountFabricated / Fabricatable.FabricationCost);
            _equipmentCountFabricated -= (Fabricatable.FabricationCost * newEquipmenCount);
        }
        return newEquipmenCount;
    }

    private float GetFabricationCostPerHour()
    {
        float eqFabrication = 0;
        foreach (var factory in Factories)
        {
            eqFabrication += (((MilitaryFactory)factory.TargetBuilding).FabricationPerHour * _countryFabrication.GetCorrectFabricationEfficiency());
        }
        return eqFabrication;
    }
}
