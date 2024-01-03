using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TacticalFleetUnitSlotUI : MonoBehaviour, FleetUISlot, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _tacticalUnitName;
    [SerializeField] private Image _tacticalUnitImage;
    [SerializeField] private Transform _shipsInUnitParent;
    [SerializeField] private ShipInTacticalFleetUnitSlotUI _shipSlotUIPrefab;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _openOrdersMenu;
    [SerializeField] private TextMeshProUGUI _openOrdersMenuButtonText;
    [SerializeField] private TacticalUnitOrdersMenuUI _unitOrdersMenu;

    private FleetUI _fleetUI;
    private TacticalFleetUnit _tacticalFleetUnit;


    private void Start()
    {
        SetUpUsingOrdersMenu();
        SetUpRemovingTacticalUnit();
    }

    public void RefreshUI(TacticalFleetUnit tacticalFleetUnit, FleetUI fleetUI)
    {
        _tacticalFleetUnit = tacticalFleetUnit;
        _fleetUI = fleetUI;
        _unitOrdersMenu.SetTargetUnit(tacticalFleetUnit);

        _tacticalUnitImage.sprite = Player.CurrentCountry.Flag;
        _tacticalUnitName.text = tacticalFleetUnit.Name;
        foreach (var ship in tacticalFleetUnit.GetShips())
        {
            var shipUI = Instantiate(_shipSlotUIPrefab, _shipsInUnitParent);
            shipUI.RefreshUI(ship, fleetUI, _tacticalFleetUnit);
        }
    }

    public void AddShipToTacticalUnit(FreeShipsSlotUI shipUI)
    {
        _tacticalFleetUnit.AddShip(shipUI.Ship);
    }

    void FleetUISlot.DestroySlot()
    {
        Destroy(gameObject);
    }


    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            _fleetUI.AddSelectedToTacticalUnit(this);
        }
    }

    private void SetUpUsingOrdersMenu()
    {
        _openOrdersMenu.onClick.AddListener(delegate
        {
            _unitOrdersMenu.gameObject.SetActive(!_unitOrdersMenu.gameObject.activeSelf);
            if (_unitOrdersMenu.gameObject.activeSelf == true)
            {
                _openOrdersMenuButtonText.text = @"Приказы \/";
            }
            else
            {
                _openOrdersMenuButtonText.text = @"Приказы /\";
            }
        });
    }

    private void SetUpRemovingTacticalUnit()
    {
        _deleteButton.onClick.AddListener(delegate
        {
            _tacticalFleetUnit.RemoveAllShips();
            Player.CurrentCountry.Fleet.TacticalFleetUnits.Remove(_tacticalFleetUnit);
            _fleetUI.RefreshUI();
        });
    }
}
