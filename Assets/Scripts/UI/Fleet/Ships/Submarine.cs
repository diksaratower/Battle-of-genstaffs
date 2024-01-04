
public class Submarine : Ship
{
    public Submarine(Country country, string name, SubmarineSO submarineSO) : base(ShipType.Submarine, country, name, submarineSO.Power, submarineSO.ShipImage)
    {
    }
}