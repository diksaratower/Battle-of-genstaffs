using System;


[Serializable]
public class DiplomaticRelationsWithCountry
{
    public int Relation;
    public bool IsAnAlly;
    public bool IsWar { get { return Diplomacy.Instance.CountriesIsWar(CountryA, CountryB); } }
    public Country CountryA { get; }
    public Country CountryB { get; }
    
    public DiplomaticRelationsWithCountry(Country countryA, Country countryB, int relation)
    {
        if (countryA == countryB)
        {
            throw new ArgumentException();
        }
        CountryA = countryA;
        CountryB = countryB;
        Relation = relation;
    }

    public bool IsThisRelation(Country countryA, Country countryB)
    {
        if((CountryA == countryA || CountryA == countryB) && (CountryB == countryA || CountryB == countryB))
        {
            return true;
        }
        return false;
    }

    public bool IsCountryRelation(Country country)
    {
        if (CountryA == country || CountryB == country)
        {
            return true;
        }
        return false;
    }
}
