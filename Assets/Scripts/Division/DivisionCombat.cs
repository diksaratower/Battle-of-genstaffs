using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DivisionCombat
{
    public static List<DivisionCombat> Combats = new List<DivisionCombat>();
    public static System.Random CombatRandom = new System.Random();

    public List<Division> Attackers = new List<Division>();
    public List<Division> Defenders = new List<Division>();
    public Action OnEnd;

    public DivisionCombat(Division division)//этот division тут не просто так, так надо.//ѕроверено второй раз он реально не просто так, даже не думай его удал€ть
    {
        Combats.Add(this);
        GameTimer.HourEnd += CalculateBattel;
        foreach (var attacker in Attackers)
        {
            attacker.OnDivisionRemove += delegate { RemoveDivisionFromCombat(attacker); };
        }
        foreach (var defender in Defenders)
        {
            defender.OnDivisionRemove += delegate { RemoveDivisionFromCombat(defender); };
        }
    }
    
    public void End()
    {
        Combats.Remove(this);
        var divisions = new List<Division>();
        divisions.AddRange(Attackers);
        divisions.AddRange(Defenders);
        foreach (var division in divisions)
        {
            division.ExitFromCombat(this, false);
        }
        GameTimer.HourEnd -= CalculateBattel;
        OnEnd?.Invoke();
    }

    public Province GetCombatProvince()
    {
        return Defenders[0].DivisionProvince;
    }
    
    public static bool TryFindBattleInProv(Province province, out DivisionCombat combat)
    {
        foreach (var comb in Combats)
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
        foreach (var attacker in Attackers)
        {
            attack += (attacker.GetAttack() * GetAviationEffectPercent(attacker));
        }
        return attack;
    }

    private float GetDefendersDefend() 
    {
        float def = 0;
        foreach (var defender in Defenders)
        {
            def += (defender.GetDefense() * GetAviationEffectPercent(defender));
        }
        return def;
    }

    public void CalculateBattel()
    {
        var rmAttackers = Attackers.FindAll(atk => atk.Organization <= 0);
        foreach (var attacker in rmAttackers)
        {
            attacker.ExitFromCombat(this);
        }
        var rmDefenders = Defenders.FindAll(df => df.Organization <= 0);
        foreach (var defender in rmDefenders)
        {
            defender.StepBackFromCombatDefender(this);
        }
        rmAttackers.ForEach(attacker => RemoveDivisionFromCombat(attacker));
        rmDefenders.ForEach(defender => RemoveDivisionFromCombat(defender));


        if(Defenders.Count == 0 || Attackers.Count == 0)
        {
            End();
            return;
        }

        foreach(var defender in Defenders)
        {
            defender.GiveDamageToOrganization((GetAttackersAttack() / Attackers.Count) / defender.GetDivisionStrength());
            IncurLosses(defender, 0.166f);
            
        }
        foreach (var attacker in Attackers)
        {
            attacker.GiveDamageToOrganization((GetDefendersDefend() / Defenders.Count) / attacker.GetDivisionStrength());
            IncurLosses(attacker, 0.5f);
        }
    }
    
    public static List<DivisionCombat> GetDivisionCombats(Division division)
    {
        var result = new List<DivisionCombat>();
        foreach (var combat in Combats)
        {
            if (combat.Defenders.Contains(division) || combat.Attackers.Contains(division))
            {
                result.Add(combat);
            }
        }
        return result;
    }

    public void RemoveDivisionFromCombat(Division division)
    {
        if (Attackers.Contains(division))
        {
            Attackers.Remove(division);
        }
        if (Defenders.Contains(division))
        {
            Defenders.Remove(division);
        }
        if(Defenders.Count == 0 || Attackers.Count == 0)
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
        var defendersOrganization = GetAverageOrganization(Defenders);
        var attackerOrganization = GetAverageOrganization(Attackers);
        if(defendersOrganization < attackerOrganization)
        {
            procent = defendersOrganization / attackerOrganization;
            if (Attackers.Count > 0)
            {
                leader = Attackers[0].CountyOwner;
            }
        }
        if (attackerOrganization < defendersOrganization)
        {
            procent = attackerOrganization / defendersOrganization;
            if (Defenders.Count > 0)
            {
                leader = Defenders[0].CountyOwner;
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
        var manpowerLosses = CombatRandom.Next(0, 7);
        IncurLossesToEquipmentType(division, EquipmentType.Manpower, manpowerLosses);
        division.CountyOwner.OnManpowerLosses?.Invoke(manpowerLosses);
        IncurLossesToEquipmentType(division, EquipmentType.Rifle, CombatRandom.Next(0, 3));
        IncurLossesToEquipmentType(division, EquipmentType.Tank, CombatRandom.Next(0, 1));
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
        if (Attackers.Contains(division) == false && Defenders.Contains(division) == false)
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
