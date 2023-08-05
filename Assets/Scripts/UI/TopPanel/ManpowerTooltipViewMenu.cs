using UnityEngine;


public class ManpowerTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        AddSimpleText($"Резерв: {ToMillions(((float)Player.CurrentCountry.CountryPreset.Population / 100f) * 5f)}млн", false);
        
        AddSimpleText($"{5f}% от общего населения страны: {ToMillions(Player.CurrentCountry.CountryPreset.Population)}млн", false);
        AddSimpleText($"А ведь каждого солдата есть семья...", false);
        base.RefreshUI(tooltipHandler);
    }

    private float ToMillions(float value)
    {
        return (float)System.Math.Round((float)(value / 1000000), 2);
    }
}
