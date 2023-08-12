using UnityEngine;


public class Building : ScriptableObject
{
    public float BuildCost = 1000f;
    public string Name;
    public Sprite BuildingImage;
    public string ID;
    public BuildingType BuildingType;
}

public abstract class BuildingInProvince : Building
{
    public abstract bool CanBuildInProvince(Province province);
}

public enum BuildingType
{
    Factory,
    MilitaryFactory,
    Airbase,
    Fortification,
    NavyBase
}

