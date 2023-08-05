using System;


public class CombatDetailsDivisionDataTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        var division = (tooltipHandler as CombatDetailsDivisionDataTooltipHandlerUI).Target;
        var combat = (tooltipHandler as CombatDetailsDivisionDataTooltipHandlerUI).TargetCombat;
        AddSimpleText("������� � ���", false);
        AddDynamicText(() => ("������� �� ���������� ��-�� ������������ " + Math.Round(division.GetEquipmentProcent(eqType => eqType != EquipmentType.Manpower) * 100f, 1) + "%"), false);
        AddDynamicText(() => ("������� �� ���������� ��-�� �� ������ ����� " + Math.Round(division.GetEquipmentProcent(eqType => eqType == EquipmentType.Manpower) * 100f, 1) + "%"), false);
        foreach (var comabatEffect in combat.GetDivisionEffectDescription(division))
        {
            AddSimpleText(comabatEffect, false);
        }
        base.RefreshUI(tooltipHandler);
    }
}
