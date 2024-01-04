using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AviationDivision : SupplyUnit
{
    public BuildingSlotRegion PositionAviabase { get; private set; }
    public Country CountryOwner { get; }
    public string Name { get; }
    public float AttackDistance => CalculateAttackDistance();


    public AviationDivision(BuildingSlotRegion positionAviabase, Country countryOwner, string name) : base(countryOwner.EquipmentStorage)
    {
        PositionAviabase = positionAviabase;
        CountryOwner = countryOwner;
        Name = name;

        _neededEquipment.Add(new TypedEquipmentCountIdPair(EquipmentType.Fighter, 100));
        _neededEquipment.Add(new TypedEquipmentCountIdPair(EquipmentType.Manpower, 200));
        
        GameTimer.HourEnd += CalculateSupply;
    }

    public void Move(BuildingSlotRegion newAviabase)
    {
        if (newAviabase == PositionAviabase)
        {
            return;
        }
        if (newAviabase.TargetBuilding.BuildingType != BuildingType.Airbase)
        {
            throw new ArgumentException();
        }
        PositionAviabase = newAviabase;
    }

    public bool CanHelp(Division division, out float bonusPercent)
    {
        if (Vector3.Distance(division.DivisionProvince.Position, PositionAviabase.Region.GetProvincesAveragePostion()) < AttackDistance &&
            division.CountyOwner == CountryOwner)
        {
            bonusPercent = 0.2f * GetEquipmentProcent(equipmentType => equipmentType != EquipmentType.Manpower) * 
                GetEquipmentProcent(equipmentType => equipmentType == EquipmentType.Manpower);
            return true;
        }
        bonusPercent = 0;
        return false;
    }

    public AirplaneEquipment GetAverageAirplane()
    {
        if (EquipmentInDivision.Count == 0)
        {
            throw new Exception("Airplanes is count 0.");
        }
        var types = new List<string>();
        foreach (var pair in EquipmentInDivision)
        {
            if (types.Contains(pair.Equipment.ID) == false)
            {
                types.Add(pair.Equipment.ID);
            }
        }
        var counts = new List<int>();
        foreach (var type in types)
        {
            counts.Add(EquipmentInDivision.FindAll(equipmentPair => equipmentPair.Equipment.ID == type).Count);
        }
        var max = counts.Max();
        foreach (var type in types)
        {
            if (EquipmentInDivision.FindAll(equipmentPair => equipmentPair.Equipment.ID == type).Count == max)
            {
                return (EquipmentInDivision.Find(equipmentPair => equipmentPair.Equipment.ID == type).Equipment as AirplaneEquipment);
            }
        }
        throw new Exception("Error in get averge airplane.");
    }

    private float CalculateAttackDistance()
    {
        if (EquipmentInDivision.Count > 0)
        {
            var distance = GetAverageAirplane().AttackDistance;
            return distance;
        }
        else
        {
            return 0f; 
        }
    }
}
