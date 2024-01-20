using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DecisionsUI : MonoBehaviour
{
    [SerializeField] private DecisionSlotUI _slotPrefab;
    [SerializeField] private BlockedDecisionSlotUI _blockedSlotPrefab;
    [SerializeField] private GridLayoutGroup _slotsParent;

    private List<IDecisionsUIViewSlot> _slotsUI = new List<IDecisionsUIViewSlot>();
    private Country _country => Player.CurrentCountry;


    public void RefreshUI(PolticsUI politicsUI)
    {
        _slotsUI.ForEach(sl => Destroy(sl.GetSlotGO()));
        _slotsUI.Clear();
        foreach (var blockedDecision in _country.Politics.BlockedDecisions)
        {
            if (blockedDecision.EternalBlock == true)
            {
                continue;
            }
            var slot = Instantiate(_blockedSlotPrefab, _slotsParent.transform);
            slot.RefreshUI(blockedDecision, politicsUI);
            _slotsUI.Add(slot);
        }
        foreach (var decision in _country.Politics.Decisions)
        {
            if (_country.Politics.BlockedDecisions.Exists(blockedDecisions => blockedDecisions.Decision == decision))
            {
                continue;
            }
            var slot = Instantiate(_slotPrefab, _slotsParent.transform);
            slot.RefreshUI(decision, politicsUI);
            _slotsUI.Add(slot);
        }
    }
}

public interface IDecisionsUIViewSlot
{
    public GameObject GetSlotGO();
}