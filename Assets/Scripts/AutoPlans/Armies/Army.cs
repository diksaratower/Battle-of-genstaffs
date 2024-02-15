using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


public class Army 
{
    public DoPlanType DoPlanType = DoPlanType.Defense;
    public Action<PlanBase> OnAddedPlan;
    public int MaxDivisionsCount { get; private set; } = 50;
    public float CashedForceFactorInFront { get; private set; }
    public ReadOnlyCollection<PlanBase> Plans => _plans.AsReadOnly();
    public ReadOnlyCollection<Division> Divisions => _divisions.AsReadOnly();

    private List<Division> _divisions = new List<Division>();
    private List<PlanBase> _plans = new List<PlanBase>();
    private CountryArmies _countryArmies { get; }


    public Army(CountryArmies countryArmies)
    {
        _countryArmies = countryArmies;
    }

    public void StopWorkArmy()
    {
        _divisions.Clear();
    }

    public void DoPlans()
    {
        foreach (var plan in Plans)
        {
            plan.DoPlan(DoPlanType);
        }
    }

    public void AddPlan(PlanBase plan)
    {
        _plans.Add(plan);
        if (plan is FrontPlan)
        {
            (plan as FrontPlan).Enemy.OnCapitulated += delegate
            {
                RemovePlan(plan);
            };
            (plan as FrontPlan).OnRecalculatedFront += (List<FrontPlan.FrontData> frontDates) => 
            {
                CashedForceFactorInFront = (plan as FrontPlan).GetForceFactor(frontDates);
            };
        }
        OnAddedPlan?.Invoke(plan);
    }

    public void RemovePlan(PlanBase plan)
    {
        if (_plans.Contains(plan) == false)
        {
            throw new TryRemoveOutOfArmyPlan();
        }
        plan.DeletePlan();
        _plans.Remove(plan);
    }

    public void RemoveAllPlans()
    {
        _plans.ForEach(plan => plan.DeletePlan());
        _plans.Clear();
    }

    public void AddDivisions(List<Division> divisions)
    {
        foreach (var division in divisions)
        {
            division.OnDivisionRemove += delegate
            {
                _divisions.Remove(division);
                if (Divisions.Count == 0)
                {
                    StopWorkArmy();
                    _countryArmies.RemoveArmy(this);
                }
            };
        }
        _divisions.AddRange(divisions);
    }

    public class TryRemoveOutOfArmyPlan : Exception
    {

    }
}

public enum DoPlanType
{
    Defense,
    Attack
}