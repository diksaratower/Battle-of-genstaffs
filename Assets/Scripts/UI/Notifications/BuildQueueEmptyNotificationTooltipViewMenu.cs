

public class BuildQueueEmptyNotificationTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not NotificationTooltipHandlerUI)
        {
            throw new System.ArgumentException();
        }
        AddSimpleText("��� �������� ������", false);
        base.RefreshUI(tooltipHandler);
    }
}
