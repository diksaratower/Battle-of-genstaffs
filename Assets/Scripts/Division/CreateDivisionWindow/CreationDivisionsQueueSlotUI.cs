using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CreationDivisionsQueueSlotUI : MonoBehaviour
{
    [SerializeField] private Image _divisionImage;
    [SerializeField] private TextMeshProUGUI _divisionName;
    [SerializeField] private Image _equipmentProcentFill;
    [SerializeField] private Image _trainingProgressFill;

    private CreationDivisionsQueueSlot _targetSlot;

    private void Update()
    {
        if (_targetSlot == null)
        {
            return;
        }
        _trainingProgressFill.fillAmount = (float)_targetSlot.CreationProgressDays / (float)_targetSlot.CretionTimeDays;
    }

    public void RefreshUI(CreationDivisionsQueueSlot queueSlot)
    {
        _targetSlot = queueSlot;
        _divisionName.text = queueSlot.DivisionName;
    }
}
