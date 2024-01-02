using System;
using System.Collections.Generic;

public class TacticalFleetUnit
{
    public string Name;
    public Action OnChangeUnit—ompound;

    private List<Ship> _ships = new List<Ship>();

    public void AddShip(Ship ship)
    {
        _ships.Add(ship);
        OnChangeUnit—ompound?.Invoke();
        Map.Instance.MarineRegions.OnRemoveShip += (Ship sh) => 
        {
            if (ship == sh)
            {
                _ships.Remove(ship);
                OnChangeUnit—ompound?.Invoke();
            }
        };
    }

    public List<Ship> GetShips()
    {
        return new List<Ship>(_ships);
    }
}
