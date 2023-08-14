using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


public class FrontPlan : PlanBase
{
    public Action<List<FrontData>> OnRecalculatedFront;
    public Action OnStoppedFront;
    public bool IsFrontStopped { get; private set; }
    public Country Enemy => _enemy;
    public Country Ally => _ally;
    private Country _enemy;
    private Country _ally;
    private int _divisionsPostionsChanges;

    public FrontPlan(List<Division> divisions, Country enemy, Country ally) : base(divisions)
    {
        AttachedDivisions = divisions;
        _enemy = enemy;
        _ally = ally;
    }

    public void Initialize()
    {
        foreach (var division in AttachedDivisions)
        {
            division.OnDivisionEnterToProvince += (Province province) => { if (division.DivisionState != DivisionAnimState.StepBack) { _divisionsPostionsChanges++; } };
        }
        RecalculateFront();
    }

    public override void DeletePlan()
    {
        foreach (var division in AttachedDivisions)
        {
            division.OnDivisionEnterToProvince -= (Province province) => { if (division.DivisionState != DivisionAnimState.StepBack) { _divisionsPostionsChanges++; } };
        }
        AttachedDivisions.Clear();
        OnStoppedFront?.Invoke();
        IsFrontStopped = true;
    }

    public override void DoPlan(DoPlanType doType)
    {
        if (_divisionsPostionsChanges > 0)
        {
            RecalculateFront();
            _divisionsPostionsChanges = 0;
        }

        if (doType == DoPlanType.Defense)
        {

        }
        if (doType == DoPlanType.Attack)
        {
            if (FrontCanExist(_ally, _enemy))
            {
                var frontAttackProvinces = Map.Instance.Provinces.FindAll(p => p.Owner == _enemy && p.Contacts.Exists(pr => pr.Owner == _ally));
                foreach (var div in AttachedDivisions)
                {
                    if (div.MovePath.Count == 0 && div.Combats.Count == 0)
                    {
                        AttackDivision(div, frontAttackProvinces);
                    }
                }
            }
        }
    }

    private async void RecalculateFront()
    {
        var newFrontDates = new List<FrontData>();
        await Task.Run(delegate 
        { 
           newFrontDates = GetFrontProvincesSuperSmartAsync(_ally, _enemy);
        });
        if (newFrontDates.Count == 0)
        {
            return;
        }
        var newFrontProvinces = new List<Province>();
        await Task.Run(delegate
        {
            foreach (var date in newFrontDates)
            {
                if (date.FrontAllyContacts.Count > 0)
                {
                    var enclavFront = date.FrontAllyContacts;
                    newFrontProvinces.AddRange(enclavFront);
                }
            }

        });
        SetDivisionsToFront(newFrontProvinces);
        OnRecalculatedFront?.Invoke(newFrontDates);
    }

    private void SetDivisionsToFront(List<Province> frontProvs)
    {
        if (AttachedDivisions.Count == 0) return;
        var divList = new List<Division>();
        divList.AddRange(AttachedDivisions);
        int DivCount = AttachedDivisions.Count;
        var plusValue = 0f;
        if (frontProvs.Count <= DivCount)
        {
            SetDivisionsToFrontDivisionsMoreFrontProvs(frontProvs, divList);
        }

        if (frontProvs.Count > DivCount)
        {
            plusValue = ((float)frontProvs.Count / (float)DivCount);
        }
        

        var divisionNumber = 0f;
        for (int i = 0; i < frontProvs.Count; i++)
        {
            var divisionIndex = Mathf.RoundToInt(divisionNumber);
            if (divisionIndex >= frontProvs.Count) break;
            if (divList.Count == 0) continue;
            if (frontProvs[divisionIndex] == null) continue;
            if (divList[0].DivisionProvince == frontProvs[divisionIndex]) continue;
            if (divList[0].MovePath.Count > 0 || divList[0].Combats.Count > 0 || divList[0].DivisionState != DivisionAnimState.Empty)
            {
                continue;
            }

            divList[0].MoveDivision(frontProvs[divisionIndex], false, pr => pr.Owner == Ally);

            divList.Remove(divList[0]);
            divisionNumber += plusValue;
        }
    }

