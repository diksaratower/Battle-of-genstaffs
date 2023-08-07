using System;
using System.Collections.Generic;

public class SupplyUnit
{
    public List<NeedEquipmentCountIdPair> EquipmentInDivision = new List<NeedEquipmentCountIdPair>();

    protected List<NeedEquipmentCountIdPair> _neededEquipment = new List<NeedEquipmentCountIdPair>(); 

    private List<SupplyRequest> _divisionSupplyRequests = new List<SupplyRequest>();
    private CountryEquipmentStorage _supplyStorage { get; }

    protected SupplyUnit(CountryEquipmentStorage supplyStorage) 
    {
        _supplyStorage = supplyStorage;
    }

    protected void CalculateSupply()
    {
        foreach (var equipmentHave in EquipmentInDivision)
        {
            var equipmentNeed = _neededEquipment.Find(slot => slot.EqType == equipmentHave.EqType);
            if ((equipmentHave.Count + GetRequestsEquiepmentCount(equipmentHave.EqType)) < equipmentNeed.Count)
            {
                var request = _supplyStorage.AddSupplyRequest(equipmentNeed.Count - equipmentHave.Count, equipmentNeed.EqType);
                _divisionSupplyRequests.Add(request);
                request.OnClosedRequest += delegate
                {
                    _divisionSupplyRequests.Remove(request);
                };
                request.OnAddedEquipment += (int AddedCount) =>
                {
                    equipmentHave.Count += AddedCount;
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
        return (be / need);
    }

    public float GetEquipmentProcent(Predicate<EquipmentType> equipmentTypePredacate)
    {
        float need = 0;
        float be = 0;
        for (int i = 0; i < EquipmentInDivision.Count; i++)
        {
            if (equipmentTypePredacate.Invoke(EquipmentInDivision[i].EqType) == true)
            {
                need += _neededEquipment[i].Count;
                be += EquipmentInDivision[i].Count;
            }
        }
        return (be / need);
    }
}
