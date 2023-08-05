using System.Collections.Generic;
using UnityEngine;


public class AirbasesViewerUI : MonoBehaviour
{
    public bool ViewAdttionalInfo => _aviationModeUI.gameObject.activeSelf; 

    [SerializeField] private Transform _arbaseViewsParent;
    [SerializeField] private AirbaseViewUI _arbaseViewPrefab;
    [SerializeField] private AviationModeUI _aviationModeUI;

    private List<AirbaseViewUI> _airbaseViews = new List<AirbaseViewUI>();


    private void Update()
    {
        RefreshAirbases();
    }

    public void ActivateAviasionMode(BuildingSlot selectedAviabaseRegion)
    {
        _aviationModeUI.gameObject.SetActive(true);
        _aviationModeUI.RefreshUI(selectedAviabaseRegion);
    }

    public void OnAirBaseGetRightClick(AirbaseViewUI sender)
    {
        _aviationModeUI.MoveSelectedDivisions(sender.Target);
    }

    private void RefreshAirbases()
    {
        var regions = Player.CurrentCountry.GetCountryRegions();
        foreach (var region in regions) 
        {
            if (region.GetBuildings(BuildingType.Airbase).Count > 0)
            {
                if (_airbaseViews.Exists(airbase => region.GetBuildings(BuildingType.Airbase).Contains(airbase.Target)) == false)
                {
                    var airbase = Instantiate(_arbaseViewPrefab, _arbaseViewsParent);
                    airbase.RefreshUI(region.GetBuildings(BuildingType.Airbase)[0], this);
                    _airbaseViews.Add(airbase);
                }
            }
        }
    }
}
