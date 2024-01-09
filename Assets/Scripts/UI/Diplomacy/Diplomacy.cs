using System;
using System.Collections.Generic;
using UnityEngine;


public class Diplomacy : MonoBehaviour
{
    public static Diplomacy Instance { get; private set; }
    public List<War> Wars = new List<War>();
    public List<GuaranteeIndependence> GuaranteesIndependences = new List<GuaranteeIndependence>();

    [SerializeField] private List<DiplomaticRelationsWithCountry> _diplomaticRelations = new List<DiplomaticRelationsWithCountry>();


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        var forRemove = new List<War>();
        foreach (var war in Wars) 
        {
            if (war.NeedEnd())
            {
                war.EndWar();
                forRemove.Add(war);
            }
        }
        Wars.RemoveAll(war => forRemove.Contains(war));
    }

    public void DeclareWar(Country aggressor, Country victim)
    {
        var war = new War();
        war.AddToWar(aggressor, WarMemberType.Aggressor);
        war.AddToWar(victim, WarMemberType.Defender);
        victim.CountryDiplomacy.OnDeclaredWarToCountry?.Invoke(war);
        UseGuarantees(aggressor, victim, war);
        Wars.Add(war);
    }

    public List<Country> GetCountryWarEnemies(Country country)
    {
        var result = new List<Country>();
        foreach (var mapCountry in Map.Instance.Countries)
        {
            if (mapCountry != country)
            {
                if (GetRelationWithCountry(country, mapCountry).IsWar == true)
                {
                    result.Add(mapCountry);
                }
            }
        }
        return result;
    }

    private void UseGuarantees(Country aggressor, Country victim, War war)
    {
        var guaranteeIndependences = GuaranteesIndependences.FindAll(guarantee => guarantee.Target == victim && guarantee.Guaranter != aggressor);
        foreach (var guaranteeIndependence in guaranteeIndependences)
        {
            war.AddToWar(guaranteeIndependence.Guaranter, WarMemberType.Defender);
        }
    }

    public bool HaveGuaranteeIndependence(Country guaranter, Country target)
    {
        return GuaranteesIndependences.Exists(guarantee => guarantee.Guaranter == guaranter && guarantee.Target == target);
    }

    public void GuaranteeIndependence(Country guaranter, Country target)
    {
        if (HaveGuaranteeIndependence(guaranter, target))
        {
            return;
        }
        var guarantee = new GuaranteeIndependence(guaranter, target);
        GuaranteesIndependences.Add(guarantee);
    }

    public DiplomaticRelationsWithCountry GetRelationWithCountry(Country countryA, Country countryB)
    {
        if (countryA == countryB)
        {
            throw new ArgumentException();
        }
        if (_diplomaticRelations.Exists(c => c.IsThisRelation(countryA, countryB) == true))
        {
            return _diplomaticRelations.Find(c => c.IsThisRelation(countryA, countryB) == true);
        }
        var relation = new DiplomaticRelationsWithCountry(countryA, countryB, 0);
        _diplomaticRelations.Add(relation);
        return relation;
    }

    public bool CountriesIsWar(Country country, Country countryTwo)
    {
        var countriesWar = Wars.Find(war => (war.CountryIsWarMember(country) && war.CountryIsWarMember(countryTwo)));
        if (countriesWar == null)
        {
            return false;
        }
        
        if (countriesWar.GetCountryIsWarMember(country).MemberType != countriesWar.GetCountryIsWarMember(countryTwo).MemberType)
        {
            return true;
        }
        return false;
    }

    public bool CountryIsAtWar(Country country)
    {
        return Wars.Exists(war => war.CountryIsWarMember(country));
    }

    public List<War> GetCountryWars(Country country)
    {
        return Wars.FindAll(war => war.CountryIsWarMember(country));
    }
}

public class GuaranteeIndependence
{
    public Country Guaranter { get; }
    public Country Target { get; }

    public GuaranteeIndependence(Country guaranter, Country target)
    {
        Guaranter = guaranter;
        Target = target;
    }
}