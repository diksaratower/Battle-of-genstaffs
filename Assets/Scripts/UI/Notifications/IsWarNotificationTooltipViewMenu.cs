

public class IsWarNotificationTooltipViewMenu : TooltipViewMenu
{
    public override void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        if (tooltipHandler is not NotificationTooltipHandlerUI)
        {
            throw new System.ArgumentException();
        }
        var country = (tooltipHandler as NotificationTooltipHandlerUI).NotificationsUI.TargetCountry;
        AddSimpleText("ּû גמ‏ול!", false);
        var wars = Diplomacy.Instance.GetCountryWars(country);
        foreach (var war in wars) 
        {
            AddSimpleText(war.GetWarName(), false);
        }
        base.RefreshUI(tooltipHandler);
    }
}
