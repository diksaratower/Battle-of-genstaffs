using System;
using System.Collections.Generic;


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
            == _warMembers.FindAll(member => (member.MemberType == WarMemberType.Aggressor && member.Country.IsСapitulated == true)).Count;

        var allDefendersCapitulated = _warMembers.FindAll(member => member.MemberType == WarMemberType.Defender).Count
            == _warMembers.FindAll(member => (member.MemberType == WarMemberType.Defender && member.Country.IsСapitulated == true)).Count;
        return allAggressorCapitulated || allDefendersCapitulated;
    }

    public void EndWar()
    {
        _warMembers.Clear();
        OnEnd?.Invoke();
    }

    public string GetWarName() 
    {
        return $"Война {_warMembers.Find(agr => agr.MemberType == WarMemberType.Aggressor).Country.Name}-{_warMembers.Find(agr => agr.MemberType == WarMemberType.Defender).Country.Name}";
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
