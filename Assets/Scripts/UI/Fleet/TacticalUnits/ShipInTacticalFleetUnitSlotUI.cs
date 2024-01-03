using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipInTacticalFleetUnitSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _shipName;
    [SerializeField] private Image _shipImage;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _removeShipFromUnit;

    public void RefreshUI(Ship ship, FleetUI fleetUI, TacticalFleetUnit tacticalUnit)
    {
        _shipName.text = ship.Name;
        _deleteButton.onClick.AddListener(delegate
        {
            fleetUI.ActiveRemoveShipMenu(ship);
        });
        _removeShipFromUnit.onClick.AddListener(delegate 
        {
            tacticalUnit.RemoveShip(ship);
        });
    }
}
