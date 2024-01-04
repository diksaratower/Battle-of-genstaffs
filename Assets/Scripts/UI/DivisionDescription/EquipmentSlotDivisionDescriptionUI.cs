using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotDivisionDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _equipmentNameText;
    [SerializeField] private Image _equipmentImage;
    [SerializeField] private TextMeshProUGUI _equipmentCountText;


    public void RefreshUI(EquipmentCountIdPair equipment)
    {
        _equipmentNameText.text = equipment.Equipment.Name;
        _equipmentImage.sprite = equipment.Equipment.EquipmentImage;
        _equipmentCountText.text = equipment.Count.ToString();
    }
}
