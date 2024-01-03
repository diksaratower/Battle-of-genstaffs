

public abstract class Ship
{
    public float Power { get; set; }
    public float Armor { get; set; }
    public ShipType Type { get; }
    public Country Country { get; }
    public string Name;
    public MarineRegion ShipPosition;

    public Ship(ShipType shipType, Country country, string name)
    {
        Type = shipType;
        Country = country;
        Name = name;
        Power = 100f;
    }
}


public enum ShipType
{
    AircraftCarrier,
    Battleship,
    Cruiser,
    Destroyer,
    Submarine,
    TorpedoBoat,
    Minesweeper,
    Frigate,
    Corvette
}