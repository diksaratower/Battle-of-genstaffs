using System.Collections.Generic;


public class SeaLandingPlan : PlanBase
{
    public Province TargetProvince { get; }
    public BuildingSlotProvince StartNavyBase { get; }

    private bool _isExecuted;


    public SeaLandingPlan(List<Division> divisions, Province targetProvince, BuildingSlotProvince startNavyBase) : base(divisions)
    {
        TargetProvince = targetProvince;
        StartNavyBase = startNavyBase;
    }

    public override void DoPlan(DoPlanType doType)
    {
        if (_isExecuted == true)
        {
            return;
        }
        if (doType == DoPlanType.Attack)
        {
            foreach (var division in AttachedDivisions)
            {
                if (division.DivisionProvince != TargetProvince && division.DivisionProvince == StartNavyBase.Province)
                {
                    if (TargetProvince.AllowedForDivision(division))
                    {
                        division.TeleportDivision(TargetProvince);
                        _isExecuted = true;
                    }
                }
            }
        }
    }

    public static bool ArmyCanExecuteSeaLanding(Army army, out BuildingSlotProvince buildingSlotProvince)
    {
        if (army.Divisions.Count == 0)
        {
            buildingSlotProvince = null;
            return false;
        }
        var firstProvince = army.Divisions[0].DivisionProvince;
        if (firstProvince.Buildings.Exists(building => building.TargetBuilding.BuildingType == BuildingType.NavyBase) == false)
        {
            buildingSlotProvince = null;
            return false;
        }
        foreach (Division division in army.Divisions)
        {
            if (division.DivisionProvince != firstProvince)
            {
                buildingSlotProvince = null;
                return false;
            }
        }
        buildingSlotProvince = firstProvince.Buildings.Find(building => building.TargetBuilding.BuildingType == BuildingType.NavyBase);
        return true;
    }
}
