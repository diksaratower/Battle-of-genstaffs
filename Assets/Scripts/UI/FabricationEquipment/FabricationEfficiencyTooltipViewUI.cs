using System.Collections.Generic;


public class FabricationEfficiencyTooltipViewUI : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not FabricationEfficiencyTooltipHandlerUI)
        {
            throw new System.ArgumentException();
        }
        var fabrication = (tooltipHandler as FabricationEfficiencyTooltipHandlerUI).FabricationEquipmentUI;
        AddSimpleText($"Ёффективность производства {fabrication.TargetCountry.CountryFabrication.GetCorrectFabricationEfficiency() * 100}%", false);
        foreach (var effect in GetFabricatioyEffectsDescriptionsForUI(fabrication.TargetCountry))
        {
            AddSimpleText(effect, false);
        }
        base.RefreshUI(tooltipHandler);
    }

    public List<string> GetFabricatioyEffectsDescriptionsForUI(Country country)
    {
        var result = new List<string>();
        foreach (var effect in country.Politics.CurrentEconomicLaw.LawEffects)
        {
            if (effect is MilitaryFabricationLawEffect)
            {
                result.Add(country.Politics.CurrentEconomicLaw.Name + " даЄт " + effect.GetEffectDescription());
            }
        }
        return result;
    }
}
