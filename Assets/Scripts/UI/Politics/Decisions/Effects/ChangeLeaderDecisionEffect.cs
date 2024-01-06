using UnityEngine;


[CreateAssetMenu(fileName = "ChangeLeader", menuName = "ScriptableObjects/Decision/Effects/ChangeLeader", order = 1)]
public class ChangeLeaderDecisionEffect : DecisionEffect
{
    public Personage NewLeader;

    public override void ExecuteDecisionEffect(Country country)
    {
        country.Politics.CountryLeader = NewLeader;
    }
}
