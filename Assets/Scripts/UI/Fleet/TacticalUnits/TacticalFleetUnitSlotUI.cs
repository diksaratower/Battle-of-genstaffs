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

    private FleetUI _fleetUI;
    private TacticalFleetUnit _tacticalFleetUnit;

    public void RefreshUI(TacticalFleetUnit tacticalFleetUnit, FleetUI fleetUI)
    {
        _tacticalFleetUnit = tacticalFleetUnit;
        _fleetUI = fleetUI;

        _tacticalUnitImage.sprite = Player.CurrentCountry.Flag;
        _tacticalUnitName.text = tacticalFleetUnit.Name;
        foreach (var ship in tacticalFleetUnit.GetShips())
        {
            var shipUI = Instantiate(_shipSlotUIPrefab, _shipsInUnitParent);
            shipUI.RefreshUI(ship, fleetUI);
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
}
