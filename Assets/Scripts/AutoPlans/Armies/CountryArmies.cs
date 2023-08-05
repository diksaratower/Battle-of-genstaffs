using System;
using System.Collections.Generic;


public class CountryArmies
{
    public List<Army> Armies => GetArmies();
    public Action OnArmiesChanged;

    private List<Army> _armies = new List<Army>();

    public CountryArmies()
    {
        GameTimer.HourEnd += DoPlans;
    }

    private void DoPlans()
    {
        foreach (var army in _armies)
        {
            army.DoPlans();
        }
    }

    public Army AddArmy(List<Division> divisions)
    {
        foreach (var division in divisions)
        {
            if (IsDivisionsAttachedWithArmy(division))
            {
                throw new TryAddDivisionsToTwoArmiesException();
            }    
        }
        var army = new Army(this);
        army.AddDivisions(divisions);
        _armies.Add(army);
        OnArmiesChanged?.Invoke();
        return army;
    }

    public void RemoveAllArmies()
    {
        _armies.Clear();
        OnArmiesChanged?.Invoke();
    }

    public void RemoveArmy(Army army)
    {
        _armies.Remove(army);
        OnArmiesChanged?.Invoke();
    }

    private List<Army> GetArmies()
    {
        return new List<Army>(_armies);
    }

    public bool IsDivisionsAttachedWithArmy(Division division)
    {
        return _armies.Exists(army => army.Divisions.Contains(division));
    }

    public CountryArmiesSerialize GetSerialize()
    {
        return new CountryArmiesSerialize(this);
    }

    public void LoadFromSerialize(CountryArmiesSerialize countryArmies)
    {
        foreach (var armySer in countryArmies.Armies)
        {
            //Armies.Add(new Army());
        }
    }

    [Serializable]
    public class CountryArmiesSerialize
    {
        public List<ArmySerialize> Armies = new List<ArmySerialize>();

        public CountryArmiesSerialize(CountryArmies cArmies)
        {
            foreach (var arm in cArmies._armies) 
            {
                Armies.Add(new ArmySerialize() { });
            }
        }
    }

    [Serializable]
    public class ArmySerialize
    {
        public string Name = "null";
    }
}

public class TryAddDivisionsToTwoArmiesException : Exception { }