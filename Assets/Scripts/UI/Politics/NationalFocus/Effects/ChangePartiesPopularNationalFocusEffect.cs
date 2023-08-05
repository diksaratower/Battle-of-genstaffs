using UnityEngine;


[CreateAssetMenu(fileName = "ChangePartiesPopular", menuName = "ScriptableObjects/Focuses/Effect/ChangePartiesPopular", order = 1)]
public class ChangePartiesPopularNationalFocusEffect : NationalFocusEffect
{
    public float ChangeProcent = 10;
    public Ideology PartyIdealogy;

    public override void ExecuteFocus(Country country)
    {
        country.Politics.AddPartyPopular(PartyIdealogy, ChangeProcent);
    }

    public override string GetEffectDescription()
    {
        return $"Популярность идеалогии {PartyIdealogy} {GameIU.FloatToStringAddPlus(ChangeProcent)}%";
    }
}
