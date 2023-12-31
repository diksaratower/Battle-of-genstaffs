using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ArmiesUI : MonoBehaviour
{
    [SerializeField] private GameIU _gameIU;
    [SerializeField] private Button _createArmyButton;
    [SerializeField] private HorizontalLayoutGroup _armiesLayoutGroup;
    [SerializeField] private ArmyUI _armyUiPrefab;
    [SerializeField] private GameObject _planPanel;
    [SerializeField] private Button _createFrontPlanButton;
    [SerializeField] private Button _createAttackLineButton;
    [SerializeField] private Button _deleteAllFrontPlansButton;
    [SerializeField] private Button _createSeaLandingButton;

    private Country _country => Player.CurrentCountry;
    private List<ArmyUI> _armiesUI = new List<ArmyUI>();


    private void Start()
    {
        _createArmyButton.onClick.AddListener(delegate 
        {
            var divisions = GetSelectedNotArmiesDivisions();
            if (divisions.Count > 0)
            {
                _country.CountryArmies.AddArmy(divisions);
            }
        });
        _createFrontPlanButton.onClick.AddListener(delegate 
        {
            var armyUI = GetSelectedArmy();
            if (armyUI != null)
            {
                armyUI.CreationFrontUI = true;
            }
        });
        _createSeaLandingButton.onClick.AddListener(delegate 
        {
            var armyUI = GetSelectedArmy();
            if (armyUI != null)
            {
                armyUI.CreationSeaLanding = true;
            }
        });
        _deleteAllFrontPlansButton.onClick.AddListener(DeleteAllFrontPlansButtonClick);
        _country.CountryArmies.OnArmiesChanged += delegate
        {
            RefreshArmies();
        };
        RefreshArmies();
    }

    private void OnEnable()
    {
        RefreshArmies();
    }

    private void Update()
    {
        var selectedArmy = GetSelectedArmy();
        if (selectedArmy != null)
        {
            _createFrontPlanButton.interactable = !selectedArmy.CreationFrontUI;
            _createSeaLandingButton.interactable = !selectedArmy.CreationSeaLanding;
        }
        _createArmyButton.interactable = GetSelectedNotArmiesDivisions().Count > 0;
        if (_armiesUI.Find(arm => arm.Selected == true) != null)
        {
            _planPanel.SetActive(true);
        }
        else
        {
            _planPanel.SetActive(false);
        }
    }

    private void RefreshArmies()
    {
        _armiesUI.ForEach(arm => { Destroy(arm.gameObject); });
        _armiesUI.Clear();
        foreach (var army in _country.CountryArmies.Armies)
        {
            var armyUI = Instantiate(_armyUiPrefab, _armiesLayoutGroup.transform);
            armyUI.Owner = this;
            armyUI.TargetArmy = army;
            _armiesUI.Add(armyUI);
        }
    }

    private List<Division> GetSelectedNotArmiesDivisions()
    {
        var selectedNotArmiesDivisions = new List<Division>();
        var selectedDivsions = _gameIU.GetSelectedDivisions().FindAll(division => division.CountyOwner == Player.CurrentCountry);
        foreach (var division in selectedDivsions) 
        {
            if (_country.CountryArmies.IsDivisionsAttachedWithArmy(division) == false)
            {
                selectedNotArmiesDivisions.Add(division);
            }
        }
        return selectedNotArmiesDivisions;
    }

    private void DeleteAllFrontPlansButtonClick()
    {
        GetSelectedArmy().TargetArmy.StopWorkArmy();
    }

    private ArmyUI GetSelectedArmy()
    {
        return _armiesUI.Find(arm => arm.Selected == true);
    }

    public void DeselectAllArmies()
    {
        foreach (var armUI in _armiesUI)
        {
            armUI.Deselect();
        }
    }
}
