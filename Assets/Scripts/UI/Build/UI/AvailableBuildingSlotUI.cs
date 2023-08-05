using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AvailableBuildingSlotUI : MonoBehaviour
{
    public Building Target { get; private set; }

    [SerializeField] private Image _buildingImage;
    [SerializeField] private TextMeshProUGUI _buildingTypeNameText;
    [SerializeField] private Outline _slotOutLine;
    [SerializeField] private Button _selectButton;

    private CountryBuildUI _countryBuildUI;

    public void RefreshUI(Building building, CountryBuildUI countryBuildUI)
    {
        Target = building;
        _countryBuildUI = countryBuildUI;
        _buildingTypeNameText.text = building.Name;
        _buildingImage.sprite = building.BuildingImage;
        _selectButton.onClick.AddListener(delegate 
        {
            countryBuildUI.SetSelectedBuilding(this);
        });
        countryBuildUI.OnChangeSelectionBuilding += UpdateSelection;
    }

    private void UpdateSelection(AvailableBuildingSlotUI slot)
    {
        if (slot == this)
        {
            _slotOutLine.enabled = true;
        }
        else
        {
            _slotOutLine.enabled = false;
        }
    }

    private void OnDestroy()
    {
        _countryBuildUI.OnChangeSelectionBuilding -= UpdateSelection;
    }
}
