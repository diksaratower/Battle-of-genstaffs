using UnityEngine;


[CreateAssetMenu(fileName = "ChangePartiesPopular", menuName = "ScriptableObjects/Decision/Effects/ChangePartiesPopular", order = 1)]
public class ChangePartiesPopularDecisionEffect : InstantEffect
{
    [SerializeField] private float _addedProcent = 5;
    [SerializeField] private string _partyName;

    public override void DoEffect(Country country)
    {
        country.Politics.AddPartyPopular(_partyName, _addedProcent);
    }

    public override string GetEffectDescription()
    {
        return "";
    }
}
