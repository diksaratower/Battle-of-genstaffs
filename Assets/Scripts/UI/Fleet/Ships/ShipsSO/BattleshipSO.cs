using UnityEngine;


[CreateAssetMenu(fileName = "Battleship", menuName = "ScriptableObjects/Fleet/BattleshipSO", order = 1)]
public class BattleshipSO : ShipSO
{
    public override Ship CreateShip(Country country)
    {
        return new Battleship(country, "Линкор", this);
    }
}
