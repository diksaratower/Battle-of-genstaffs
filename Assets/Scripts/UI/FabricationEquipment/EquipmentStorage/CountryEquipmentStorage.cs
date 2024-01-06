using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


public class CountryEquipmentStorage
{
    private List<CountryEquipmentStorageSlot> _equipmentSlots = new List<CountryEquipmentStorageSlot>();

    private Country _country;

    public CountryEquipmentStorage(Country country)
    {
        GameTimer.DayEnd += CalculateSupply;
        _country = country;
    }

    private void CalculateSupply()
    {
        var typesEquipment = Enum.GetValues(typeof(EquipmentType)).Cast<EquipmentType>().ToList();
        var divisions = UnitsManager.Instance.Divisions.FindAll(d => d.CountyOwner == _country);
        var aviationDivisions = UnitsManager.Instance.AviationDivisions.FindAll(d => d.CountryOwner == _country);
        SupplyDivisions(typesEquipment, divisions);
        SupplyAviation(aviationDivisions);
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
        var divisions = UnitsManager.Instance.Divisions.FindAll(d => d.CountyOwner == _country);
        foreach (var division in divisions)
        {
            deficit += -(division.GetDefficit(equipmentType));
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

    private void SupplyDivisions(List<EquipmentType> typesEquipment, List<Division> divisions)
    {
        foreach (var type in typesEquipment)
        {
            var allNeed = 0;
            foreach (var division in divisions)
            {
                allNeed += (division.GetDefficit(type) * -1);
            }
            var getedEquipment = RemoveHowMuchIsAvailableDeteil(allNeed, type, out var getedCount);
            foreach (var equipment in getedEquipment)
            {
                while (true)
                {
                    if (equipment.Count == 0 || equipment.Count < 0)
                    {
                        break;
                    }
                    foreach (var division in divisions)
                    {
                        if (division.GetDefficit(type) == 0)
                        {
                            continue;
                        }
                        if (equipment.Count == 0)
                        {
                            break;
                        }
                        equipment.Count -= 1;
                        division.AddEquipment(new EquipmentCountIdPair(equipment.Equipment, 1));
                    }
                }
            }
        }
        foreach (var division in divisions)
        {
            division.OnGetSupply?.Invoke();
        }
    }

    private void SupplyAviation(List<AviationDivision> aviationDivisions)
    {
        foreach (var aviationDivision in aviationDivisions)
        {
            var getedEquipment = RemoveHowMuchIsAvailableDeteil((aviationDivision.GetDefficit(EquipmentType.Fighter) * -1), EquipmentType.Fighter, out var getedCount);
            foreach (var equipment in getedEquipment)
            {
                aviationDivision.AddEquipment(equipment);
            }
        }
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

    private List<EquipmentCountIdPair> RemoveEquipmentDetail(EquipmentType equipmentType, int count)
    {
        var list = new List<EquipmentCountIdPair>();
        if (GetEquipmentCount(equipmentType) < count)
        {
            throw new System.Exception("You try get non-existent equipment");
        }
        foreach (var sl in _equipmentSlots)
        {
            var equipment = EquipmentManagerSO.GetEquipmentFromID(sl.ID);
            if (equipment.EqType == equipmentType)
            {
                var difference = sl.EquipmentCount - count;
                if (difference >= 0)
                {
                    sl.EquipmentCount -= count;
                    list.Add(new EquipmentCountIdPair(equipment, count));
                    break;
                }
                if (difference < 0)
                {
                    sl.EquipmentCount = 0;
                    if (list.Find(eq => eq.Equipment == equipment) == null)
                    {
                        list.Add(new EquipmentCountIdPair(equipment, sl.EquipmentCount));
                    }
                    else
                    {
                        list.Find(eq => eq.Equipment == equipment).Count += sl.EquipmentCount;
                    }
                    count -= sl.EquipmentCount;
                }
            }
            if (count == 0)
            {
                break;
            }
        }
        return list;
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

    private List<EquipmentCountIdPair> RemoveHowMuchIsAvailableDeteil(int needCount, EquipmentType equipmentType, out int getCount)
    {
        var list = new List<EquipmentCountIdPair>();
        if (GetEquipmentCount(equipmentType) >= needCount)
        {
            list.AddRange(RemoveEquipmentDetail(equipmentType, needCount));
            getCount = needCount;
        }
        else
        {
            getCount = GetEquipmentCount(equipmentType);
            list.AddRange(RemoveEquipmentDetail(equipmentType, getCount));
        }
        return list;
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