    private void SetDivisionsToFrontDivisionsMoreFrontProvs(List<Province> frontProvs, List<Division> divisions)
    {
        while (divisions.Count > 0)
        {
            for (int i = 0; i < frontProvs.Count; i++)
            {
                if (divisions.Count == 0)
                {
                    break;
                }
                divisions[0].MoveDivision(frontProvs[i], false, pr => pr.Owner == Ally);
                divisions.Remove(divisions[0]);
            }
        }
    }

    private void AttackDivision(Division division, List<Province> frontProvinces)
    {
        var attackingProvs = new List<Province>();
        foreach (var div in AttachedDivisions)
        {
            if (div.MovePath.Count > 0)
            {
                attackingProvs.AddRange(div.MovePath);
            }
        }
        Province attakingProf = null;
        attakingProf = division.DivisionProvince.Contacts.Find(p => p.Owner == _enemy && !attackingProvs.Contains(p));
        if(attakingProf == null)
        {
            var enemyProvs = frontProvinces.FindAll(p => p.Owner == _enemy && !attackingProvs.Contains(p));
            if(enemyProvs.Count == 0) 
            {
                return;
            }
            attakingProf = Division.FindMinDistanceProv(enemyProvs, division.DivisionProvince);
        }

        if (((division.Organization / division.MaxOrganization) > 0.25f) && division.Combats.Count == 0 && division.GetEquipmentProcent() > 0.60f)
        {
            division.MoveDivision(attakingProf);
        }
    }

    public float GetForceFactor(List<FrontData> frontDates)
    {
        var enemyDivisionsInFront = new List<Division>(); 
        foreach (var frontData in frontDates)
        {
            foreach (var province in frontData.FrontProvinces)
            {
                foreach (var division in province.DivisionsInProvince)
                {
                    if (division.CountyOwner == _enemy)
                    {
                        enemyDivisionsInFront.Add(division);
                    }
                }
            }
        }
        var ourAttack = 0f;
        var enemyDefens = 0.00001f;
        AttachedDivisions.ForEach(division => { ourAttack += division.GetAttack(); });
        enemyDivisionsInFront.ForEach(division => { enemyDefens += division.GetDefense(); });
        return ourAttack / enemyDefens;
    }

    public static List<FrontData> GetFrontProvincesSuperSmartAsync(Country ally, Country enemy)
    {
        var result = new List<FrontData>();
        var frontProvinces = new List<Province>();

        frontProvinces = GetFrontProvinces(enemy, ally);

        while (frontProvinces.Count != 0)
        {
            var bfsProvinces = FrontPlan.BFS(frontProvinces[0], frontProvinces);
            result.Add(new FrontData(bfsProvinces, new List<Province>()));
            frontProvinces.RemoveAll(pr => bfsProvinces.Contains(pr));
        }

        foreach (var data in result)
        {
            foreach (var province in data.FrontProvinces)
            {
                data.FrontAllyContacts.AddRange(province.Contacts.FindAll(con => (con.Owner == ally && data.FrontAllyContacts.Contains(con) == false)));
            }
        }
        return result;
    }

    public struct FrontData
    {
        public List<Province> FrontProvinces { get; }
        public List<Province> FrontAllyContacts { get; }

        public FrontData(List<Province> frontProvinces, List<Province> frontAllyContacts)
        {
            FrontProvinces = frontProvinces;
            FrontAllyContacts = frontAllyContacts;
        }
    }

    public static bool FrontCanExist(Country ally, Country enemy)
    {
        var regions = ally.GetCountryRegions();
        if (regions.Count == 0)
        {
            return false;
        }
        foreach (var region in regions)
        {
            foreach (var province in region.Provinces)
            {
                if (province.Owner == ally && province.Contacts.Exists(pr => pr.Owner == enemy))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static List<Province> GetFrontProvinces(Country ally, Country enemy)
    {
        var frontProvs = new List<Province>();
        frontProvs = Map.Instance.Provinces.FindAll(p => p.Owner == ally && p.Contacts.Exists(pr => pr.Owner == enemy));
        return frontProvs;
    }

    public static List<Province> BFS(Province root, List<Province> allowed)
    {
        var q = new Queue<Province>();
        var visited = new HashSet<Province>();

        q.Enqueue(root);
        visited.Add(root);

        while (q.Count > 0)
        {
            var node = q.Dequeue();
            foreach (var c in node.Contacts)
            {
                if (!allowed.Contains(c)) continue;
                if (visited.Contains(c)) continue;
                q.Enqueue(c);
                visited.Add(c);
            }
        }

        return visited.ToList();
    }
}
