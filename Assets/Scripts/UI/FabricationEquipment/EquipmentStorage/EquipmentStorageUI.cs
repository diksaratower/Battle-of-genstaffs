using UnityEngine;
using UnityEngine.UI;


public class EquipmentStorageUI : MonoBehaviour
{
    [SerializeField] private EquipmentStorageSlotUI _slotPrefab;
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private Toggle _shortOrDetailToggle;

    private bool _shortView => _shortOrDetailToggle.isOn;
    private Country _country => Player.CurrentCountry;

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_shortView)
        {
            CreateTypeSlotView(EquipmentType.Rifle, "Винтовки");
            CreateTypeSlotView(EquipmentType.Tank, "Танки");
            CreateTypeSlotView(EquipmentType.Artillery, "Артиллерия");
        }
    }

    private void CreateTypeSlotView(EquipmentType equipmentType, string typeName)
    {
        var typeImage = GetOverageEquipmentTypeSprite(equipmentType);
        var slot = Instantiate(_slotPrefab, _slotsParent);
        slot.RefreshUI(typeName, typeImage, equipmentType, _country.EquipmentStorage);
    }

    private Sprite GetOverageEquipmentTypeSprite(EquipmentType equipmentType)
    {
        var equipment = EquipmentManagerSO.GetAllEquipment().Find(eq => eq.EqType == equipmentType);
        return equipment.EquipmentImage;
    }
}
