using UnityEngine;
using UnityEngine.UI;


public class CountryJustifyWarGoalDataUI : MonoBehaviour
{
    [SerializeField] private Image _countryFlag;


    public void RefreshUI(Country country, WarGoalJustificationQueueSlot justificationQueueSlot)
    {
        _countryFlag.sprite = country.Flag;

        var tooltip = gameObject.AddComponent<NotPrefabTooltipHandlerUI>();

        tooltip.Initialize((TooltipViewMenu menu) =>
        {
            menu.AddDynamicText(() => $"Оправдание цели войны {justificationQueueSlot.JustificationProgress}/{justificationQueueSlot.JustificationTimeDays} дней", false);
        });
    }
}