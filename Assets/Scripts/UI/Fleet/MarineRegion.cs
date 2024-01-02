using System.Collections.Generic;
using UnityEngine;


public class MarineRegion : MonoBehaviour
{
    public string Name;
    public List<Province> Provinces = new List<Province>();
    public List<MarineRegion> Contacts = new List<MarineRegion>();
    public Transform Center { get; private set; }
    
    [SerializeField] private Transform _center;


    private void Awake()
    {
        if (_center != null)
        {
            Center = _center;
        }
        else
        {
            Center = transform;
        }
    }

    public bool CountryIsDominate(Country country)
    {
        float countryPower = 0;
        float enemyPower = 0;
        foreach (Ship ship in GetRegionShips())
        {
            if (ship.Country == country)
            {
                countryPower += ship.Power;
            }
            else
            {
                enemyPower += ship.Power;
            }
        }
        return countryPower > enemyPower;
    }

    public List<Ship> GetRegionShips()
    {
        return Map.Instance.MarineRegions.Ships.FindAll(ship => ship.ShipPosition == this);
    }
}
