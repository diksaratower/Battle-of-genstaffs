using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreeShipsSlotUI : MonoBehaviour, FleetUISlot
{
    public bool IsSelected { get; private set; }
    public Ship Ship { get; private set; }

    [SerializeField] private Image _shipImage;
    [SerializeField] private TextMeshProUGUI _divisionName;
    [SerializeField] private TextMeshProUGUI _regionPositionName;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Outline _selectedOutline;
    [SerializeField] private Button _selectButton;

    
    public void Awake()
    {
        _selectButton.onClick.AddListener(delegate 
        {
            SetSelection(!IsSelected);
        });
    }

    public void SetSelection(bool selected)
    {
        _selectedOutline.enabled = selected;
        IsSelected = selected;
    }

    public void RefreshUI(Ship ship, FleetUI fleetUI)
    {
        Ship = ship;
        _divisionName.text = ship.Name;
        _regionPositionName.text = ship.ShipPosition.Name;
        _deleteButton.onClick.AddListener(delegate 
        {
            fleetUI.ActiveRemoveShipMenu(ship);
        });
    }

    void FleetUISlot.DestroySlot()
    {
        Destroy(gameObject);
    }
}
