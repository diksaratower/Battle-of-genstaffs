using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FleetUI : MonoBehaviour
{
    [SerializeField] private Transform _fleetSlotsUIParent;
    [SerializeField] private FreeShipsSlotUI _freeShipSlotUIPrefab;
    [SerializeField] private TacticalFleetUnitSlotUI _tacticalFleetUnitSlotUIPrefab;
    [SerializeField] private TextMeshProUGUI _shipsCountText;
    [SerializeField] private Button _createTacticalFleetUnit;
    [SerializeField] private RemoveShipMenuUI _removeShipMenu;
    [SerializeField] private FleetMarineRegionUI _regionUIPrefab;
    [SerializeField] private Transform _regionsUIParent;
    [SerializeField] private GameObject _marineRegions;

    private List<FleetUISlot> _fleetSlotsUI = new List<FleetUISlot>();


    private void Start()
    {
        SetUpReactionToChangeShipsCount();

        SetUpCreatingTacticalUnits();
        RefreshUI();
        CreateMarineRegionsUI();
    }

    private void Update()
    {
        _shipsCountText.text = $"В нашем славном флоте {Map.Instance.MarineRegions.Ships.Count} кораблей!";
    }

    private void OnEnable()
    {
        _marineRegions.SetActive(true);
    }

    private void OnDisable()
    {
        if (_marineRegions)
        {
            _marineRegions.SetActive(false);
        }
    }

    public void AddSelectedToTacticalUnit(TacticalFleetUnitSlotUI tacticalFleetUnitUI)
    {
        var freeShipsUI = GetFretShipsUIs();
        foreach (var shipUI in freeShipsUI)
        {
            if (shipUI.IsSelected)
            {
                tacticalFleetUnitUI.AddShipToTacticalUnit(shipUI);
            }
        }
        RefreshUI();
    }

    public void ActiveRemoveShipMenu(Ship ship)
    {
        _removeShipMenu.gameObject.SetActive(true);
        _removeShipMenu.SetShip(ship);
    }

    public void RefreshUI()
    {
        var lastSelectedShips = GetSelectedFreeShips();
    
        ClearUI();
        var alreadyAdded = new List<Ship>();
        foreach (var tacticalUnit in Player.CurrentCountry.Fleet.TacticalFleetUnits)
        {
            AddTacticalUnitUI(tacticalUnit);
            alreadyAdded.AddRange(tacticalUnit.GetShips());
        }
        foreach (var ship in Map.Instance.MarineRegions.Ships)
        {
            if (ship.Country == Player.CurrentCountry)
            {
                if (alreadyAdded.Contains(ship) == false)
                {
                    AddFreeShipUI(ship);
                }
            }
        }
        RestoreSelectedShips(lastSelectedShips);
    }

    private void ClearUI()
    {
        _fleetSlotsUI.ForEach(slotUI => { slotUI.DestroySlot(); });
        _fleetSlotsUI.Clear();
    }

    private void SetUpCreatingTacticalUnits()
    {
        _createTacticalFleetUnit.onClick.AddListener(delegate
        {
            var unit = new TacticalFleetUnit();
            unit.Name = $"Тактическое соеденение ({Player.CurrentCountry.Fleet.TacticalFleetUnits.Count + 1})";
            Player.CurrentCountry.Fleet.TacticalFleetUnits.Add(unit);
            unit.OnChangeUnitСompound += delegate
            {
                RefreshUI();
            };
            RefreshUI();
        });
    }

    private void CreateMarineRegionsUI()
    {
        foreach (var region in Map.Instance.MarineRegions.MarineRegionsList)
        {
            var regionUI = Instantiate(_regionUIPrefab, _regionsUIParent);
            regionUI.SetTarget(region);
        }
    }

    private void SetUpReactionToChangeShipsCount()
    {
        Map.Instance.MarineRegions.OnCreateShip += delegate
        {
            RefreshUI();
        };
        Map.Instance.MarineRegions.OnRemoveShip += delegate
        {
            RefreshUI();
        };
    }

    private void RestoreSelectedShips(List<Ship> lastSelected)
    {
        foreach (var shipUI in GetFretShipsUIs())
        {
            if (lastSelected.Contains(shipUI.Ship))
            {
                shipUI.SetSelection(true);
            }
        }
    }

    private List<Ship> GetSelectedFreeShips()
    {
        var selectedShips = new List<Ship>();
        foreach (var shipUI in GetFretShipsUIs())
        {
            if (shipUI.IsSelected)
            {
                selectedShips.Add(shipUI.Ship);
            }
        }
        return selectedShips;
    }

    private List<FreeShipsSlotUI> GetFretShipsUIs()
    {
        return _fleetSlotsUI.FindAll(slot => slot is FreeShipsSlotUI).Cast<FreeShipsSlotUI>().ToList();
    }

    private void AddFreeShipUI(Ship ship)
    {
        var slotUI = Instantiate(_freeShipSlotUIPrefab, _fleetSlotsUIParent);
        slotUI.RefreshUI(ship, this);
        _fleetSlotsUI.Add(slotUI);
    }

    private void AddTacticalUnitUI(TacticalFleetUnit tacticalUnit)
    {
        var slotUI = Instantiate(_tacticalFleetUnitSlotUIPrefab, _fleetSlotsUIParent);
        slotUI.RefreshUI(tacticalUnit, this);
        _fleetSlotsUI.Add(slotUI);
    }
}


public interface FleetUISlot
{
    void DestroySlot();
}