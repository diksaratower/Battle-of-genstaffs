using UnityEngine;


public abstract class ShipSO : ScriptableObject, IFabricatable
{
    public float Power;
    public Sprite ShipImage;
    public string Name;
    public string ID;
    public float BuildCost = 1000;
    public ShipType ShipType;

    float IFabricatable.FabricationCost => BuildCost;
    string IFabricatable.ID => ID;
    string IFabricatable.Name => Name;
    Sprite IFabricatable.ItemImage => ShipImage;

    public abstract Ship CreateShip(Country country);
}
