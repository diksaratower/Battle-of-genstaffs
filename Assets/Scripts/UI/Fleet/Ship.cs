

public abstract class Ship
{
    public int Power { get; set; }
    public int Armor { get; set; }
    public ShipType Type { get; }
    public Country Country { get; }
    public string Name;

    public Ship(ShipType shipType, Country country)
    {
        Type = shipType;
        Country = country;
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