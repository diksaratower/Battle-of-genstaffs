using UnityEngine;


[CreateAssetMenu(fileName = "ChangeLeader", menuName = "ScriptableObjects/Decision/Effects/ChangeLeader", order = 1)]
public class ChangeLeaderDecisionEffect : InstantEffect
{
    public Personage NewLeader;

    public override void DoEffect(Country country)
    {
        country.Politics.ChangeLeader(NewLeader);
    }

    public override string GetEffectDescription()
    {
        return "";
    }
}
