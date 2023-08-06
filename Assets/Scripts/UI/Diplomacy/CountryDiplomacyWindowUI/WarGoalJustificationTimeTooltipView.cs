using UnityEngine;


public class WarGoalJustificationTimeTooltipView : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        var data = (tooltipHandler as NotPrefabTooltipHandlerUI).HandlerData as JustifyWarGoalTooltipHandlerData;
        AddDynamicText(() => $"Оправдание цели войны {data.JustificationQueueSlot.JustificationProgress}/{data.JustificationQueueSlot.JustificationTimeDays} дней", false);
        base.RefreshUI(tooltipHandler);
    }
}
