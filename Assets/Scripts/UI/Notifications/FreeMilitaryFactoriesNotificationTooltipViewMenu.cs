

public class FreeMilitaryFactoriesNotificationTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not NotificationTooltipHandlerUI)
        {
            throw new System.ArgumentException();
        }
        AddSimpleText("Есть не занятые военные заводы", false);
        base.RefreshUI(tooltipHandler);
    }
}
