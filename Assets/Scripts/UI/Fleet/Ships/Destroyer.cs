

public class Destroyer : Ship
{
    public Destroyer(Country country, string name, DestroyerSO destroyerSO) : base(ShipType.Destroyer, country, name, destroyerSO.Power, destroyerSO.ShipImage)
    {
    }
}
