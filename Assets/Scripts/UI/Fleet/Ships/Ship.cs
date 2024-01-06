

using UnityEngine;

public abstract class Ship
{
    public float Power { get; }
    public ShipType Type { get; }
    public Country Country { get; }
    public Sprite ShipImage { get; }
    public string ShipTypeID { get; private set; }

    public string Name;
    public MarineRegion ShipPosition;

    public Ship(Country country, string name, ShipSO shipSO)
    {
        Country = country;
        Name = name;
        Power = shipSO.Power;
        ShipImage = shipSO.ShipImage;
        ShipTypeID = shipSO.ID;
        Type = shipSO.ShipType;
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