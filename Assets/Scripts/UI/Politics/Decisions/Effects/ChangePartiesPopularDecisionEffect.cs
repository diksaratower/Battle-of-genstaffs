using UnityEngine;


[CreateAssetMenu(fileName = "ChangePartiesPopular", menuName = "ScriptableObjects/Decision/Effects/ChangePartiesPopular", order = 1)]
public class ChangePartiesPopularDecisionEffect : DecisionEffect
{
    [SerializeField] private float _addedProcent = 5;
    [SerializeField] private string _partyName;

    public override void ExecuteDecisionEffect(Country country)
    {
        country.Politics.AddPartyPopular(_partyName, _addedProcent);
    }
}
