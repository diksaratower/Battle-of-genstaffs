using UnityEngine;


public abstract class ShipSO : ScriptableObject
{
    public float Power;
    public Sprite ShipImage;
    public string Name;

    public abstract Ship CreateShip();
}
