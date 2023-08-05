using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlanBase
{
    public List<Division> AttachedDivisions = new List<Division>();

  
    public PlanBase() {
    }

    public PlanBase(List<Division> divisions)
    {
        AttachedDivisions = divisions;
    }

    public virtual void DoPlan(DoPlanType doType)
    {

    }

    public virtual void RemoveAllDivisions()
    {
        AttachedDivisions.Clear();
    }

    public virtual void DeletePlan()
    {
        RemoveAllDivisions();
    }

    public void StopAllDivs()
    {

    }
}
