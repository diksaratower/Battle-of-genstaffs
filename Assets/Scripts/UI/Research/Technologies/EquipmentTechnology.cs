using UnityEngine;


[CreateAssetMenu(fileName = "EquipmentTechnology", menuName = "ScriptableObjects/Technologies/EquipmentTechnology", order = 1)]
public class EquipmentTechnology : Technology
{
    public Equipment UnlockEquipment;
    public override string ID { get => UnlockEquipment.ID; }
    public override string TechName { get => UnlockEquipment.Name; }
    public override Sprite TechImage { get => UnlockEquipment.EquipmentImage; }
}
