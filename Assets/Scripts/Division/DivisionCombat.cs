using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


[Serializable]
public class DivisionCombat
{
    public ReadOnlyCollection<Division> Attackers => _attackers.AsReadOnly();
    public ReadOnlyCollection<Division> Defenders => _defenders.AsReadOnly();

    private List<Division> _attackers = new List<Division>();
    private List<Division> _defenders = new List<Division>();
    public Action OnEnd;

    
    private DivisionCombat() 
    {
    }

    public static DivisionCombat CreateCombat()
    {
        var combat = new DivisionCombat();
        UnitsManager.Instance.Combats.Add(combat);
        GameTimer.HourEnd += combat.CalculateBattel;
        return combat;
    }

    public void AddAtacker(Division division)
    {
        _attackers.Add(division);
        division.OnDivisionRemove += delegate
        {
            RemoveDivisionFromCombatIfExist(division);
        };
    }

    public void AddDefenser(Division division)
    {
        _defenders.Add(division);
        division.OnDivisionRemove += delegate
        {
            RemoveDivisionFromCombatIfExist(division);
        };
    }

    public void End()
    {
        UnitsManager.Instance.Combats.Remove(this);
        var divisions = new List<Division>();
        divisions.AddRange(_attackers);
        divisions.AddRange(_defenders);
        foreach (var division in divisions)
        {
            division.ExitFromCombat(this, false);
        }
        GameTimer.HourEnd -= CalculateBattel;
        OnEnd?.Invoke();
    }

    public Province GetCombatProvince()
    {
        return _defenders[0].DivisionProvince;
    }
    
    public static bool TryFindBattleInProv(Province province, out DivisionCombat combat)
    {
        foreach (var comb in UnitsManager.Instance.Combats)
        {
            if(comb.GetCombatProvince() == province)
            {
                combat = comb;
                return true;
            }
        }
        combat = null;
        return false;
    }

    private float GetAttackersAttack()
    {
        float attack = 0;
        foreach (var attacker in _attackers)
        {
            var politicsAttackCoof = attacker.CountyOwner.Politics.GetPoliticCooficentDivisionAttack();
            attack += (attacker.GetAttack() * GetAviationEffectPercent(attacker) * politicsAttackCoof);
        }
        return attack;
    }

    private float GetDefendersDefend() 
    {
        float def = 0;
        foreach (var defender in _defenders)
        {
            def += (defender.GetDefense() * GetAviationEffectPercent(defender));
        }
        return def;
    }

    public void CalculateBattel()
    {
        var rmAttackers = _attackers.FindAll(atk => atk.Organization <= 0);
        foreach (var attacker in rmAttackers)
        {
            attacker.ExitFromCombat(this);
        }
        var rmDefenders = _defenders.FindAll(df => df.Organization <= 0);
        foreach (var defender in rmDefenders)
        {
            defender.StepBackFromCombatDefender(this);
        }
        rmAttackers.ForEach(attacker => RemoveDivisionFromCombatIfExist(attacker));
        rmDefenders.ForEach(defender => RemoveDivisionFromCombatIfExist(defender));


        if(_defenders.Count == 0 || _attackers.Count == 0)
        {
            End();
            return;
        }

        foreach(var defender in _defenders)
        {
            defender.GiveDamageToOrganization((GetAttackersAttack() / _attackers.Count) / defender.GetDivisionStrength());
            IncurLosses(defender, 0.166f);
            
        }
        foreach (var attacker in _attackers)
        {
            attacker.GiveDamageToOrganization((GetDefendersDefend() / _defenders.Count) / attacker.GetDivisionStrength());
            IncurLosses(attacker, 0.5f);
        }
    }
    
    public static List<DivisionCombat> GetDivisionCombats(Division division)
    {
        var result = new List<DivisionCombat>();
        foreach (var combat in UnitsManager.Instance.Combats)
        {
            if (combat._defenders.Contains(division) || combat._attackers.Contains(division))
            {
                result.Add(combat);
            }
        }
        return result;
    }

    public void RemoveDivisionFromCombatIfExist(Division division)
    {
        if (_attackers.Contains(division))
        {
            _attackers.Remove(division);
        }
        if (_defenders.Contains(division))
        {
            _defenders.Remove(division);
        }
        if(_defenders.Count == 0 || _attackers.Count == 0)
        {
            End();
        }
    }

    public float GetProcentOfCombat()
    {
        return GetProcentOfCombat(out _);
    }

    public float GetProcentOfCombat(out Country leader)
    {
        leader = null;
        var procent = 0f;
        var defendersOrganization = GetAverageOrganization(_defenders);
        var attackerOrganization = GetAverageOrganization(_attackers);
        if(defendersOrganization < attackerOrganization)
        {
            procent = defendersOrganization / attackerOrganization;
            if (_attackers.Count > 0)
            {
                leader = _attackers[0].CountyOwner;
            }
        }
        if (attackerOrganization < defendersOrganization)
        {
            procent = attackerOrganization / defendersOrganization;
            if (_defenders.Count > 0)
            {
                leader = _defenders[0].CountyOwner;
            }
        }

        if (attackerOrganization == 0 || defendersOrganization == 0)
        {
            procent = 1f;
        }
        return procent;
    }

    private void IncurLosses(Division division, float lossesProcent)
    {
        var manpowerLosses = UnitsManager.CombatRandom.Next(0, 7);
        IncurLossesToEquipmentType(division, EquipmentType.Manpower, manpowerLosses);
        division.CountyOwner.OnManpowerLosses?.Invoke(manpowerLosses);
        IncurLossesToEquipmentType(division, EquipmentType.Rifle, UnitsManager.CombatRandom.Next(0, 3));
        IncurLossesToEquipmentType(division, EquipmentType.Tank, UnitsManager.CombatRandom.Next(0, 1));
    }

    private void IncurLossesToEquipmentType(Division division, EquipmentType equipmentType, int lossCount)
    {
        var equipmentCountIdPair = division.EquipmentInDivision.Find(equipment => equipment.Equipment.EqType == equipmentType);
        if(equipmentCountIdPair == null)
        {
            return;
        }
        for (int i = 0; i < lossCount; i++)
        {
            if (equipmentCountIdPair.Count > 0)
            {
                equipmentCountIdPair.Count -= 1;
            }
            else
            {
                break;
            }
        }
    }

    private float GetAviationEffectPercent(Division division)
    {
        var countryAviation = UnitsManager.Instance.AviationDivisions.FindAll(aviationDivision => aviationDivision.CountryOwner == division.CountyOwner);
        var aviationDivision = countryAviation.Find(aviationDivision => aviationDivision.CanHelp(division, out float _) == true);
        if (aviationDivision != null)
        {
            aviationDivision.CanHelp(division, out float bonus);
            return 1f + bonus;
        }
        return 1f;
    }

    private float GetAverageOrganization(List<Division> divisions)
    {
        float all = 0;
        foreach (var division in divisions)
        {
            all += division.Organization;
        }
        return (all / divisions.Count);
    }

    public List<string> GetDivisionEffectDescription(Division division)
    {
        if (_attackers.Contains(division) == false && _defenders.Contains(division) == false)
        {
            throw new ArgumentOutOfRangeException();
        }
        var result = new List<string>();
        var aviationEffect = GetAviationEffectPercent(division);
        if (aviationEffect != 1f)
        {
            result.Add($"ƒивизи€ получает бонус от авиации к защите и атаке {GameIU.FloatToStringAddPlus((float)Math.Round((aviationEffect - 1f) * 100, 2))}%");
        }
        return result;
    }
}
