using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryFabricationEquipmentSlot
{
    public List<BuildingSlotRegion> Factories = new List<BuildingSlotRegion>();
    public string EquipmentID;

    private float _equipmentCountFabricated;
    private CountryFabricationEquipment _countryFabrication;

    public CountryFabricationEquipmentSlot(string id, List<BuildingSlotRegion> factories, CountryFabricationEquipment countryFabrication)
    {
        _countryFabrication = countryFabrication;
        Factories = factories;
        EquipmentID = id;
    }

    public float GetEquipmentFabricationPerHour()
    {
        float eqFabrication = 0;
        foreach (var factory in Factories)
        {
            eqFabrication += (((MilitaryFactory)factory.TargetBuilding).FabricationPerHour * _countryFabrication.GetCorrectFabricationEfficiency());
        }
        var newEquipmentCount = eqFabrication / EquipmentManagerSO.GetEquipmentFromID(EquipmentID).FabricationCost;
        return newEquipmentCount;
    }

    public int Fabricate()
    {
        _equipmentCountFabricated += GetEquipmentFabricationPerHour();
        var newEquipmentInt = (int)Math.Truncate(_equipmentCountFabricated);
        _equipmentCountFabricated -= newEquipmentInt;
        return newEquipmentInt;
    }
}
