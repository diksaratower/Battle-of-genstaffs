using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatDetailsWindowUI : MonoBehaviour
{
    [SerializeField] private Transform _attackDivisionsSlotsParent;
    [SerializeField] private Transform _defensDivisionsSlotsParent;
    [SerializeField] private CombatDetailsDivisionSlotUI _slotPrefab;
    [SerializeField] private Image _combatAllyField;
    [SerializeField] private Image _attackersFlag;
    [SerializeField] private Image _defendersFlag;

    private List<CombatDetailsDivisionSlotUI> _slots = new List<CombatDetailsDivisionSlotUI>();
    private DivisionCombat _divisionCombat;

    private void Update()
    {
        if (_divisionCombat != null)
        {
            var procent = _divisionCombat.GetProcentOfCombat();
            _combatAllyField.fillAmount = 1 - (float)Math.Acos(procent);
        }
    }

    public void RefreshUI(DivisionCombat combat)
    {
        _divisionCombat = combat;
        ClearSlots();
        foreach (var attacker in _divisionCombat.Attackers)
        {
            SpawnSlot(attacker, _attackDivisionsSlotsParent);
        }
        foreach (var defender in _divisionCombat.Defenders)
        {
            SpawnSlot(defender, _defensDivisionsSlotsParent);
        }
        _attackersFlag.sprite = combat.Attackers[0].CountyOwner.Flag;
        _defendersFlag.sprite = combat.Defenders[0].CountyOwner.Flag;
        combat.OnEnd += Close;
    }

    private void Close()
    {
        _divisionCombat = null;
        gameObject.SetActive(false);
    }

    private void ClearSlots()
    {
        foreach (var sl in _slots)
        {
            Destroy(sl.gameObject);
        }
        _slots.Clear();
    }

    private void SpawnSlot(Division division, Transform parent)
    {
        var slot = Instantiate(_slotPrefab, parent);
        slot.RefreshUI(division, _divisionCombat);
        _slots.Add(slot);
    }
}
