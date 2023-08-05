using System;


public class BuildingQueueSlotTooltipViewUI : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not BuildingQueueSlotTooltipHandlerUI)
        {
            throw new ArgumentException();
        }
        var slot = (tooltipHandler as BuildingQueueSlotTooltipHandlerUI).QueueSlotUI;
        AddSimpleText(slot.BuildSlot.Building.Name, false);
        AddSimpleText("��������� ��������� " + slot.BuildSlot.Building.BuildCost, false);
        AddDynamicText(() => $"����������: {GetBuildRoundProcent(slot)}%", false);
        base.RefreshUI(tooltipHandler);
    }

    private float GetBuildRoundProcent(BuildingQueueSlotUI slot)
    {
        var buildProcent = (slot.BuildSlot.BuildProgress / slot.BuildSlot.Building.BuildCost) * 100;
        return (float)Math.Round(buildProcent, 2);
    }
}
