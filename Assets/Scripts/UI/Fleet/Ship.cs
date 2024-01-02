

public abstract class Ship
{
    public int Power { get; set; }
    public int Armor { get; set; }
    public ShipType Type { get; }
    public Country Country { get; }
    public string Name;
    public MarineRegion ShipPosition;

    public Ship(ShipType shipType, Country country, string name)
    {
        Type = shipType;
        Country = country;
        Name = name;
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