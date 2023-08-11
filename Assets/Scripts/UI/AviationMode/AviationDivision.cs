using System;
using UnityEngine;


public class AviationDivision : SupplyUnit
{
    public BuildingSlotRegion PositionAviabase { get; private set; }
    public Country CountryOwner { get; }
    public string Name { get; }
    public float AttackDistance { get; }


    public AviationDivision(BuildingSlotRegion positionAviabase, Country countryOwner, string name) : base(countryOwner.EquipmentStorage)
    {
        PositionAviabase = positionAviabase;
        CountryOwner = countryOwner;
        Name = name;
        AttackDistance = 87f;
        _neededEquipment.Add(new NeedEquipmentCountIdPair(EquipmentType.Fighter, 100));
        _neededEquipment.Add(new NeedEquipmentCountIdPair(EquipmentType.Manpower, 200));
        foreach (var needEquiepment in _neededEquipment)
        {
            EquipmentInDivision.Add(new NeedEquipmentCountIdPair(needEquiepment.EqType, 0));
        }
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
}
