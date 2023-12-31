using System.Collections.Generic;
using UnityEngine;


public class MarineRegion : MonoBehaviour
{
    public List<Province> Provinces = new List<Province>();
    public List<MarineRegion> Contacts = new List<MarineRegion>();
    public List<Ship> Ships = new List<Ship>();

    public bool CountryIsDominate(Country country)
    {
        float countryPower = 0;
        float enemyPower = 0;
        foreach (Ship ship in Ships)
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
}
