using UnityEngine;


[CreateAssetMenu(fileName = "ChangeIdealogy", menuName = "ScriptableObjects/Decision/Effects/ChangeIdealogy", order = 1)]
public class ChangeIdealogyDecisionEffect : InstantEffect
{
    public Ideology NewIdeology;

    public override void DoEffect(Country country)
    {
    }

    public override string GetEffectDescription()
    {
        return "";
    }
}
