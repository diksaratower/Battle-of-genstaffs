using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryEquipmentStorageSlot
{
    public int EquipmentCount;
    public string ID;

    public CountryEquipmentStorageSlot(string id, int count)
    {
        ID = id;
        EquipmentCount = count;
    }
}
