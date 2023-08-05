using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Battalion", menuName = "ScriptableObjects/Battalion", order = 1)]
public class Battalion : ScriptableObject
{
    public string Name;
    public float Attack;
    public float Defend;
    public float Organization;
    public float Speed;
    public int ManPower = 1000;
    public Sprite BatImage;
    public DivisionViewType ViewType;
    public List<NeedEquipmentCountIdPair> NeedEquipmentList = new List<NeedEquipmentCountIdPair>();
    public List<Technology> NeededTechnologies = new List<Technology>();

    public bool CanUsed(Country country)
    {
        foreach (var technology in NeededTechnologies)
        {
            if (country.Research.GetOpenedTechnologies().Contains(technology) == false)
            {
                return false;
            }
        }
        return true;
    }
}


[Serializable]
public class NeedEquipmentCountIdPair
{
    public EquipmentType EqType;
    public int Count;

    public NeedEquipmentCountIdPair(EquipmentType eqType, int count)
    {
        EqType = eqType;
        Count = count;
    }
}

public enum DivisionViewType
{
    Infantry,
    Tanks
}