using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EquipmentManager", order = 1)]
public class EquipmentManagerSO : ScriptableObject
{
    public List<Equipment> EquipmentList = new List<Equipment>();

    private static EquipmentManagerSO _instance;

    public static Equipment GetEquipmentFromID(string id)
    {
        return GetInstance().EquipmentList.Find(x => x.ID == id);
    }

    public static List<Equipment> GetAllEquipment()
    {
        var list = new List<Equipment>();
        list.AddRange(GetInstance().EquipmentList);
        return list;
    }

    private static EquipmentManagerSO GetInstance()
    {
        if (_instance == null)
        {
            _instance = EquipmentManagerInstancer.GetInstance();
        }
        return _instance;
    }
}
