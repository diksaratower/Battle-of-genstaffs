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
    [SerializeField] private Image _fabricationFill;
    [SerializeField] private GameObject _fabricationFillGO;
    [SerializeField] private Color _equipmentCountDefaultColor;
    [SerializeField] private Color _equipmentCountNegativeColor;

    private CountryFabricationEquipmentSlot _fabricationSlot;
    private Country _country;

    public void RefreshSlot(CountryFabricationEquipmentSlot fabricationSlot, Country country)
    {
        _fabricationSlot = fabricationSlot;
        _country = country;
        _equipmentImage.sprite = fabricationSlot.Fabricatable.ItemImage;
        _equipmentName.text = fabricationSlot.Fabricatable.Name;
        UpdateFactoriesCount(fabricationSlot);
        _addFactoryButton.onClick.AddListener(() =>
        {
            var countryMilitaryFactories = country.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory);
            var tryAddCount = 1;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                tryAddCount = 10;
            }
            for (int i = 0; i < tryAddCount; i++)
            {
                foreach (var fac in countryMilitaryFactories)
                {
                    if (!country.CountryFabrication.FactoryIsUses(fac))
                    {
                        fabricationSlot.Factories.Add(fac);
                        UpdateFactoriesCount(fabricationSlot);
                        break;
                    }
                }
            }
        });
        _removeFactoryButton.onClick.AddListener(() =>
        {
            var tryRemoveCount = 1;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                tryRemoveCount = 10;
            }
            for (int i = 0; i < tryRemoveCount; i++)
            {
                if (fabricationSlot.Factories.Count > 1)
                {
                    fabricationSlot.Factories.Remove(fabricationSlot.Factories[fabricationSlot.Factories.Count - 1]);
                    UpdateFactoriesCount(fabricationSlot);
                }
            }
        });
        _removeSlotButton.onClick.AddListener(() => 
        {
            country.CountryFabrication.RemoveSlot(fabricationSlot);
        });
    }

    private void Update()
    {
        UpdateFabricationWithTime();
        UpdateFabricationFill();
        CountInStorageUpdate();
    }

    private void UpdateFabricationWithTime()
    {
        var fabricationPerHour = _fabricationSlot.GetEquipmentFabricationCountPerHour();
        if (fabricationPerHour >= 1f)
        {
            _fabricationPerHour.text = "Производится: " + fabricationPerHour + " ед/час";
        }
        if (fabricationPerHour < 1f)
        {
            _fabricationPerHour.text = "Производится: " + (fabricationPerHour * 24) + " ед/день";
        }
        if ((fabricationPerHour * 24) < 1f)
        {
            _fabricationPerHour.text = "Производится: " + ((fabricationPerHour * 24) * 30) + " ед/месяц";
        }
    }

    private void UpdateFabricationFill()
    {
        if (_fabricationSlot.GetEquipmentFabricationCountPerHour() < 0.05f)
        {
            _fabricationFillGO.SetActive(true);
            _fabricationFill.fillAmount = _fabricationSlot.GetFabricationPercent();
        }
        else
        {
            if (_fabricationFillGO.activeSelf == true)
            {
                _fabricationFillGO.SetActive(false);
            }
        }
    }

    private void CountInStorageUpdate()
    {
        if (_fabricationSlot.Fabricatable is Equipment)
        {
            if (_equipmentInStorage.gameObject.activeSelf == false)
            {
                _equipmentInStorage.gameObject.SetActive(true);
            }
            var equipmentType = (_fabricationSlot.Fabricatable as Equipment).EqType;
            var equipmentCount = _country.EquipmentStorage.GetEquipmentCountWithDeficit(equipmentType);
            _equipmentInStorage.text = equipmentCount.ToString();
            if (equipmentCount < 0)
            {
                _equipmentInStorage.color = _equipmentCountNegativeColor;
            }
            else
            {
                _equipmentInStorage.color = _equipmentCountDefaultColor;
            }
        }
        else
        {
            if (_equipmentInStorage.gameObject.activeSelf == true)
            {
                _equipmentInStorage.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateFactoriesCount(CountryFabricationEquipmentSlot fabricationSlot)
    {
        _factoriesCountText.text = fabricationSlot.Factories.Count.ToString();
    }
}
