using UnityEngine;


[CreateAssetMenu(fileName = "Fortification", menuName = "ScriptableObjects/Build/Buildings/Fortification", order = 1)]
public class Fortification : BuildingInProvince
{
    public override bool CanBuildInProvince(Province province)
    {
        return true;
    }
}
