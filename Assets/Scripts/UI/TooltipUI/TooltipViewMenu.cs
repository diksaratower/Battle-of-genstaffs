using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class TooltipViewMenu : MonoBehaviour
{
    private TooltipsDataSO _tooltipsData => NotPrefabTooltipHandlerUI.GetTooltipsData();
    [SerializeField] private VerticalLayoutGroup _verticalGroup;

    private List<DynamicText> _dynamicTexts = new List<DynamicText>(); 
    private readonly Vector3 _offest = new Vector3(25, -15);

    public virtual void RefreshUI(TooltipHandlerUI tooltipHandler)
    {
        UpdatePositionFollowMouse();
    }

    protected virtual void Update()
    {
        _dynamicTexts.ForEach(text => 
        {
            text.UpdateText();
        });
        UpdatePositionFollowMouse();
    }

    protected void AddDynamicText(Func<string> getTextFunc, bool prefferdSizeHorizontal = true)
    {
        var text = new DynamicText(InstantiateText(getTextFunc(), prefferdSizeHorizontal), getTextFunc);
        text.UpdateText();
        _dynamicTexts.Add(text);
    }

    protected void AddSimpleText(string text, bool prefferdSizeHorizontal = true)
    {
        InstantiateText(text, prefferdSizeHorizontal);
    }

    private TextMeshProUGUI InstantiateText(string text, bool prefferdSizeHorizontal = true)
    {
        if (_verticalGroup == null)
        {
            _verticalGroup = GetComponent<VerticalLayoutGroup>();
        }
        var textObject = Instantiate(_tooltipsData.SimpleTextPrefab, _verticalGroup.transform);
        textObject.text = text;
        if (prefferdSizeHorizontal == false)
        {
            textObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        return textObject;
    }

    protected void UpdatePositionFollowMouse()
    {
        var rectTransform = (transform as RectTransform);
        var mousePos = GameCamera.Instance.CorrectScreenPointResolutionTrue(Input.mousePosition);
        rectTransform.anchoredPosition = mousePos + new Vector3(rectTransform.rect.size.x / 2, -(rectTransform.rect.size.y / 2)) + _offest;
    }

    private class DynamicText
    {
        private TextMeshProUGUI _text;
        private Func<string> _getNewTextFunc;

        public DynamicText(TextMeshProUGUI text, Func<string> getTextFunc)
        {
            _text = text;
            _getNewTextFunc = getTextFunc;
        }

        public void UpdateText()
        {
            _text.text = _getNewTextFunc();
        }
    }
}
