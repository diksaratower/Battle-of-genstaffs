using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Diplomacy : MonoBehaviour
{
    public static Diplomacy Instance { get; private set; }
    public static List<War> Wars = new List<War>();

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
        Wars.Add(war);
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

public class War
{
    public Action OnEnd;

    private List<WarMember> _warMembers = new List<WarMember>();

    public War()
    {

    }

    public List<WarMember> GetMembers()
    {
        return new List<WarMember>(_warMembers);
    }

    public void AddToWar(Country country, WarMemberType warMemberType)
    {
        _warMembers.Add(new WarMember(country, warMemberType));
    }

    public bool CountryIsWarMember(Country country)
    {
        return GetCountryIsWarMember(country) != null;
    }

    public WarMember GetCountryIsWarMember(Country country)
    {
        var member = _warMembers.Find(member => member.Country == country);
        return member;
    }

    public bool NeedEnd()
    {
        var allAggressorCapitulated = _warMembers.FindAll(member => member.MemberType == WarMemberType.Aggressor).Count 
            == _warMembers.FindAll(member => (member.MemberType == WarMemberType.Aggressor && member.Country.IsÑapitulated == true)).Count;

        var allDefendersCapitulated = _warMembers.FindAll(member => member.MemberType == WarMemberType.Defender).Count
            == _warMembers.FindAll(member => (member.MemberType == WarMemberType.Defender && member.Country.IsÑapitulated == true)).Count;
        return allAggressorCapitulated || allDefendersCapitulated;
    }

    public void EndWar()
    {
        _warMembers.Clear();
        OnEnd?.Invoke();
    }

    public string GetWarName() 
    {
        return $"Âîéíà {_warMembers.Find(agr => agr.MemberType == WarMemberType.Aggressor).Country.Name}-{_warMembers.Find(agr => agr.MemberType == WarMemberType.Defender).Country.Name}";
    }
}

public class WarMember
{
    public Country Country { get; }
    public WarMemberType MemberType { get; }
    public int ManPowerLosses { get; private set; }

    public WarMember(Country country, WarMemberType memberType)
    {
        Country = country;
        MemberType = memberType;
        country.OnManpowerLosses += (int lossesCount) =>
        {
            ManPowerLosses += lossesCount;
        };
    }
}

public enum WarMemberType
{
    Aggressor,
    Defender
}

