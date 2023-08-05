using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CountryFabricationEquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _equipmentName;
    [SerializeField] private Image _equipmentImage;
    [SerializeField] private TextMeshProUGUI _factoriesCountText;
    [SerializeField] private TextMeshProUGUI _fabricationPerHour;
    [SerializeField] private TextMeshProUGUI _equipmentInStorage;
    [SerializeField] private Button _addFactoryButton;
    [SerializeField] private Button _removeFactoryButton;
    [SerializeField] private Button _removeSlotButton;

    private CountryFabricationEquipmentSlot _fabricationSlot;
    private Country _country;

    public void RefreshSlot(CountryFabricationEquipmentSlot fabricationSlot, Country country)
    {
        _fabricationSlot = fabricationSlot;
        _country = country;
        var equipment = EquipmentManagerSO.GetEquipmentFromID(fabricationSlot.EquipmentID);
        _equipmentImage.sprite = equipment.EquipmentImage;
        _equipmentName.text = equipment.Name;
        UpdateFactoriesCount(fabricationSlot);
        _addFactoryButton.onClick.AddListener(() =>
        {
            foreach (var fac in country.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory))
            {
                if (!country.CountryFabrication.FactoryIsUses(fac))
                {
                    fabricationSlot.Factories.Add(fac);
                    UpdateFactoriesCount(fabricationSlot);
                    return;
                }
            }
        });
        _removeFactoryButton.onClick.AddListener(() =>
        {
            if (fabricationSlot.Factories.Count > 1)
            {
                fabricationSlot.Factories.Remove(fabricationSlot.Factories[fabricationSlot.Factories.Count - 1]);
                UpdateFactoriesCount(fabricationSlot);
            }
        });
        _removeSlotButton.onClick.AddListener(() => 
        {
            country.CountryFabrication.RemoveSlot(fabricationSlot);
        });
    }

    private void Update()
    {
        _fabricationPerHour.text = "Производится: " + _fabricationSlot.GetEquipmentFabricationPerHour() + " ед/час";
        _equipmentInStorage.text = _country.EquipmentStorage.GetEquipmentCountWithDeficit(EquipmentManagerSO.GetEquipmentFromID(_fabricationSlot.EquipmentID).EqType).ToString();
    }

    private void UpdateFactoriesCount(CountryFabricationEquipmentSlot fabricationSlot)
    {
        _factoriesCountText.text = fabricationSlot.Factories.Count.ToString();
    }
}
