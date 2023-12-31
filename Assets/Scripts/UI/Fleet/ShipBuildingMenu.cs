using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShipBuildingMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _shipTypeDropdown;
    [SerializeField] private List<ShipType> _availableShipTypes = new List<ShipType>();
    [SerializeField] private Button _createButton;

    private void Awake()
    {
        _shipTypeDropdown.options.Clear();
        foreach (ShipType shipType in _availableShipTypes)
        {
            _shipTypeDropdown.options.Add(new TMP_Dropdown.OptionData(shipType.ToString()));
            
        }
        _createButton.onClick.AddListener(ShipTypeDropdownOnClick);
    }

    private void ShipTypeDropdownOnClick()
    {
        ShipType shipType = _availableShipTypes[_shipTypeDropdown.value];

        Ship ship = new Battleship(Player.CurrentCountry);
        ship.Name = "Новый корабль";
    }
}
