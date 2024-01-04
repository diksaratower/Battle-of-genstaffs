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
            var newEquipmentCount = slot.Fabricate();
            if (slot.Fabricatable is Equipment)
            {
                _country.EquipmentStorage.AddEquipment((slot.Fabricatable as Equipment).ID, newEquipmentCount);
            }
            if (slot.Fabricatable is ShipSO)
            {
                for (int i = 0; i < newEquipmentCount; i++)
                {
                    Map.Instance.MarineRegions.AddShip((slot.Fabricatable as ShipSO).CreateShip(_country));
                }
            }
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

    public void AddSlot(IFabricatable fabricatable, List<BuildingSlotRegion> factories) 
    {
        foreach(var factory in factories) 
        {
            if(FactoryIsUses(factory))
            {
                throw new System.Exception("Factory already use");
            }
        }
        if (EquipmentSlots.Exists(sl => sl.Fabricatable == fabricatable)) return;
        EquipmentSlots.Add(new CountryFabricationEquipmentSlot(fabricatable, factories, this));
        OnAddedSlot?.Invoke();
    }

    public void RemoveSlot(CountryFabricationEquipmentSlot fabricationSlot)
    {
        EquipmentSlots.Remove(fabricationSlot);
        OnRemovedSlot?.Invoke();
    }

    public bool EquipmentIsFabricating(IFabricatable fabricatble)
    {
        return EquipmentSlots.Find(sl => sl.Fabricatable == fabricatble) != null;
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


public interface IFabricatable
{
    public float FabricationCost { get; }
    public string ID { get; }
    public string Name { get; }
    public Sprite ItemImage { get; }
}