using UnityEngine;


[CreateAssetMenu(fileName = "Destroyer", menuName = "ScriptableObjects/Fleet/DestroyerSO", order = 1)]
public class DestroyerSO : ShipSO
{
    public override Ship CreateShip(Country country)
    {
        return new Destroyer(country, "Ёсминец", this);
    }
}
