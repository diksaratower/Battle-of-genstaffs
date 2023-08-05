using UnityEngine.EventSystems;
using UnityEngine;
using System;


public class NotPrefabTooltipHandlerUI : TooltipHandlerUI
{
    private Type _viewMenuType;
    private static TooltipsDataSO _dataCashed;

    public void Initialize<T>() where T : TooltipViewMenu
    {
        GetTooltipsData();
        _viewMenuType = typeof(T);
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
