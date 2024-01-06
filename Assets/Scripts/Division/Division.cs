using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Division : SupplyUnit
{
    public DivisionAnimState DivisionState { get; private set; } = DivisionAnimState.Empty;
    public Country CountyOwner { get; private set; }
    public Province DivisionProvince { get; private set; }
    public Action<DivisionAnimState> OnDivisionChangedState;
    public Action<Province> OnDivisionEnterToProvince;
    public Action<DivisionTemplate> OnSetTemplate;
    public Action OnDivisionRemove;
    public Sprite DivisionAvatar;
    public string Name;
    public DivisionTemplate Template;
    public float Organization;
    public List<Province> MovePath = new List<Province>();
    public List<DivisionCombat> Combats => DivisionCombat.GetDivisionCombats(this);
    public float MaxOrganization = 10;
    public bool IsDeleted { get; private set; } = false;

    private float _speed = 0;
    private float _attack = 1;
    private float _defense = 1;
    private float _moveOrganizationLoss = 3;
    private float _recoverySpeedPercent = 0.03f;

    private float _passedPathProcent = 0;

    public Division(Country country) : base(country.EquipmentStorage)
    {
        CountyOwner = country;
        Organization = MaxOrganization;
       
        GameTimer.HourEnd += CalculateMovement;
        GameTimer.HourEnd += CalculateOrganization;
        OnDivisionRemove += delegate
        {
            StopSupply();
        };
    }

    private void CalculateMovement()
    {
        if (MovePath.Count == 0)
        {
            return;
        }
        if (MovePath[0] == DivisionProvince)
        {
            MovePath.Remove(MovePath[0]);
            if(MovePath.Count == 0)
            {
                return;
            }
        }
        if (Combats.Count > 0) return;
        if (MovePath[0].DivisionsInProvince.Count > 0)
        {
            var divsForBattels = MovePath[0].DivisionsInProvince.FindAll(d => (CountyOwner.CountryDivsCanFightWithDiv(d)
            && d.DivisionState != DivisionAnimState.StepBack));
            if(divsForBattels.Count > 0)
            {
                if(DivisionCombat.TryFindBattleInProv(MovePath[0], out DivisionCombat comb))
                {
                    comb.Attackers.Add(this);
                    EnterToCombat(comb);
                    return;
                }
                else
                {
                    var c = new DivisionCombat(this);
                    c.Attackers.Add(this);
                    c.Defenders.AddRange(divsForBattels);
                    foreach (var div in divsForBattels)
                    {
                        div.EnterToCombat(c);
                    }
                    EnterToCombat(c);
                    return;
                }    
            }
        }

        _passedPathProcent += _speed;

        if (_passedPathProcent >= 100)
        {
            if (MovePath[0].AllowedForDivision(this) == false)
            {
                throw new Exception("Province not allowed for country.");
            }
            if(DivisionState == DivisionAnimState.StepBack)
            {
                if (MovePath[0].FriendlyForDivision(this) == false)
                {
                    KillDivision();
                    return;
                }
            }
            _passedPathProcent = 0;
            DivisionProvince = MovePath[0];
            DivisionProvince.OnDivisonEnter(this);
            MovePath.Remove(MovePath[0]);
            if (DivisionState == DivisionAnimState.StepBack)
            {
                if (MovePath.Count == 0)
                {
                    SetState(DivisionAnimState.Empty);
                }
            }
            if (MovePath.Count == 0)
            {
                SetState(DivisionAnimState.Empty);
            }
            OnDivisionEnterToProvince?.Invoke(DivisionProvince);
        }
    }

    private void CalculateOrganization()
    {
        if (Organization > 0 && DivisionState == DivisionAnimState.Move)
        {
            Organization -= _moveOrganizationLoss; 
        }
        if (Combats.Count == 0 && Organization < MaxOrganization)
        {
            Organization += (MaxOrganization * _recoverySpeedPercent);
            if (Organization > MaxOrganization)
            {
                Organization = MaxOrganization;
            }
        }
    }

    public void TeleportDivision(Province province)
    {
        StopDivision();
        DivisionProvince = province;
        DivisionProvince.OnDivisonEnter(this);
        OnDivisionEnterToProvince?.Invoke(DivisionProvince);
    }

    public bool NeedDrawDivisionUI()
    {
        if (IsDeleted == true)
        {
            return false;
        }
        if (CountyOwner == Player.CurrentCountry)
        {
            return true;
        }
        if (DivisionProvince.Contacts.Exists(p => p.Owner == Player.CurrentCountry))
        {
            var playerContactProvinces = DivisionProvince.Contacts.FindAll(p => p.Owner == Player.CurrentCountry);
            foreach (var contact in playerContactProvinces)
            {
                if (contact.DivisionsInProvince.Exists(division => division.CountyOwner == Player.CurrentCountry))
                {
                    return true;
                }
            }
        }
        if (DivisionProvince.Owner == Player.CurrentCountry)
        {
            return true;
        }
        return false;
    }

    public void MoveDivision(Province targetProv, bool extendPath, List<Province> allowedProvs)
    {
        MoveDivision(targetProv, extendPath, pr => allowedProvs.Contains(pr));
    }

    public void MoveDivision(Province targetProv, bool extendPath = false, Predicate<Province> allowedProvs = null)
    {
        if(DivisionState == DivisionAnimState.Defend)
        {
            return;
        }
        if (DivisionState == DivisionAnimState.Attack)
        {
            foreach (var combat in Combats)
            {
                ExitFromCombat(combat, !extendPath);
            }
        }

        if (extendPath && MovePath.Count > 0)
        {
            var newPath = PathFindAStar.FindPath(MovePath[MovePath.Count - 1], targetProv, allowedProvs, this);
            if (newPath == null)
            {
                return;
            }
            MovePath.AddRange(newPath);
        }
        if (extendPath == false)
        {
            var newPath = PathFindAStar.FindPath(DivisionProvince, targetProv, allowedProvs, this);
            if (newPath == null)
            {
                return;
            }
            MovePath.Clear();
            MovePath = newPath;
            if (MovePath.Count > 0)
            {
                SetState(DivisionAnimState.Move);
            }
        }
        if(MovePath.Count == 1) 
        {
            if (MovePath[0] == DivisionProvince)
            {
                SetState(DivisionAnimState.Empty);
                MovePath.Clear();
            }
        }
    }

    public void StopDivision()
    {
        MovePath.Clear();
        SetState(DivisionAnimState.Empty);
    }

    public static Province FindMinDistanceProv(List<Province> provinces, Province targetProv)
    {
        var distances = new List<float>();
        for (int i = 0; i < provinces.Count; i++)
        {
            distances.Add(Vector3.Distance(provinces[i].Position, targetProv.Position));
        }
        var min = distances.Min();
        return provinces.Find(p => Vector3.Distance(p.Position, targetProv.Position) == min);
    }

    public float GetDivisionStrength()
    {
        return (GetEquipmentProcent(eqType => eqType != EquipmentType.Manpower) * GetEquipmentProcent(eqType => eqType == EquipmentType.Manpower));
    }

    public float GetAttack()
    {
        var attack = 0f;
        foreach (var equipmnet in EquipmentInDivision)
        {
            if (equipmnet.Equipment is IGroundCombatEquipment)
            {
                attack += ((equipmnet.Equipment as IGroundCombatEquipment).Attack) * equipmnet.Count;
            }
        }
        return attack;
    }

    public float GetDefense()
    {
        var defense = 0f;
        foreach (var equipmnet in EquipmentInDivision)
        {
            if (equipmnet.Equipment is IGroundCombatEquipment)
            {
                defense += ((equipmnet.Equipment as IGroundCombatEquipment).Defens) * equipmnet.Count;
            }
        }
        return defense;
    }

    public void GiveDamageToOrganization(float damage)
    {
        Organization -= damage;
        if (Organization < 0)
        {
            Organization = 0;
        }
    }

    public void EnterToCombat(DivisionCombat combat)
    {
        if(combat.Attackers.Contains(this))
        {
            SetState(DivisionAnimState.Attack);
        }
        else if (combat.Defenders.Contains(this))
        {
            SetState(DivisionAnimState.Defend);
        }
        else
        {
            Debug.Log("error in battel set state");
        }
    }

    public void ExitFromCombat(DivisionCombat combat, bool stopDivision = true)
    {
        combat.RemoveDivisionFromCombat(this);
        SetState(DivisionAnimState.Empty);
        if (stopDivision)
        {
            StopDivision();
        }
    }

    public void StepBackFromCombatDefender(DivisionCombat combat)
    {
        var stepBackProv = DivisionProvince.Contacts.Find(province => province.FriendlyForDivision(this) == true); //p.Owner == CountyOwner && p != DivisionProvince);
        
        if (stepBackProv == null)
        {
            KillDivision();
            return;
        }
        MovePath = new List<Province>();
        StopDivision();
        MoveDivision(stepBackProv);
        SetState(DivisionAnimState.StepBack);
    }

    public void SetTemplate(DivisionTemplate template)
    {
        Template = template;
        _neededEquipment = Template.GetTemplateNeedEquipment();
        EquipmentInDivision.Clear();
        /*EquipmentInDivision.AddRange(Template.GetTemplateNeedEquipment());
        for (int i = 0; i < EquipmentInDivision.Count; i++)
        {
            EquipmentInDivision[i] = new TypedEquipmentCountIdPair(EquipmentInDivision[i].EqType, 0);
        }*/
       
        DivisionAvatar = template.GetAvatar();
        _attack = template.Attack;
        _defense = template.Defend;
        MaxOrganization = template.Organization;
        _speed = template.Speed;
        OnSetTemplate?.Invoke(template);
    }

    public void KillDivision()
    {
        GameTimer.HourEnd -= CalculateMovement;
        GameTimer.HourEnd -= CalculateOrganization;
        OnDivisionRemove?.Invoke();
        UnitsManager.Instance.RemoveDivision(this);
        IsDeleted = true;
    }

    private void SetState(DivisionAnimState divisionState)
    {
        DivisionState = divisionState;
        OnDivisionChangedState?.Invoke(DivisionState);
    }
}

[Serializable]
public class EquipmentInDivision
{
    public string ID;
    public int Count;

    public static implicit operator EquipmentInDivision(KeyValuePair<string, int> kp)
    {
        return new EquipmentInDivision() { Count = kp.Value, ID = kp.Key };
    }
}

public enum DivisionAnimState
{
    Empty,
    Move,
    StepBack,
    Attack,
    Defend
}