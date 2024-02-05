using System;
using System.Collections.Generic;
using UnityEngine;


public class UnitsManager : MonoBehaviour, ISaveble
{
    public static Action<Division, Province> OnDivisionEnterToProvince;

    public List<Division> Divisions = new List<Division>();
    public List<AviationDivision> AviationDivisions = new List<AviationDivision>();
    public Action<AviationDivision> OnCreateAviationDivision;
    public Action<AviationDivision> OnRemoveAviationDivision;
    public Action<Division> OnCreateDivision;
    public Action<Division> OnRemoveDivision;
    public static UnitsManager Instance;


    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
        OnDivisionEnterToProvince = null;
        OnCreateAviationDivision = null;
        OnRemoveAviationDivision = null;
        OnCreateDivision = null;
        OnRemoveDivision = null;
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

    public void RemoveAviationDivision(AviationDivision aviationDivision)
    {
        if (AviationDivisions.Contains(aviationDivision) == false)
        {
            throw new ArgumentException("Wrong in remove division.");
        }
        AviationDivisions.Remove(aviationDivision);
        OnRemoveAviationDivision?.Invoke(aviationDivision);
    }

    private Division AddDivision(Province province, Country owner, DivisionTemplate template)
    {
        var division = new Division(owner);
        division.OnDivisionEnterToProvince += (Province enteredProvince) =>
        {
            OnDivisionEnterToProvince?.Invoke(division, enteredProvince);
        };
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
                var serialize = new DivisionSave()
                {
                    Position = division.DivisionProvince.Position,
                    TemplateIndex = division.CountyOwner.Templates.Templates.IndexOf(division.Template),
                    CountryOwnerID = division.CountyOwner.ID,
                    Organization = division.Organization
                };
                foreach (var equipment in division.EquipmentInDivision)
                {
                    serialize.EquipmentInDivision.Add(new EquipmentCountIdPairSave(equipment));
                }
                Divisions.Add(serialize);
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
            foreach (var divisonSave in Divisions)
            {
                var divOwner = Map.Instance.GetCountryFromId(divisonSave.CountryOwnerID);
                var division = manager.AddDivision(divisonSave.Position, divOwner.Templates.Templates[divisonSave.TemplateIndex], divOwner);
                division.Organization = divisonSave.Organization;

                foreach (var equipmentSave in divisonSave.EquipmentInDivision)
                {
                    division.EquipmentInDivision.Add(new EquipmentCountIdPair(EquipmentManagerSO.GetEquipmentFromID(equipmentSave.EquipmentID), equipmentSave.Count));
                }
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
            public List<EquipmentCountIdPairSave> EquipmentInDivision = new List<EquipmentCountIdPairSave>();
        }

        [Serializable]
        public class EquipmentCountIdPairSave
        {
            public string EquipmentID;
            public int Count;

            public EquipmentCountIdPairSave(EquipmentCountIdPair countIdPair)
            {
                EquipmentID = countIdPair.Equipment.ID;
                Count = countIdPair.Count;
            }
        }
    }
}

public enum DivisionType
{
    Infantry,
    Tanks
}