using System.Collections.Generic;


public class PolitPowerTooltipUI : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        AddSimpleText("������� ����� ������ (� ����): " + 
            Player.CurrentCountry.Politics.ApplyPolitPowerGrowthEffects(Player.CurrentCountry.Politics.PolitPowerGrowthSpeed), false);
        foreach (var effect in GetPolitPowerGrowthEffectsDescriptionsForUI())
        {
            AddSimpleText(effect, false);
        }
        AddSimpleText($"������� �������� {Player.CurrentCountry.Politics.PolitPowerGrowthSpeed}", false);
        AddSimpleText($"��������� {Player.CurrentDifficultie.Name} {GameIU.FloatToStringAddPlus(Player.CurrentDifficultie.PolitPowerBonusPercent)}%", false);
        AddSimpleText("�����. ������ ����� ��� �������� ������� ������� ��������� � ��.", false);
        base.RefreshUI(tooltipHandler);
    }

    public List<string> GetPolitPowerGrowthEffectsDescriptionsForUI()
    {
        var result = new List<string>();
        foreach (var adviser in Player.CurrentCountry.Politics.Advisers)
        {
            var effects = adviser.GetTraitEffects<PolitPowerGrowthTraitEffect>();
            foreach (var effect in effects)
            {
                result.Add($"{adviser.Name} ��� {GameIU.FloatToStringAddPlus((effect as PolitPowerGrowthTraitEffect).PolitPowerGrothIncreaseProcent)}%");
            }
        }
        return result;
    }
}
