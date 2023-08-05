

public class NotExecuteFocusNotificationTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not NotificationTooltipHandlerUI)
        {
            throw new System.ArgumentException();
        }
        AddSimpleText("Направление политики не выбрано", false);
        base.RefreshUI(tooltipHandler);
    }
}
