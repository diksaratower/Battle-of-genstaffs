using UnityEngine;


public class ManpowerTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        AddSimpleText($"������: {ToMillions(((float)Player.CurrentCountry.CountryPreset.Population / 100f) * 5f)}���", false);
        
        AddSimpleText($"{5f}% �� ������ ��������� ������: {ToMillions(Player.CurrentCountry.CountryPreset.Population)}���", false);
        AddSimpleText($"� ���� ������� ������� ���� �����...", false);
        base.RefreshUI(tooltipHandler);
    }

    private float ToMillions(float value)
    {
        return (float)System.Math.Round((float)(value / 1000000), 2);
    }
}
