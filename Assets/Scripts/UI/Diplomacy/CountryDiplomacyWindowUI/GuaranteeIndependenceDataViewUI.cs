using UnityEngine;
using UnityEngine.UI;


public class GuaranteeIndependenceDataViewUI : MonoBehaviour
{
    [SerializeField] private Image _countryFlag;


    public void RefreshUI(Country guaranter, Country target)
    {
        _countryFlag.sprite = guaranter.Flag;
        var tooltip = gameObject.AddComponent<NotPrefabTooltipHandlerUI>();

        tooltip.Initialize((TooltipViewMenu menu) => 
        {
            menu.AddSimpleText($"{guaranter.Name} является гарантом независимости государства {target.Name}", false);
        });
    }
}