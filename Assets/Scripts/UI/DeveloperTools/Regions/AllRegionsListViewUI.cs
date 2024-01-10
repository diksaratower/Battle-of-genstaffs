using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class AllRegionsListViewUI : MonoBehaviour
{
    [SerializeField] private AllRegionsListViewSlotUI _regionSlotUIPrefab;
    [SerializeField] private Transform _regionsSlotsParent;
    [SerializeField] private TMP_InputField _regiopnSearchField;

    private List<AllRegionsListViewSlotUI> _regionsViewSlots = new List<AllRegionsListViewSlotUI>();


    private void Start()
    {
        _regiopnSearchField.onValueChanged.AddListener(delegate
        {
            RefreshUI();
        });
        RefreshUI();
    }

    private void RefreshUI()
    {
        var useSearch = (_regiopnSearchField.text != "");
        _regionsViewSlots.ForEach(slot => 
        {
            Destroy(slot.gameObject);
        });
        _regionsViewSlots.Clear();
        foreach (var region in Map.Instance.MapRegions)
        {
            if (useSearch)
            {
                var regionName = region.Name.ToLower();
                var query = _regiopnSearchField.text.ToLower();

                if (!regionName.Contains(query))
                {
                    continue;
                }
            }
            var regionUI = Instantiate(_regionSlotUIPrefab, _regionsSlotsParent);
            regionUI.RefreshUI(region);
            _regionsViewSlots.Add(regionUI);
        }
    }
}
