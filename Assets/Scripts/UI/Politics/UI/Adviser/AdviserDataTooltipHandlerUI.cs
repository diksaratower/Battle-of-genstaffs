using UnityEngine.EventSystems;


public class AdviserDataTooltipHandlerUI : TooltipHandlerUI
{
    public Personage TargetAdviser { get; private set; }

    public void SetAdviser(Personage personage)
    {
        TargetAdviser = personage;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(TargetAdviser == null)
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
