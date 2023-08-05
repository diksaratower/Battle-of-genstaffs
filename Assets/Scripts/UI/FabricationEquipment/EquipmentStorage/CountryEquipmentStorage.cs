using System;
using System.Collections.Generic;
using UnityEngine;


public class CountryEquipmentStorage
{
    private List<CountryEquipmentStorageSlot> _equipmentSlots = new List<CountryEquipmentStorageSlot>();
    private List<SupplyRequest> _supplyRequests = new List<SupplyRequest>();

    public CountryEquipmentStorage(Country country)
    {
        GameTimer.HourEnd += CalculateSupply;
    }

    private void CalculateSupply()
    {
        foreach (var request in _supplyRequests)
        {
            RemoveHowMuchIsAvailable(request.EquipmentCount, request.EquipmentType, out var getedCount);
            if (getedCount == 0)
            {
                continue;
            }
            request.EquipmentCount -= getedCount;
            request.OnAddedEquipment?.Invoke(getedCount);
            if (request.EquipmentCount <= 0)
            {
                request.OnClosedRequest?.Invoke();
                continue;
            }
        }
        _supplyRequests.RemoveAll(request => (request.RequestClosed == true));
    }

    public SupplyRequest AddSupplyRequest(int equipmentCount, EquipmentType equipmentType)
    {
        var request = new SupplyRequest(equipmentCount, equipmentType);
        _supplyRequests.Add(request);
        return request;
    }

    public int GetEquipmentCountWithDeficit(EquipmentType equipmentType)
    {
        var equipmentCount = GetEquipmentCount(equipmentType);
        equipmentCount -= GetEquipmentDeficit(equipmentType);
        return equipmentCount;
    }

    public int GetEquipmentCount(string id)
    {
        if (_equipmentSlots.Find(eq => eq.ID == id) == null)
        {
            return 0;
        }
        return _equipmentSlots.Find(eq => eq.ID == id).EquipmentCount;
    }

    public int GetEquipmentCount(EquipmentType equipmentType)
    {
        int count = 0;
        foreach (var sl in _equipmentSlots)
        {
            if (EquipmentManagerSO.GetEquipmentFromID(sl.ID).EqType == equipmentType)
            {
                count += sl.EquipmentCount;
            }
        }
        return count;
    }

    public int GetEquipmentDeficit(EquipmentType equipmentType)
    {
        var deficit = 0;
        foreach (var request in _supplyRequests)
        {
            if (request.EquipmentType == equipmentType)
            {
                deficit += request.EquipmentCount;
            }
        }
        return deficit;
    }

    public void AddEquipment(string id, int count)
    {
        if(_equipmentSlots.Find(eq => eq.ID == id) == null)
        {
            _equipmentSlots.Add(new CountryEquipmentStorageSlot(id, count));
            return;
        }
        else
        {
            _equipmentSlots.Find(eq => eq.ID == id).EquipmentCount += count;
        }
    }

    public void SetManpowerCount(int count)
    {
        var slot = _equipmentSlots.Find(slot => EquipmentManagerSO.GetEquipmentFromID(slot.ID).EqType == EquipmentType.Manpower);
        if (slot != null)
        {
            slot.EquipmentCount = count;
        }
        else
        {
            AddEquipment("manpower", count);
        }
    }

    private void RemoveEquipment(string id, int count)
    {
        if (GetEquipmentCount(id) < count) 
        {
            throw new System.Exception("You try get non-existent equipment");
        }
        if(GetEquipmentCount(id) - count == 0)
        {
            _equipmentSlots.Remove(_equipmentSlots.Find(eq => eq.ID == id));
            return;
        }
        _equipmentSlots.Find(eq => eq.ID == id).EquipmentCount -= count;
    }

    private void RemoveEquipment(EquipmentType equipmentType, int count)
    {
        if (GetEquipmentCount(equipmentType) < count)
        {
            throw new System.Exception("You try get non-existent equipment");
        }
        if (GetEquipmentCount(equipmentType) - count == 0)
        {
            _equipmentSlots.RemoveAll(sl => EquipmentManagerSO.GetEquipmentFromID(sl.ID).EqType == equipmentType);
            return;
        }
        foreach (var sl in _equipmentSlots)
        {
            if (EquipmentManagerSO.GetEquipmentFromID(sl.ID).EqType == equipmentType)
            {
                var difference = sl.EquipmentCount - count;
                if (difference >= 0)
                {
                    sl.EquipmentCount -= count;
                    break;
                }
                if (difference < 0)
                {
                    sl.EquipmentCount = 0;
                    count -= sl.EquipmentCount;
                }
            }
            if(count == 0)
            {
                break;
            }
        }
    }

    private void RemoveHowMuchIsAvailable(int needCount, EquipmentType equipmentType, out int getCount)
    {
        if(GetEquipmentCount(equipmentType) >= needCount)
        {
            RemoveEquipment(equipmentType, needCount);
            getCount = needCount;
        }
        else
        {
            getCount = GetEquipmentCount(equipmentType);
            RemoveEquipment(equipmentType, getCount);
        }
    }

    public CountryEquipmentStorageSave GetSerialize()
    {
        return new CountryEquipmentStorageSave(this);
    }

    public void LoadFromSerialize(CountryEquipmentStorageSave ser)
    {
        ser.Load(this);
    }

    [Serializable]
    public class CountryEquipmentStorageSave : SerializeForSave
    {
        public List<CountryEquipmentStorageSlotSave> Slots = new List<CountryEquipmentStorageSlotSave>();

        public CountryEquipmentStorageSave(CountryEquipmentStorage templates)
        {
            foreach (var temp in templates._equipmentSlots)
            {
                var dTemp = new CountryEquipmentStorageSlotSave();
                dTemp.SetData(temp);
                Slots.Add(dTemp);
            }
        }

        public CountryEquipmentStorageSave(string jsonSave)
        {
            JsonUtility.FromJsonOverwrite(jsonSave, this);
        }

        public override void Load(object objTarget)
        {
            var storage = objTarget as CountryEquipmentStorage;
            storage._equipmentSlots.Clear();
            foreach (var saveTemplate in Slots)
            {
                storage._equipmentSlots.Add(saveTemplate.CreateSlotFromData());
            }
        }

        public override string SaveToJson()
        {
            return JsonUtility.ToJson(this);
        }

        [Serializable]
        public class CountryEquipmentStorageSlotSave
        {
            public int EquipmentCount;
            public string ID;
            public void SetData(CountryEquipmentStorageSlot slot)
            {
                ID = slot.ID;
                EquipmentCount = slot.EquipmentCount;
            }

            public CountryEquipmentStorageSlot CreateSlotFromData()
            {
                var sl = new CountryEquipmentStorageSlot(ID, EquipmentCount);
                return sl;
            }
        }
      
    }
}

public class SupplyRequest
{
    public EquipmentType EquipmentType { get; }
    public SupplyRequestType RequestType { get; }
    public bool RequestClosed { get; private set; }
    public Action OnClosedRequest;
    public Action<int> OnAddedEquipment;
    public int EquipmentCount;

    public SupplyRequest(int equipmentCount, EquipmentType equipmentType)
    {
        EquipmentCount = equipmentCount;
        EquipmentType = equipmentType;
        RequestType = SupplyRequestType.Division;
        OnClosedRequest += delegate
        {
            RequestClosed = true;
        };
    }
}

public enum SupplyRequestType
{
    Division
}