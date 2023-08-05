

public class BuildQueueEmptyNotificationTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not NotificationTooltipHandlerUI)
        {
            throw new System.ArgumentException();
        }
        AddSimpleText("Нет активных строек", false);
        base.RefreshUI(tooltipHandler);
    }
}
