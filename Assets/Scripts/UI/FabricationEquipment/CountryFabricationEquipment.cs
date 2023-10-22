using System;
using System.Collections.Generic;
using UnityEngine;

public class CountryFabricationEquipment
{
    public Action OnAddedSlot;
    public Action OnRemovedSlot;
    public List<CountryFabricationEquipmentSlot> EquipmentSlots = new List<CountryFabricationEquipmentSlot>();

    private float _baseFabricationEfficiency = 1f;
    private Country _country;

    public CountryFabricationEquipment(Country country) 
    {
        _country = country;
        GameTimer.HourEnd += OnHourEnd;
    }

    private void OnHourEnd()
    {
        foreach (var slot in EquipmentSlots) 
        {
            var newEquipment = Mathf.RoundToInt(slot.Fabricate());
            _country.EquipmentStorage.AddEquipment(slot.EquipmentID, newEquipment);
        }
    }

    public float GetBaseFabricationEfficiency()
    {
        return _baseFabricationEfficiency;
    }

    public float GetCorrectFabricationEfficiency()
    {
        var effeciency = _baseFabricationEfficiency;
        effeciency += _country.Politics.GetPoliticCorrectionMilitaryFabrication(_baseFabricationEfficiency);
        if (_country == Player.CurrentCountry)
        {
            effeciency += Player.CurrentDifficultie.ProductionFactor;
        }
        return effeciency;
    }

    public void AddSlot(string id, List<BuildingSlotRegion> factories) 
    {
        foreach(var fac in factories) 
        {
            if(FactoryIsUses(fac))
            {
                throw new System.Exception("Factory already use");
            }
        }
        if (EquipmentSlots.Exists(sl => sl.EquipmentID == id)) return;
        EquipmentSlots.Add(new CountryFabricationEquipmentSlot(id, factories, this));
        OnAddedSlot?.Invoke();
    }

    public void RemoveSlot(CountryFabricationEquipmentSlot fabricationSlot)
    {
        EquipmentSlots.Remove(fabricationSlot);
        OnRemovedSlot?.Invoke();
    }

    public bool EquipmentIsFabricating(Equipment equipment)
    {
        return EquipmentSlots.Find(sl => sl.EquipmentID == equipment.ID) != null;
    }

    public List<BuildingSlotRegion> GetNotUseMilitaryFactories()
    {
        var factories = new List<BuildingSlotRegion>();
        foreach (var fac in _country.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory))
        {
            if (!FactoryIsUses(fac))
            {
                factories.Add(fac);
            }
        }
        return factories;
    }

    public bool FactoryIsUses(BuildingSlotRegion factory)
    {
        foreach(var slot in EquipmentSlots) 
        {
            foreach (var fac in slot.Factories)
            {
                if(fac == factory)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
