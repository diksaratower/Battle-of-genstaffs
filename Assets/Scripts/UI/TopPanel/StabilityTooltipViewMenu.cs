using System.Collections.Generic;


public class StabilityTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        AddSimpleText($"������������ ������: {Player.CurrentCountry.Politics.CalculateStability()} %", false);
        foreach (var effect in GetStabilityEffectsDescriptionsForUI())
        {
            AddSimpleText(effect, false);
        }
        AddSimpleText($"������� ��������: {Player.CurrentCountry.Politics.BaseStability}", false);
        AddSimpleText("������������ ������ ���������� ��������� ����������� �������� ��������� �������� �� �������.", false);
        base.RefreshUI(tooltipHandler);
    }

    public List<string> GetStabilityEffectsDescriptionsForUI()
    {
        var result = new List<string>();
        foreach (var adviser in Player.CurrentCountry.Politics.Advisers)
        {
            var effects = adviser.GetTraitEffects<ChangeStabilityTraitEffect>();
            foreach (var effect in effects)
            {
                result.Add($"{adviser.Name} ��� {GameIU.FloatToStringAddPlus((effect as ChangeStabilityTraitEffect).ChangeStabilityProcent)}%");
            }
        }
        return result;
    }
}
