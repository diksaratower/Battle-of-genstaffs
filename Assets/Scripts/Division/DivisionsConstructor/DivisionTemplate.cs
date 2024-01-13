using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DivisionTemplate
{
    public List<Battalion> Battalions => GetBattalions();
    public List<DivisionLine> DivisionLines = new List<DivisionLine>();
    public string Name;
    public float Attack => GetAttack();
    public float Defend => GetDefend();
    public float Organization => GetOrganization();
    public int Manpower => GetManpower();
    public float Speed => GetSpeed();

    public DivisionTemplate()
    {

    }

    public DivisionTemplate(string name)
    {
        Name = name;
    }

    private float GetSpeed()
    {
        var bats = GetBattalions();
        var speeds = new List<float>();
        foreach (var bat in bats) 
        {
            speeds.Add(bat.Speed);
        }
        if(speeds.Count == 0)
        {
            return 0;
        }
        return speeds.Min();
    }

    public List<Battalion> GetBattalions()
    {
        var bats = new List<Battalion>();
        foreach(var ln in DivisionLines) 
        {
            bats.AddRange(ln.Battalions);
        }
        return bats;
    }

    public Sprite GetAvatar()
    {
        if (Battalions.Count == 0)
        {
            return null;
        }
        return Battalions[0].BatImage;
    }

    public Battalion GetAverageBattlion()
    {
        if (Battalions.Count == 0)
        {
            return null;
        }
        return Battalions[0];
    }

    public void UpdateDivisionsWithTemplate()
    {
        foreach (var division in UnitsManager.Instance.Divisions)
        {
            if (division.Template == this)
            {
                division.SetTemplate(this);
            }
        }
    }

    private float GetAttack()
    {
        float attack = 0;
        foreach (var bat in Battalions) 
        {
            attack += bat.Attack;
        }
        return attack;
    }

    private float GetDefend()
    {
        float defend = 0;
        foreach (var bat in Battalions)
        {
            defend += bat.Defend;
        }
        return defend;
    }

    private float GetOrganization()
    {
        float organization = 0;
        foreach (var bat in Battalions)
        {
            organization += bat.Organization;
        }
        return organization;
    }

    private int GetManpower()
    {
        int manPower = 0;
        foreach (var bat in Battalions)
        {
            manPower += bat.ManPower;
        }
        return manPower;
    }

    public List<KeyValuePair<string, int>> GetTemplateNeedEquipmentKeyValPair()
    {
        var list = new List<KeyValuePair<string, int>>();
        var types = Enum.GetValues(typeof(EquipmentType));
        for (int i = 0; i < types.Length; i++)
        {
            int eqNeedCount = 0;
            foreach (var bat in Battalions)
            {
                foreach (var needEq in bat.NeedEquipmentList)
                {
                    if (needEq.EqType == (EquipmentType)types.GetValue(i))
                    {
                        eqNeedCount += needEq.Count;
                    }
                }
            }
            if(eqNeedCount > 0)
            {
                var typePair = new KeyValuePair<string, int>(types.GetValue(i).ToString(), eqNeedCount);
                list.Add(typePair);
            }
        }
        return list;
    }

    public List<TypedEquipmentCountIdPair> GetTemplateNeedEquipment()
    {
        var list = new List<TypedEquipmentCountIdPair>();
        var types = Enum.GetValues(typeof(EquipmentType));
        for (int i = 0; i < types.Length; i++)
        {
            int eqNeedCount = 0;
            foreach (var bat in Battalions)
            {
                foreach (var needEq in bat.NeedEquipmentList)
                {
                    if (needEq.EqType == (EquipmentType)types.GetValue(i))
                    {
                        eqNeedCount += needEq.Count;
                    }
                }
            }
            if (eqNeedCount > 0)
            {
               // var typePair = new KeyValuePair<string, int>(types.GetValue(i).ToString(), eqNeedCount);
                list.Add(new TypedEquipmentCountIdPair((EquipmentType)types.GetValue(i), eqNeedCount));
            }
        }
        return list;
    }
}
