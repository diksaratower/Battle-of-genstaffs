using System.Collections.Generic;
using UnityEngine;


public class MarineRegion : MonoBehaviour
{
    public string Name;
    public List<Province> Provinces = new List<Province>();
    public List<MarineRegion> Contacts = new List<MarineRegion>();
    public Transform Center { get; private set; }
    
    [SerializeField] private Transform _center;
    
    private MeshRenderer _regionRenderer;

    private void Awake()
    {
        _regionRenderer = GetComponent<MeshRenderer>();
        if (_center != null)
        {
            Center = _center;
        }
        else
        {
            Center = transform;
        }
    }

    private void Update()
    {
        IsDominate(Player.CurrentCountry, out var percentDomination);
        if (percentDomination == 0f)
        {
            ColoredRegion(Map.Instance.MarineRegions.NeutralDominationColor);
        }
        if (percentDomination > 0.5f)
        {
            ColoredRegion(Map.Instance.MarineRegions.OurDominationColor);
        }
        if (percentDomination <= 0.5f && percentDomination != 0)
        {
            ColoredRegion(Map.Instance.MarineRegions.EnemyDominationColor);
        }
    }

    public bool IsDominate(Country country, out float percentDomination)
    {
        var isDominate = IsDominate(country, new List<Country>(), out var percent, out _, out _);
        percentDomination = percent;
        return isDominate;
    }

    public bool IsDominate(Country country, out float percentDomination, out float enemyPower, out float countryPower)
    {
        var isDominate = IsDominate(country, new List<Country>(), out var percent, out enemyPower, out countryPower);
        percentDomination = percent;
        return isDominate;
    }

    public bool IsDominate(Country country, List<Country> countryEnemies, out float percentDomination, out float enemyDomination, out float countryDomination)
    {
        GetBaseDomination(country, countryEnemies, out var countryPower, out var enemyPower);

        if (enemyPower == 0)
        {
            foreach (var enemy in countryEnemies)
            {
                enemyPower += GetInfluenceOfPhantomShips(enemy);
            }
        }
        if (countryPower == 0)
        {
            countryPower += GetInfluenceOfPhantomShips(country);
        }
        var isDominate = CalculateDominationWithPowers(countryPower, enemyPower, out var percent);
        percentDomination = percent;
        enemyDomination = enemyPower;
        countryDomination = countryPower;
        return isDominate;
    }

    public List<Ship> GetRegionShips()
    {
        return Map.Instance.MarineRegions.Ships.FindAll(ship => ship.ShipPosition == this);
    }

    private void GetBaseDomination(Country country, List<Country> countryEnemies, out float countryBasePower, out float enemyBasePower)
    {
        var enemyPower = 0f;
        var countryPower = 0f;
        foreach (var ship in GetRegionShips())
        {
            if (IsNeed�onsideShip(ship) == false)
            {
                continue;
            }

            if (ship.Country == country)
            {
                countryPower += ship.Power;
            }
            if (countryEnemies.Contains(ship.Country))
            {
                enemyPower += ship.Power;
            }
        }
        countryBasePower = countryPower;
        enemyBasePower = enemyPower;
    }

    private bool CalculateDominationWithPowers(float countryPower, float enemyPower, out float percentDomination)
    {
        if (countryPower == 0 && enemyPower == 0)
        {
            percentDomination = 0;
            return false;
        }
        if (countryPower == 0)
        {
            percentDomination = 0;
            return false;
        }
        if (enemyPower == 0)
        {
            percentDomination = 1;
            return true;
        }
        if (countryPower == enemyPower)
        {
            percentDomination = 0.5f;
            return false;
        }
        percentDomination = 0;
        if (countryPower > enemyPower)
        {
            percentDomination = countryPower / enemyPower;
        }
        if (countryPower < enemyPower)
        {
            percentDomination = enemyPower / countryPower;
        }

        return countryPower > enemyPower;
    }

    private float GetInfluenceOfPhantomShips(Country country)
    {
        var phantomFactor = 0f;
        var tacticalUnits = country.Fleet.TacticalFleetUnits.FindAll(unit => unit.IsWorkingInRegion(this));
        foreach (var unit in tacticalUnits) 
        {
            foreach (var ship in unit.GetShips())
            {
                phantomFactor += ship.Power;
            }
        }
        phantomFactor *= 0.01f;
        return phantomFactor;
    }

    private void ColoredRegion(Material color)
    {
        //_regionRenderer.material.
        _regionRenderer.material = color;
    }

    private bool IsNeed�onsideShip(Ship ship)
    {
        var tacticalUnit = ship.Country.Fleet.TacticalFleetUnits.Find(unit => unit.GetShips().Contains(ship));
        if (tacticalUnit != null)
        {
            if (tacticalUnit.Order == FleetOrders.Domination)
            {
                return true;
            }
        }
        return false;
    }
}