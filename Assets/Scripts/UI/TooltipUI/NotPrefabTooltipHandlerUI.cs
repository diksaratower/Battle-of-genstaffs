using UnityEngine.EventSystems;
using UnityEngine;
using System;


public class NotPrefabTooltipHandlerUI : TooltipHandlerUI
{
    public NotPrefabTooltipHandlerData HandlerData = null;

    private Type _viewMenuType;
    private static TooltipsDataSO _dataCashed;
    private Action<TooltipViewMenu> _onRefreshUI;

    public void Initialize<T>() where T : TooltipViewMenu
    {
        GetTooltipsData();
        _viewMenuType = typeof(T);
    }

    public void Initialize(Action<TooltipViewMenu> onRefreshUI)
    {
        GetTooltipsData();
        _viewMenuType = typeof(TooltipViewMenu);
        _onRefreshUI = onRefreshUI;
    }

    public void Initialize<T>(NotPrefabTooltipHandlerData handlerData) where T : TooltipViewMenu
    {
        GetTooltipsData();
        _viewMenuType = typeof(T);
        HandlerData = handlerData;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipViewMenu != null)
        {
            return;
        }
        var menu = Instantiate(GetTooltipsData().MenuStandartPrefab, GetTooltipsParent().transform);
        menu.AddComponent(_viewMenuType);
        _tooltipViewMenu = menu.GetComponent<TooltipViewMenu>();
        _tooltipViewMenu.OnRefreshUI += (TooltipViewMenu tooltipMenu) => _onRefreshUI?.Invoke(tooltipMenu);
        _tooltipViewMenu.RefreshUI(this);
    }

    public static TooltipsDataSO GetTooltipsData()
    {
        if (_dataCashed == null)
        {
            _dataCashed = Resources.Load<TooltipsDataSO>("TooltipData");
        }
        return _dataCashed;
    }
}

public abstract class NotPrefabTooltipHandlerData
{
}