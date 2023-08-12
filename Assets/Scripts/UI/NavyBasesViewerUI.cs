using System.Collections.Generic;
using UnityEngine;


public class NavyBasesViewerUI : MonoBehaviour
{
    [SerializeField] private Transform _navyBaseViewsParent;
    [SerializeField] private NavyBaseViewUI _navyBaseViewPrefab;

    private List<NavyBaseViewUI> _navybaseViews = new List<NavyBaseViewUI>();


    private void Start()
    {
        CountryBuild.OnAddBuilding += (Building building) =>
        {
            if (building.BuildingType == BuildingType.NavyBase)
            {
                RefreshNavyBases();
            }
        };
        RefreshNavyBases();
    }

    private void RefreshNavyBases()
    {
        foreach (var region in Map.Instance.MapRegions)
        {
            foreach (var province in region.Provinces)
            {
                var navybasesProvince = province.Buildings.FindAll(building => building.TargetBuilding.BuildingType == BuildingType.NavyBase);
                if (navybasesProvince.Count > 0)
                {
                    if (_navybaseViews.Exists(navybase => navybasesProvince.Contains(navybase.Target)) == false)
                    {
                        var airbase = Instantiate(_navyBaseViewPrefab, _navyBaseViewsParent);
                        airbase.RefreshUI(navybasesProvince[0], this);
                        _navybaseViews.Add(airbase);
                    }
                }
            }
        }
    }
}
