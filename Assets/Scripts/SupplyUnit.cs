using System;
using System.Collections.Generic;
using System.Linq;

public class SupplyUnit
{
    public Action OnGetSupply;
    public List<EquipmentCountIdPair> EquipmentInDivision = new List<EquipmentCountIdPair>();

    protected List<TypedEquipmentCountIdPair> _neededEquipment = new List<TypedEquipmentCountIdPair>(); 
    
    private CountryEquipmentStorage _supplyStorage { get; }

    protected SupplyUnit(CountryEquipmentStorage supplyStorage) 
    {
        _supplyStorage = supplyStorage;
    }

    public int GetDefficit(EquipmentType eqType)
    {
        if (_neededEquipment.Exists(pair => pair.EqType == eqType) == false)
        {
            return 0;
        }
        var defficit = GetHaveCountWithType(eqType) - _neededEquipment.Find(pair => pair.EqType == eqType).Count;
        if (defficit > 0)
        {
            throw new Exception("Defficit many 0");
        }
        return defficit;
    }

    protected void StopSupply()
    {
        
    }

    public void AddEquipment(EquipmentCountIdPair countIdPair)
    {
        if (EquipmentInDivision.Find(eq => eq.Equipment == countIdPair.Equipment) == null)
        {
            EquipmentInDivision.Add(new EquipmentCountIdPair(countIdPair.Equipment, countIdPair.Count));
            return;
        }
        else
        {
            EquipmentInDivision.Find(eq => eq.Equipment == countIdPair.Equipment).Count += countIdPair.Count;
        }
    }

    private int GetHaveCountWithType(EquipmentType equipmentType)
    {
        var count = 0;
        foreach (var equipmentSlot in EquipmentInDivision)
        {
            if (equipmentSlot.Equipment.EqType == equipmentType)
            {
                count += equipmentSlot.Count;
            }
        }
        return count;
    }

    private List<TypedEquipmentCountIdPair> GetTypedEquipmentCount(List<EquipmentCountIdPair> simpleEquipment)
    {
        var list = new List<TypedEquipmentCountIdPair>();
        var types = Enum.GetValues(typeof(EquipmentType));
        for (int i = 0; i < types.Length; i++)
        {
            int eqNeedCount = 0;

            foreach (var equipment in simpleEquipment)
            {
                if (equipment.Equipment.EqType == (EquipmentType)types.GetValue(i))
                {
                    eqNeedCount += equipment.Count;
                }
            }

            if (eqNeedCount > 0)
            {
                var typePair = new TypedEquipmentCountIdPair((EquipmentType)types.GetValue(i), eqNeedCount);
                list.Add(typePair);
            }
        }
        return list;
    }

    protected void ReturnEquipmentToStorage()
    {
        foreach (var equipment in EquipmentInDivision)
        {
            _supplyStorage.AddEquipment(equipment.Equipment.ID, equipment.Count);
        }
    }

    public float GetEquipmentProcent()
    {
        float need = 0;
        float be = 0;
        for (int i = 0; i < EquipmentInDivision.Count; i++)
        {
            need += _neededEquipment[i].Count;
            be += EquipmentInDivision[i].Count;
        }
        if (need == 0)
        {
            return 0;
        }
        return (be / need);
    }

    public float GetEquipmentProcent(Predicate<EquipmentType> equipmentTypePredacate)
    {
        var typedCounts = GetTypedEquipmentCount(EquipmentInDivision);
        float need = 0;
        float be = 0;
        for (int i = 0; i < typedCounts.Count; i++)
        {
            if (equipmentTypePredacate.Invoke(typedCounts[i].EqType) == true)
            {
                need += _neededEquipment[i].Count;
                be += typedCounts[i].Count;
            }
        }
        if (need == 0)
        {
            return 0;
        }
        return (be / need);
    }

    public float GetBattleStrengh()
    {
        var equipmentPercent = GetEquipmentProcent(eqType => eqType != EquipmentType.Manpower);
        var manpowerPercent = GetEquipmentProcent(eqType => eqType == EquipmentType.Manpower);
        return equipmentPercent * manpowerPercent;
    }
}
