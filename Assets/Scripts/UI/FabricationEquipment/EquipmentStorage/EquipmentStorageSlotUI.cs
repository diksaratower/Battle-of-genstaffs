using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentStorageSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _equipmentNameText;
    [SerializeField] private Image _eguipmentImage;
    [SerializeField] private TextMeshProUGUI _equipmentCountText;
    [SerializeField] private Color _notDeficitTextColor;
    [SerializeField] private Color _haveDeficitTextColor;

    private EquipmentType _equipmentType;
    private CountryEquipmentStorage _storage;

    public void RefreshUI(string typeName, Sprite typeImage, EquipmentType equipmentType, CountryEquipmentStorage storage)
    {
        _storage = storage;
        _equipmentType = equipmentType;
        _equipmentNameText.text = typeName;
        _eguipmentImage.sprite = typeImage;
    }

    private void Update()
    {
        if (_storage != null)
        {
            UpdateEquipmentCount(_equipmentType, _storage);
        }
    }

    private void UpdateEquipmentCount(EquipmentType equipmentType, CountryEquipmentStorage storage)
    {
        var equipmentCount = storage.GetEquipmentCountWithDeficit(equipmentType);
        if(equipmentCount > 0)
        {
            _equipmentCountText.color = _notDeficitTextColor;
        }
        if (equipmentCount < 0)
        {
            _equipmentCountText.color = _haveDeficitTextColor;
        }

        _equipmentCountText.text = equipmentCount.ToString();
    }
}
