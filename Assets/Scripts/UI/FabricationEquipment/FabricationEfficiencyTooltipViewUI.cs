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
        AddSimpleText($"������������� ������������ {fabrication.TargetCountry.CountryFabrication.GetCorrectFabricationEfficiency() * 100}%", false);
        AddSimpleText($"������� �������� ������������� {fabrication.TargetCountry.CountryFabrication.GetBaseFabricationEfficiency() * 100}%");
        foreach (var effect in GetFabricatioyEffectsDescriptionsForUI(fabrication.TargetCountry))
        {
            AddSimpleText(effect, false);
        }
        base.RefreshUI(tooltipHandler);
    }

    public List<string> GetFabricatioyEffectsDescriptionsForUI(Country country)
    {
        var result = new List<string>();
        result.Add($"����� �� ��������� {Player.CurrentDifficultie.Name} {GameIU.FloatToStringAddPlus(Player.CurrentDifficultie.ProductionFactor * 100)}%");
        foreach (var effect in country.Politics.CurrentEconomicLaw.LawEffects)
        {
            if (effect is MilitaryFabricationLawEffect)
            {
                result.Add(country.Politics.CurrentEconomicLaw.Name + " ��� " + effect.GetEffectDescription());
            }
        }
        return result;
    }
}
