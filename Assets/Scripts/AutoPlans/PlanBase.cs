using System;
using System.Collections.Generic;


public abstract class PlanBase
{
    public List<Division> AttachedDivisions = new List<Division>();

    public PlanBase(List<Division> divisions)
    {
        AttachedDivisions = divisions;
    }

    public abstract void DoPlan(DoPlanType doType);

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
