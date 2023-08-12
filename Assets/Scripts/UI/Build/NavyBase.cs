using UnityEngine;


[CreateAssetMenu(fileName = "NavyBase", menuName = "ScriptableObjects/Build/Buildings/NavyBase", order = 1)]
public class NavyBase : BuildingInProvince
{
    public override bool CanBuildInProvince(Province province)
    {
        if (province.Contacts.Count < 6 && province.Buildings.Exists(building => building.TargetBuilding.BuildingType == BuildingType.NavyBase) == false)
        {
            return true;
        }
        return false;
    }
}
