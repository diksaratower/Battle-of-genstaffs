using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Army 
{
    public Action<PlanBase> OnAddedPlan;
    public int MaxDivisionsCount { get; private set; } = 50;
    public int DivisionsCount { get => Divisions.Count; }
    public List<PlanBase> Plans => new List<PlanBase>(_plans);
    public List<Division> Divisions = new List<Division>();
    public DoPlanType DoPlanType = DoPlanType.Defense;

    private List<PlanBase> _plans = new List<PlanBase>();

    public Army(CountryArmies countryArmies)
    {

    }

    public void StopWorkArmy()
    {
        Divisions.Clear();
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
        Divisions.AddRange(divisions);
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