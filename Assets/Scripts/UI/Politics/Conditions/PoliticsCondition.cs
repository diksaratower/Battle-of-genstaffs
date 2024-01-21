using UnityEngine;


public abstract class PoliticsCondition : ScriptableObject
{
    public abstract bool CountryIsFits(Country country);

    public abstract string GetConditionDescription();
}
