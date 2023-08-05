

public class CanResearchTechNotificationTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not NotificationTooltipHandlerUI)
        {
            throw new System.ArgumentException();
        }
        AddSimpleText("Очков опыта хватает на изучение технологий.", false);
        base.RefreshUI(tooltipHandler);
    }
}
