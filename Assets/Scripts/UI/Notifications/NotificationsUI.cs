using UnityEngine;
using UnityEngine.UI;

public class NotificationsUI : MonoBehaviour
{
    public Country TargetCountry => Player.CurrentCountry;

    [SerializeField] private GameObject _notBuildingsInQueue;
    [SerializeField] private GameObject _freeMilitaryFactories;
    [SerializeField] private GameObject _canResearchTech;
    [SerializeField] private GameObject _notChoosedFocus;
    [SerializeField] private Button _countryIsWar;
    [SerializeField] private WarUI _warUI;

    private void Start()
    {
        _countryIsWar.onClick.AddListener(delegate 
        {
            if (Diplomacy.Instance.CountryIsAtWar(TargetCountry))
            {
                _warUI.gameObject.SetActive(true);
                _warUI.RefreshUI(Diplomacy.Instance.GetCountryWars(TargetCountry)[0]);
            }
        });
    }

    private void Update()
    {
        _notBuildingsInQueue.SetActive(TargetCountry.CountryBuild.BuildingsQueue.Count == 0);
        _freeMilitaryFactories.SetActive(IsHaveFreeFactories());
        _canResearchTech.SetActive(IsCanResearchTech());
        _notChoosedFocus.SetActive(TargetCountry.Politics.ExecutingFocus == null);
        _countryIsWar.gameObject.SetActive(Diplomacy.Instance.CountryIsAtWar(TargetCountry));
    }

    private bool IsCanResearchTech()
    {
        var openedTechnologies = TargetCountry.Research.GetOpenedTechnologies();
        return TechnologiesManagerSO.GetAllTechs().Exists(tech => (!openedTechnologies.Contains(tech) && tech.OpenCost < TargetCountry.Research.ResearchPointCount));
    }

    private bool IsHaveFreeFactories()
    {
        var militaryFactoryNotUse = TargetCountry.CountryFabrication.GetNotUseMilitaryFactories().Count;
        var militaryFactory = TargetCountry.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory).Count;
        return ((militaryFactory - militaryFactoryNotUse) != militaryFactory);
    }
}
