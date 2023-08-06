using UnityEngine;


public class WarGoalJustificationTimeTooltipView : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        var data = (tooltipHandler as NotPrefabTooltipHandlerUI).HandlerData as JustifyWarGoalTooltipHandlerData;
        AddDynamicText(() => $"���������� ���� ����� {data.JustificationQueueSlot.JustificationProgress}/{data.JustificationQueueSlot.JustificationTimeDays} ����", false);
        base.RefreshUI(tooltipHandler);
    }
}
