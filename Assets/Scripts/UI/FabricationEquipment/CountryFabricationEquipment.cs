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

    public float GetCorrectFabricationEfficiency()
    {
        return _baseFabricationEfficiency + _country.Politics.GetPoliticCorrectionMilitaryFabrication(_baseFabricationEfficiency);
    }

    public void AddSlot(string id, List<BuildingSlot> factories) 
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

    public List<BuildingSlot> GetNotUseMilitaryFactories()
    {
        var factories = new List<BuildingSlot>();
        foreach (var fac in _country.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory))
        {
            if (!FactoryIsUses(fac))
            {
                factories.Add(fac);
            }
        }
        return factories;
    }

    public bool FactoryIsUses(BuildingSlot factory)
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
