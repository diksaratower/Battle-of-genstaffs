using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipHandlerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected TooltipViewMenu _tooltipViewMenuPrefab;

    protected TooltipViewMenu _tooltipViewMenu;
    protected TooltipsParent _cashedTooltipsParent;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipViewMenu != null)
        {
            return;
        }
        _tooltipViewMenu = Instantiate(_tooltipViewMenuPrefab, GetTooltipsParent().transform);
        _tooltipViewMenu.RefreshUI(this);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        Destroy(_tooltipViewMenu.gameObject);
    }

    protected void OnDestroy()
    {
        if (_tooltipViewMenu == null)
        {
            return;
        }
        Destroy(_tooltipViewMenu.gameObject);
    }

    protected void OnDisable()
    {
        if (_tooltipViewMenu == null)
        {
            return;
        }
        Destroy(_tooltipViewMenu.gameObject);
    }

    public TooltipsParent GetTooltipsParent()
    {
        if(_cashedTooltipsParent == null)
        {
            _cashedTooltipsParent = FindAnyObjectByType<TooltipsParent>();
        }
        return _cashedTooltipsParent;
    }
}
