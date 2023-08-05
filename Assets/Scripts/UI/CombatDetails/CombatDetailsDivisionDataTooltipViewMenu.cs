using System;


public class CombatDetailsDivisionDataTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        var division = (tooltipHandler as CombatDetailsDivisionDataTooltipHandlerUI).Target;
        var combat = (tooltipHandler as CombatDetailsDivisionDataTooltipHandlerUI).TargetCombat;
        AddSimpleText("Дивизия в бою", false);
        AddDynamicText(() => ("Дивизия не эффективна из-за оснащённости " + Math.Round(division.GetEquipmentProcent(eqType => eqType != EquipmentType.Manpower) * 100f, 1) + "%"), false);
        AddDynamicText(() => ("Дивизия не эффективна из-за не хватки людей " + Math.Round(division.GetEquipmentProcent(eqType => eqType == EquipmentType.Manpower) * 100f, 1) + "%"), false);
        foreach (var comabatEffect in combat.GetDivisionEffectDescription(division))
        {
            AddSimpleText(comabatEffect, false);
        }
        base.RefreshUI(tooltipHandler);
    }
}
