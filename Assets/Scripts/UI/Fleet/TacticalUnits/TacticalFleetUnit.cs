using System;
using System.Collections.Generic;
using UnityEngine;

public class TacticalFleetUnit
{
    public string Name;
    public Action OnChangeUnitÑompound;
    public FleetOrders Order { get; private set; } = FleetOrders.None;
    public int WorkingSeasCount => _targetRegionsDomination.Count;

    private List<Ship> _ships = new List<Ship>();
    private List<MarineRegion> _targetRegionsDomination = new List<MarineRegion>();

    public void AddShip(Ship ship)
    {
        _ships.Add(ship);
        OnChangeUnitÑompound?.Invoke();
        Map.Instance.MarineRegions.OnRemoveShip += (Ship sh) => 
        {
            if (ship == sh)
            {
                RemoveShip(ship);
            }
        };
        if (Order == FleetOrders.Domination && _targetRegionsDomination.Count > 0)
        {
            RecalculateShipsPositions(_targetRegionsDomination, _ships);
        }
    }

    public void RemoveShip(Ship ship)
    {
        _ships.Remove(ship);
        OnChangeUnitÑompound?.Invoke();
        if (Order == FleetOrders.Domination && _targetRegionsDomination.Count > 0)
        {
            RecalculateShipsPositions(_targetRegionsDomination, _ships);
        }
    }

    public void AddRegionDominationOrder(MarineRegion targetRegion)
    {
        Order = FleetOrders.Domination;
        _targetRegionsDomination.Add(targetRegion);
        foreach (var ship in _ships)
        {
            ship.ShipPosition = targetRegion;
        }
        RecalculateShipsPositions(_targetRegionsDomination, _ships);
    }

    public void DropOrder()
    {
        Order = FleetOrders.None;
        _targetRegionsDomination.Clear();
    }

    public bool IsWorkingInRegion(MarineRegion region)
    {
        return _targetRegionsDomination.Contains(region);
    }

    private void RecalculateShipsPositions(List<MarineRegion> marineRegions, List<Ship> ships)
    {
        if (ships.Count == 0) return;
        var shipsList = new List<Ship>();
        shipsList.AddRange(ships);
        int DivCount = ships.Count;
        var plusValue = 0f;
        if (marineRegions.Count <= DivCount)
        {
            SetShipsToRegionsMoreShips(marineRegions, shipsList);
        }

        if (marineRegions.Count > DivCount)
        {
            plusValue = ((float)marineRegions.Count / (float)DivCount);
        }


        var divisionNumber = 0f;
        for (int i = 0; i < marineRegions.Count; i++)
        {
            var divisionIndex = Mathf.RoundToInt(divisionNumber);
            if (divisionIndex >= marineRegions.Count) break;
            if (shipsList.Count == 0) continue;
            if (marineRegions[divisionIndex] == null) continue;
            if (shipsList[0].ShipPosition == marineRegions[divisionIndex]) continue;
            

            shipsList[0].ShipPosition = marineRegions[divisionIndex];

            shipsList.Remove(shipsList[0]);
            divisionNumber += plusValue;
        }
    }

    private void SetShipsToRegionsMoreShips(List<MarineRegion> marineRegions, List<Ship> ships)
    {
        while (ships.Count > 0)
        {
            for (int i = 0; i < marineRegions.Count; i++)
            {
                if (ships.Count == 0)
                {
                    break;
                }
                ships[0].ShipPosition = marineRegions[i];
                ships.Remove(ships[0]);
            }
        }
    }

    public List<Ship> GetShips()
    {
        return new List<Ship>(_ships);
    }

    public void RemoveAllShips()
    {
        Order = FleetOrders.None;
        _ships.Clear();
    }
}

public enum FleetOrders
{
    None,
    Domination
}