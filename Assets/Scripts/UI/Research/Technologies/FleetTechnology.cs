using UnityEngine;


[CreateAssetMenu(fileName = "FleetTechnology", menuName = "ScriptableObjects/Technologies/FleetTechnology", order = 1)]
public class FleetTechnology : Technology
{
    public ShipSO UnlockShip;
    public override string ID => UnlockShip.ID;
    public override Sprite TechImage => UnlockShip.ShipImage;
    public override string TechName => UnlockShip.Name;

    public override bool CanFabricatable => true;
    public override IFabricatable Fabricatable => UnlockShip;
}
