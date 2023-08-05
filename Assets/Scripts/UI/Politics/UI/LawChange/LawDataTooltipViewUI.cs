

public class LawDataTooltipViewUI : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not LawDataTooltipHandlerUI)
        {
            throw new System.ArgumentException();
        }
        var law = (tooltipHandler as LawDataTooltipHandlerUI).TargetLaw;
        AddSimpleText(law.Name, false);
        foreach (var effect in law.LawEffects)
        {
            AddSimpleText(effect.GetEffectDescription(), false);
        }
        base.RefreshUI(tooltipHandler);
    }
}
