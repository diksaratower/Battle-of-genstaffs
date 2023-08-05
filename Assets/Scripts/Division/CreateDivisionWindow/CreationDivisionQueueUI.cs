using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CreationDivisionQueueUI : MonoBehaviour
{
    [SerializeField] private CreationDivisionsQueueSlotUI _queueSlotUIPrefab;
    [SerializeField] private Transform _queueSlotsParent;
    [SerializeField] private TextMeshProUGUI _slotsCountText;

    private List<CreationDivisionsQueueSlotUI> _creationQueueSlotsUIs = new List<CreationDivisionsQueueSlotUI>();
    private Country _country => Player.CurrentCountry;

    private void Start()
    {
        _country.CreationDivisions.OnAddedSlotToQueue += delegate 
        {
            RefreshUI();
        };
        _country.CreationDivisions.OnRemovedSlotFromQueue += delegate
        {
            RefreshUI();
        };
        RefreshUI();
    }

    private void RefreshUI()
    {
        _slotsCountText.text = $"Разворачивается {_country.CreationDivisions.CreationQueue.Count}/{_country.CreationDivisions.MaxQueueSlots}";
        _creationQueueSlotsUIs.ForEach(slotUI => 
        {
            Destroy(slotUI.gameObject);
        });
        _creationQueueSlotsUIs.Clear();
        foreach (var slot in _country.CreationDivisions.CreationQueue)
        {
            var slotUI = Instantiate(_queueSlotUIPrefab, _queueSlotsParent);
            slotUI.RefreshUI(slot);
            _creationQueueSlotsUIs.Add(slotUI);
        }
    }
}
