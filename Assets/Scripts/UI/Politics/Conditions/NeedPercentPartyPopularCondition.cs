using UnityEngine;


[CreateAssetMenu(fileName = "NeedPercentPartyPopularCondition", menuName = "ScriptableObjects/PoliticsConditions/NeedPercentPartyPopularCondition", order = 1)]
public class NeedPercentPartyPopularCondition : PoliticsCondition
{
    public float NeedPercentPopularity = 10f;
    public PoliticalParty Party;

    public override bool CountryIsFits(Country country)
    {
        if (country.Politics.GetPercentPopularity(Party) > NeedPercentPopularity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string GetConditionDescription()
    {
        return $"Необходимо {NeedPercentPopularity}% популярности партии {Party.Name}.";
    }
}
