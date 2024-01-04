

public class Battleship : Ship
{


    public Battleship(Country country, string name, BattleshipSO battleshipSO) : base(ShipType.Battleship, country, name, battleshipSO.Power, battleshipSO.ShipImage)
    {
    }
}
