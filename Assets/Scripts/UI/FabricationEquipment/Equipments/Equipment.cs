using UnityEngine;


public class Equipment : ScriptableObject
{
    public float FabricationCost;
    public string Name;
    public string ID;
    public Sprite EquipmentImage;
    public EquipmentType EqType;
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