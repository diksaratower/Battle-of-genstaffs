using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PathFindAStar;

public class SeaLandingPlan : PlanBase
{
    public Province TargetProvince { get; }
    public BuildingSlotProvince StartNavyBase { get; }
    public Action OnRemoveLanding;

    private bool _isExecuted;
    private Country _country;


    public SeaLandingPlan(List<Division> divisions, Province targetProvince, BuildingSlotProvince startNavyBase, Country country) : base(divisions)
    {
        TargetProvince = targetProvince;
        StartNavyBase = startNavyBase;
        _country = country;
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
                        if (WeAreDominateInMarinePath())
                        {
                            division.TeleportDivision(TargetProvince);
                            _isExecuted = true;
                        }
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

    private bool WeAreDominateInMarinePath()
    {
        var marinePath = FindPathMarineLandingWithSea();
        var dominateRegions = 0;
        foreach (var region in marinePath)
        {
            if (region.IsDominate(_country, Diplomacy.Instance.GetCountryWarEnemies(_country), out _))
            {
                dominateRegions++;
            }
        }
        return dominateRegions == marinePath.Count;
    }

    public List<MarineRegion> FindPathMarineLandingWithSea()
    {
        if (StartNavyBase.Province.Contacts.Count == 6 || TargetProvince.Contacts.Count == 6)
        {
            throw new System.Exception("It is not waterside.");
        }
        var startMarineRegion = Map.Instance.MarineRegions.MarineRegionsList.Find(region => region.Provinces.Contains(StartNavyBase.Province));
        var targetMarineRegion = Map.Instance.MarineRegions.MarineRegionsList.Find(region => region.Provinces.Contains(TargetProvince));
        if (startMarineRegion == targetMarineRegion)
        {
            return new List<MarineRegion>(1) { startMarineRegion };
        }
        else
        {
            return FindPathBFS(startMarineRegion, targetMarineRegion, Map.Instance.MarineRegions.MarineRegionsList);
        }
    }

    public override void DeletePlan()
    {
        base.DeletePlan();
        OnRemoveLanding?.Invoke();
    }

    private static List<MarineRegion> FindPathBFS(MarineRegion root, MarineRegion target, List<MarineRegion> allowed)
    {
        var q = new Queue<BfsNode>();
        var visited = new List<BfsNode>();

        var rootNode = new BfsNode(null, root);
        q.Enqueue(rootNode);
        visited.Add(rootNode);

        while (q.Count > 0)
        {
            var node = q.Dequeue();
            foreach (var c in node.MarineRegion.Contacts)
            {
                var contactNode = new BfsNode(node, c);
                if (!allowed.Contains(contactNode.MarineRegion)) continue;
                if (visited.Exists(n => n.MarineRegion == contactNode.MarineRegion)) continue;
                q.Enqueue(contactNode);
                visited.Add(contactNode);
                if (contactNode.MarineRegion == target)
                {
                    return GetPathForNode(contactNode);
                }
            }
        }

        throw new System.Exception("Can not find marinr path for landing.");//visited.ToList();
    }

    private static List<MarineRegion> GetPathForNode(BfsNode pathNode)
    {
        var result = new List<MarineRegion>();
        var currentNode = pathNode;
        while (currentNode != null)
        {
            result.Add(currentNode.MarineRegion);
            currentNode = currentNode.ComeFrom;
        }
        result.Reverse();
        return result;
    }

    private class BfsNode
    {
        public BfsNode ComeFrom { get; }
        public MarineRegion MarineRegion { get; }

        public BfsNode(BfsNode comeFrom, MarineRegion marineRegion)
        {
            ComeFrom = comeFrom;
            MarineRegion = marineRegion;
        }
    }
}
