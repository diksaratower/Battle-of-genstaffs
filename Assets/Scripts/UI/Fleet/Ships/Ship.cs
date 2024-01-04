

using UnityEngine;

public abstract class Ship
{
    public float Power { get; }
    public ShipType Type { get; }
    public Country Country { get; }
    public Sprite ShipImage { get; }

    public string Name;
    public MarineRegion ShipPosition;

    public Ship(ShipType shipType, Country country, string name, float power, Sprite shipImage)
    {
        Type = shipType;
        Country = country;
        Name = name;
        Power = power;
        ShipImage = shipImage;
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