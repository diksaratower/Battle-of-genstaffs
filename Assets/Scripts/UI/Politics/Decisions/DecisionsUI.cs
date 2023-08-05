using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionsUI : MonoBehaviour
{
    [SerializeField] private DecisionSlotUI _slotPrefab;
    [SerializeField] private GridLayoutGroup _slotsParent;

    private List<DecisionSlotUI> _slotsUI = new List<DecisionSlotUI>();
    private Country _country => Player.CurrentCountry;

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        _slotsUI.ForEach(sl => Destroy(sl.gameObject));
        _slotsUI.Clear();
        foreach (var decision in _country.Politics.Preset.Decisions)
        {
            var slot = Instantiate(_slotPrefab, _slotsParent.transform);
            slot.Target = decision;
        }
    }
}
