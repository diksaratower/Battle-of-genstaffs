using System;
using System.Collections.Generic;
using UnityEngine;


public class UnitsManager : MonoBehaviour, ISaveble
{
    public List<Division> Divisions = new List<Division>();
    public List<AviationDivision> AviationDivisions = new List<AviationDivision>();
    public Action<AviationDivision> OnCreateAviationDivision;
    public Action<Division> OnCreateDivision;
    public Action<Division> OnRemoveDivision;
    public static UnitsManager Instance;


    private void Awake()
    {
        Instance = this;
    }

    public Division AddDivision(Province province, DivisionTemplate template, Country owner)
    { 
        return AddDivision(province.Position, template, owner);
    }

    public Division AddDivision(Vector3 position, DivisionTemplate template, Country owner)
    {
        return AddDivision(Map.GetProvinceFromPosition(position), owner, template);
    }

    public AviationDivision AddAviationDivision(BuildingSlotRegion aviabase, Country owner)
    {
        var aviationDivision = new AviationDivision(aviabase, owner, $"{AviationDivisions.Count + 1} авиадивизия {owner.Name}");
        AviationDivisions.Add(aviationDivision);
        OnCreateAviationDivision?.Invoke(aviationDivision);
        return aviationDivision;
    }

    private Division AddDivision(Province province, Country owner, DivisionTemplate template)
    {
        var division = new Division(owner);
        division.Name = "Infantry division " + (Divisions.Count + 1).ToString();
        division.SetTemplate(template); 
        Divisions.Add(division);
        division.TeleportDivision(province);
        OnCreateDivision?.Invoke(division);
        return division;
    }

    public void RemoveDivision(Division division)
    {
        if (!Divisions.Contains(division))
        {
            throw new System.Exception("Division not be");
        }
        foreach (var combat in division.Combats)
        {
            combat.RemoveDivisionFromCombat(division);
        }
        OnRemoveDivision?.Invoke(division);
        division.DivisionProvince.DivisionsInProvince.Remove(division);
        Divisions.Remove(division);
    }

    string ISaveble.GetFileName()
    {
        return "divisions";
    }

    string ISaveble.Save()
    {
        return new UnitsManagerSerialize(this).SaveToJson();
    }

    void ISaveble.Load(string json)
    {
        new UnitsManagerSerialize(json).Load(this);
    }

    Type ISaveble.GetSaveType()
    {
        throw new NotImplementedException();
    }

    public class UnitsManagerSerialize : SerializeForSave
    {
        public List<DivisionSave> Divisions = new List<DivisionSave>();

        public UnitsManagerSerialize(UnitsManager manager)
        {
            foreach (var division in manager.Divisions)
            {
                var ser = new DivisionSave()
                {
                    Position = division.DivisionProvince.Position,
                    TemplateIndex = division.CountyOwner.Templates.Templates.IndexOf(division.Template),
                    CountryOwnerID = division.CountyOwner.ID,
                    Organization = division.Organization
                };
                //ser.EquipmentInDivision = division.EquipmentInDivision;
                Divisions.Add(ser);
            }
        }

        public UnitsManagerSerialize(string jsonSave)
        {
            JsonUtility.FromJsonOverwrite(jsonSave, this);
        }

        public override void Load(object objTarget)
        {
            var manager = (UnitsManager)objTarget;
            while (manager.Divisions.Count != 0)
            {
                manager.RemoveDivision(manager.Divisions[0]);
            }
            foreach (var divison in Divisions)
            {
                var divOwner = Map.Instance.GetCountryFromId(divison.CountryOwnerID);
                var div = manager.AddDivision(divison.Position, divOwner.Templates.Templates[divison.TemplateIndex], divOwner);
                div.Organization = divison.Organization;
                //div.EquipmentInDivision = divison.EquipmentInDivision;
            }
        }

        public override string SaveToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        [Serializable]
        public class DivisionSave
        {
            public Vector3 Position;
            public int TemplateIndex;
            public float Organization;
            public string CountryOwnerID;
            public List<TypedEquipmentCountIdPair> EquipmentInDivision = new List<TypedEquipmentCountIdPair>();
        }
    }
}

public enum DivisionType
{
    Infantry,
    Tanks
}