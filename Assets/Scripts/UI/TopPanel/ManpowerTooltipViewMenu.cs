using System;
using UnityEngine;


public class ManpowerTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        AddSimpleText($"������: {ToPettyNumberView(Player.CurrentCountry.EquipmentStorage.GetEquipmentCount(EquipmentType.Manpower))}", false);
        
        var conscriptionPercent = Player.CurrentCountry.Politics.GetConscriptionPercent();
        AddSimpleText($"{conscriptionPercent}% �� ������ ��������� ������: {ToPettyNumberView(Player.CurrentCountry.CountryPreset.Population)}", false);
        AddSimpleText($"� ���� ������� ������� ���� �����...", false);
        base.RefreshUI(tooltipHandler);
    }

    private string ToPettyNumberView(float value)
    {
        var result = Math.Round(value, 2).ToString();
        if (value > 1000f)
        {
            result = Math.Round(value / 1000f).ToString() + " ���.";
        }
        if (value > 1000000f)
        {
            result = Math.Round(value / 1000000f).ToString() + " ���.";
        }
        return result;
    }

    private float ToMillions(float value)
    {
        return (float)System.Math.Round((float)(value / 1000000), 2);
    }
}
