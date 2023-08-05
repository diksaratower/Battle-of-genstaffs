using UnityEngine;


public class Building : ScriptableObject
{
    public float BuildCost = 1000f;
    public string Name;
    public Sprite BuildingImage;
    public string ID;
    public BuildingType BuildingType;
}

public enum BuildingType
{
    Factory,
    MilitaryFactory,
    Airbase
}
