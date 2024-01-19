using System.Collections.Generic;


public class StabilityTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        AddSimpleText($"Устойчивость режима: {Player.CurrentCountry.Politics.CalculateStability()} %", false);
        foreach (var effect in GetStabilityEffectsDescriptionsForUI())
        {
            AddSimpleText(effect, false);
        }
        AddSimpleText($"Базовое значение: {Player.CurrentCountry.Politics.BaseStability}", false);
        AddSimpleText("Устойчивость режима показывает насколько государство способно сохранить контроль за страной.", false);
        base.RefreshUI(tooltipHandler);
    }

    public List<string> GetStabilityEffectsDescriptionsForUI()
    {
        var result = new List<string>();
        foreach (var adviser in Player.CurrentCountry.Politics.Advisers)
        {
            var effects = adviser.GetEffects<ChangeStabilityTraitEffect>();
            foreach (var effect in effects)
            {
                result.Add($"{adviser.Name} даёт {GameIU.FloatToStringAddPlus(effect.ChangeStabilityProcent)}%");
            }
        }
        foreach (var trait in Player.CurrentCountry.Politics.Traits)
        {
            var traitsEffects = trait.GetEffects<ChangeStabilityTraitEffect>();
            foreach (var effect in traitsEffects)
            {
                result.Add($"Черта {trait.Name} даёт {GameIU.FloatToStringAddPlus(effect.ChangeStabilityProcent)}%");
            }
        }
        return result;
    }
}
