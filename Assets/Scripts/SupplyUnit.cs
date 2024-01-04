using System;
using System.Collections.Generic;
using System.Linq;

public class SupplyUnit
{
    public Action OnGetSupply;
    public List<EquipmentCountIdPair> EquipmentInDivision = new List<EquipmentCountIdPair>();

    protected List<TypedEquipmentCountIdPair> _neededEquipment = new List<TypedEquipmentCountIdPair>(); 
    

    private List<SupplyRequest> _divisionSupplyRequests = new List<SupplyRequest>();
    private CountryEquipmentStorage _supplyStorage { get; }

    protected SupplyUnit(CountryEquipmentStorage supplyStorage) 
    {
        _supplyStorage = supplyStorage;
    }

    protected void CalculateSupply()
    {
        var typesList = Enum.GetValues(typeof(EquipmentType)).Cast<EquipmentType>().ToList();
        foreach (var eqType in typesList)
        {
            var equipmentNeed = _neededEquipment.Find(slot => slot.EqType == eqType);
            if (equipmentNeed == null)
            {
                continue;
            }
            if ((GetHaveCountWithType(eqType) + GetRequestsEquiepmentCount(eqType)) < equipmentNeed.Count)
            {
                var request = _supplyStorage.AddSupplyRequest(equipmentNeed.Count - GetHaveCountWithType(eqType), equipmentNeed.EqType);
                _divisionSupplyRequests.Add(request);
                request.OnClosedRequest += delegate
                {
                    _divisionSupplyRequests.Remove(request);
                };
                request.OnAddedEquipment += (List<EquipmentCountIdPair> countIdPairs) =>
                {
                    foreach (var pair in countIdPairs)
                    {
                        AddEquipment(pair);
                    }
                    OnGetSupply?.Invoke();
                };
            }
        }
        
    }

    protected void StopSupply()
    {
        var requestToClose = new List<SupplyRequest>(_divisionSupplyRequests);
        foreach (var request in requestToClose)
        {
            request.OnClosedRequest?.Invoke();
        }
    }

    private void AddEquipment(EquipmentCountIdPair countIdPair)
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

    private int GetRequestsEquiepmentCount(EquipmentType equipmentType)
    {
        var requests = _divisionSupplyRequests.FindAll(request => request.EquipmentType == equipmentType);
        var result = 0;
        foreach (var request in requests)
        {
            result += request.EquipmentCount;
        }
        return result;
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
}
