using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CreationDivisionsQueueSlotUI : MonoBehaviour
{
    [SerializeField] private Image _divisionImage;
    [SerializeField] private TextMeshProUGUI _divisionName;
    [SerializeField] private Image _equipmentProcentFill;
    [SerializeField] private Image _trainingProgressFill;
    [SerializeField] private Button _deleteButton;

    private CreationDivisionsQueueSlot _targetSlot;

    private void Update()
    {
        if (_targetSlot == null)
        {
            return;
        }
        _trainingProgressFill.fillAmount = (float)_targetSlot.CreationProgressDays / (float)_targetSlot.CretionTimeDays;
    }

    public void RefreshUI(CreationDivisionsQueueSlot queueSlot, CountryCreationDivisions countryCreationDivisions)
    {
        _targetSlot = queueSlot;
        _divisionName.text = queueSlot.DivisionName;
        _divisionImage.sprite = queueSlot.DivisionTemplate.GetAverageBattlion().BatImage;

        var tooltip = gameObject.AddComponent<NotPrefabTooltipHandlerUI>();

        tooltip.Initialize((TooltipViewMenu menu) => 
        {
            menu.AddSimpleText($"Дивизия {queueSlot.DivisionName} обучается.", false);
            menu.AddDynamicText(() => $"До конца обучения {_targetSlot.CretionTimeDays - _targetSlot.CreationProgressDays} дней.", false);
        });

        _deleteButton.onClick.AddListener(delegate 
        {
            countryCreationDivisions.RemoveDivisionCreation(queueSlot);
        });
    }
}
