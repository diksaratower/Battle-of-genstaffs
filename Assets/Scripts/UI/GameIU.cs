using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GameIU : MonoBehaviour
{
    public static GameIU Instance { get; private set; }
    public bool BlockDivisionSelecting { get; set; }

    [SerializeField] private CombatUI _combatUIPrefab;
    [SerializeField] private Transform _combatsUIParent;
    [SerializeField] private DiplomacyWindowUI _diplomacyWindow;
    [SerializeField] private SelectedDivisionsViewUI _selectedDivisionsViewUI;
    [SerializeField] private CombatDetailsWindowUI _combatDetailsWindowUI;
    [SerializeField] private ExitMenuUI _exitMenu;
    [SerializeField] private AviationModeUI _aviationModeUI;

    private List<Division> _selectedDivisions = new List<Division>();
    private List<CombatUI> _combatsUI = new List<CombatUI>();


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Country.OnCountryCapitulated += OpenEventWindowCountryCapitulated;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _exitMenu.gameObject.SetActive(!_exitMenu.gameObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && !PointerConstainsUI())
        {
            _aviationModeUI.gameObject.SetActive(false);
            MoveSelectedDivisions();
            OpenDiplmaticWindow();
        }
        UpdateBattelsUI();
    }

    private void OnDestroy()
    {
        Country.OnCountryCapitulated -= OpenEventWindowCountryCapitulated;
    }

    public void OnDivisionClick(Division division, DivisionView divisionUI)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            DeselectedAllDivisions();
        }
        if(divisionUI.Selected)
        {
            DeselectDivisions(divisionUI.Divisions);
        }
        else
        {
            SelectDivisions(divisionUI.Divisions);
        }
    }

    public List<Division> GetSelectedDivisions()
    {
        return _selectedDivisions;
    }

    public void DeselectedAllDivisions()
    {
        var forDeselesct = new List<Division>();
        forDeselesct.AddRange(_selectedDivisions);
        DeselectDivisions(forDeselesct);
    }

    public void DeselectDivisions(List<Division> divisions)
    {
        divisions.ForEach(division => 
        {
            DeselectDivision(division);
        });
    }

    public void DeselectDivision(Division division)
    {
        if(_selectedDivisions.Contains(division) == false)
        {
            throw new ArgumentException();
        }
        _selectedDivisions.Remove(division);
    }

    public void SelectDivisions(List<Division> divisions)
    {
        foreach (var division in divisions) 
        {
            if (_selectedDivisions.Contains(division) == false)
            {
                _selectedDivisions.Add(division); 
            }
        }
    }
    
    public static string FloatToStringAddPlus(float value)
    {
        var result = value.ToString();
        if (value > 0)
        {
            result = "+" + result;
        }
        return result;
    }

    private void OpenDiplmaticWindow()
    {
        if (GetSelectedDivisions().Count == 0)
        {
            if (GameCamera.Instance.ChekHitToProvinceWithMousePosition(out var prov))
            {
                if (prov.Owner != Player.CurrentCountry && prov.Owner.ID != "null")
                {
                    _diplomacyWindow.gameObject.SetActive(true);
                    _diplomacyWindow.RefreshUI(prov.Owner, Player.CurrentCountry);
                }
            }
        }
    }

    private void MoveSelectedDivisions()
    {
        if (GetSelectedDivisions().Count > 0)
        {
            if (GameCamera.Instance.ChekHitToProvinceWithMousePosition(out var prov))
            {
                if (Player.CurrentCountry.ProvinceAllowedForCountryArmy(prov))
                {
                    var selectedDivisions = GetSelectedDivisions();
                    var allowedProvs = Player.CurrentCountry.GetAllowedProvForCountryDivisions();
                    foreach (var division in selectedDivisions)
                    {
                        if (division.CountyOwner == Player.CurrentCountry)
                        {
                            var extend = Input.GetKey(KeyCode.LeftShift);
                            division.MoveDivision(prov, extend, allowedProvs);
                        }
                    }
                }
            }
        }
    }

    private void UpdateBattelsUI()
    {
        var toDelete = _combatsUI.FindAll(combatUI => combatUI.IsCombatEnd == true);
        foreach (var combatUIToDestoy in toDelete)
        {
            _combatsUI.Remove(combatUIToDestoy);
            Destroy(combatUIToDestoy.gameObject);
        }
        foreach (var combat in DivisionCombat.Combats)
        {
            if (!_combatsUI.Exists(cUI => cUI.Target == combat))
            {
                var combatUI = Instantiate(_combatUIPrefab, _combatsUIParent);

                _combatsUI.Add(combatUI);
                combatUI.RefreshUI(combat);
            }
        }
    }

    private void OpenEventWindowCountryCapitulated(Country country, InvaderCountry invader)
    {
        EventsViewUI.Instance.ViewNewsEvent(EventsViewUI.GetCapitulationText(country, invader.Country));
    }

    private bool PointerConstainsUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public bool IsDivisionSelect(Division division)
    {
        return _selectedDivisions.Contains(division);
    }

    public static CombatDetailsWindowUI OpenCombatDetailsWindow()
    {
        var gUI = FindObjectOfType<GameIU>();
        gUI._combatDetailsWindowUI.gameObject.SetActive(true);
        return gUI._combatDetailsWindowUI;
    }
}
