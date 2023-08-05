

public class AdviserDataTooltipViewUI : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if(tooltipHandler is not AdviserDataTooltipHandlerUI) 
        {
            throw new System.ArgumentException();
        }
        var personge = (tooltipHandler as AdviserDataTooltipHandlerUI).TargetAdviser;
        AddSimpleText(personge.Name, false);
        var advisersTraits = personge.Traits.FindAll(trait => trait is AdviserTrait);
        foreach (var trait in advisersTraits)
        {
            AddSimpleText(trait.TraitName + ": ", false);
            foreach (var effect in trait.TraitEffects)
            {
                AddSimpleText(effect.GetEffectDescription(), false);
            }
        }
        base.RefreshUI(tooltipHandler);
    }
}
