using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipInTacticalFleetUnitSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _shipName;
    [SerializeField] private Image _shipImage;
    [SerializeField] private Button _deleteButton;


    public void RefreshUI(Ship ship, FleetUI fleetUI)
    {
        _shipName.text = ship.Name;
        _deleteButton.onClick.AddListener(delegate
        {
            fleetUI.ActiveRemoveShipMenu(ship);
        });
    }
}
