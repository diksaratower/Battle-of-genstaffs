using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddEquipmentForFabricationSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _equipmentNameText;
    [SerializeField] private Image _equipmentImage;
    [SerializeField] private TextMeshProUGUI _equipmentCostText;
    [SerializeField] private Button _watchDetailedInformationButton;
    [SerializeField] private Button _addEquipmentButton;

    public void RefreshUI(IFabricatable item, CountryFabricationEquipmentUI fabricationEquipmentUI, AddEquipmentForFabricationUI addForFabricationUI)
    {
        _equipmentNameText.text = item.Name;
        _equipmentImage.sprite = item.ItemImage;
         _equipmentCostText.text = "Стоимость прозвд: " + item.FabricationCost;
        _addEquipmentButton.onClick.AddListener(() => {
            if (fabricationEquipmentUI.TargetCountry.CountryFabrication.GetNotUseMilitaryFactories().Count > 0)
            {
                fabricationEquipmentUI.TargetCountry.CountryFabrication.AddSlot(item,
                    new List<BuildingSlotRegion>() { fabricationEquipmentUI.TargetCountry.CountryFabrication.GetNotUseMilitaryFactories()[0] });
                addForFabricationUI.RefreshUI(fabricationEquipmentUI.TargetCountry);
            }
        });
    }
}
