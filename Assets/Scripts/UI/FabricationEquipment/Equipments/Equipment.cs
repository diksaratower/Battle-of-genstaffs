using UnityEngine;


public class Equipment : ScriptableObject, IFabricatable
{
    public float FabricationCost;
    public string Name;
    public string ID;
    public Sprite EquipmentImage;
    public EquipmentType EqType;

    float IFabricatable.FabricationCost => FabricationCost;
    string IFabricatable.ID => ID;
    string IFabricatable.Name => Name;
    Sprite IFabricatable.ItemImage => EquipmentImage;
}

public enum EquipmentType
{
    Rifle,
    Tank,
    Truck,
    Manpower,
    Artillery,
    Fighter
}

public interface IGroundCombatEquipment
{
    public float Attack { get; }
    public float Defens { get; }
}