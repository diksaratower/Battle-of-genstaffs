using UnityEngine;
using UnityEngine.UI;


public class CountryTraitUI : MonoBehaviour
{
    [SerializeField] private Image _traitImage;


    public void RefreshUI(CountryTrait trait)
    {
        _traitImage.sprite = trait.TraitImage;
        var tooltip = gameObject.AddComponent<NotPrefabTooltipHandlerUI>();

        tooltip.Initialize((TooltipViewMenu menu) =>
        {
            menu.AddSimpleText(trait.Name, false);
            foreach (var effect in trait.CountryTraitEffects)
            {
                menu.AddSimpleText(effect.GetEffectDescription());
            }
        });
    }
}
