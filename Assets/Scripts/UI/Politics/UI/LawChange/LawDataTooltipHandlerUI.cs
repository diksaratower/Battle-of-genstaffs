using UnityEngine.EventSystems;


public class LawDataTooltipHandlerUI : TooltipHandlerUI
{
    public Law TargetLaw { get; private set; }

    public void SetLaw(Law law)
    {
        TargetLaw = law;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (TargetLaw == null)
        {
            return;
        }
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
