using UnityEngine;


[CreateAssetMenu(fileName = "ChangePartiesPopular", menuName = "ScriptableObjects/Focuses/Effect/ChangePartiesPopular", order = 1)]
public class ChangePartiesPopularNationalFocusEffect : InstantEffect
{
    public float ChangeProcent = 10;
    public Ideology PartyIdealogy;

    public override void DoEffect(Country country)
    {
        country.Politics.AddPartyPopular(PartyIdealogy, ChangeProcent);
    }

    public override string GetEffectDescription()
    {
        return $"Популярность идеалогии {UtilsPoliticsUI.IdeologyToString(PartyIdealogy)} {GameIU.FloatToStringAddPlus(ChangeProcent)}%";
    }
}
