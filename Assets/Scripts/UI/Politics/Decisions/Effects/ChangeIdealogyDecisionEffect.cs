using UnityEngine;


[CreateAssetMenu(fileName = "ChangeIdealogy", menuName = "ScriptableObjects/Decision/Effects/ChangeIdealogy", order = 1)]
public class ChangeIdealogyDecisionEffect : DecisionEffect
{
    public Ideology NewIdeology;

    public override void ExecuteDecisionEffect(Country country)
    {
        throw new System.NotImplementedException();
    }
}
