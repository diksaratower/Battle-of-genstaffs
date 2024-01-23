using UnityEngine;


[CreateAssetMenu(fileName = "AddCountryTraitInstantEffect", menuName = "ScriptableObjects/Effects/Instant/AddCountryTraitInstantEffect", order = 1)]
public class AddCountryTraitInstantEffect : InstantEffect
{
    public CountryTrait CountryTrait;


    public override void DoEffect(Country country)
    {
        country.Politics.AddTrait(CountryTrait);
    }

    public override string GetEffectDescription()
    {
        return $"Добавит черту гос-ва {CountryTrait.Name}";
    }
}
