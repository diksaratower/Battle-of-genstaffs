using UnityEngine;


[CreateAssetMenu(fileName = "ChangePartyPopularAdviserEffect", menuName = "ScriptableObjects/Personages/Traits/Effects/ChangePartyPopular", order = 1)]
public class ChangePartyPopularTraitEffect : TraitEffect
{
    public float AddedPartyPopularPerDay;
    public Ideology PartyIdeology;

    public override string GetEffectDescription()
    {
        return $"Прирост популярности партии {PartyIdeology} в день на {GameIU.FloatToStringAddPlus(AddedPartyPopularPerDay)}%.";
    }
}
