using System;
using TMPro;
using UnityEngine;


public class BlockedDecisionSlotUI : MonoBehaviour, IDecisionsUIViewSlot
{
    [SerializeField] private TextMeshProUGUI _decisionName;
    [SerializeField] private TextMeshProUGUI _blockedTimeLeft;

    private DecisionsBlockSlot _targetBlockedDecisionSlot;


    private void Update()
    {
        if (_targetBlockedDecisionSlot != null)
        {
            _blockedTimeLeft.text = $"До разблокировки осталось {_targetBlockedDecisionSlot.RechargeTimeLeftDays} дней.";
        }
    }

    public void RefreshUI(DecisionsBlockSlot blockedDecisionSlot, PolticsUI politicsUI)
    {
        _targetBlockedDecisionSlot = blockedDecisionSlot;
        _decisionName.text = blockedDecisionSlot.Decision.Name;
    }

    GameObject IDecisionsUIViewSlot.GetSlotGO()
    {
        return gameObject;
    }
}
