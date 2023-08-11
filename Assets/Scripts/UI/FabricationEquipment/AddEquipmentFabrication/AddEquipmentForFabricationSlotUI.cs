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

    public void RefreshUI(Technology technology, CountryFabricationEquipmentUI fabricationEquipmentUI, AddEquipmentForFabricationUI addForFabricationUI)
    {
        if(technology is not EquipmentTechnology)
        {
            throw new System.Exception("This tech not fit for fabrication.");
        }
        _equipmentNameText.text = technology.TechName;
        _equipmentImage.sprite = technology.TechImage;
         _equipmentCostText.text = "Стоимость прозвд: " + (technology as EquipmentTechnology).UnlockEquipment.FabricationCost;
        _addEquipmentButton.onClick.AddListener(() => {
            if (fabricationEquipmentUI.TargetCountry.CountryFabrication.GetNotUseMilitaryFactories().Count > 0)
            {
                fabricationEquipmentUI.TargetCountry.CountryFabrication.AddSlot(technology.ID,
                    new List<BuildingSlotRegion>() { fabricationEquipmentUI.TargetCountry.CountryFabrication.GetNotUseMilitaryFactories()[0] });
                addForFabricationUI.RefreshUI(fabricationEquipmentUI.TargetCountry);
            }
        });
    }
}
