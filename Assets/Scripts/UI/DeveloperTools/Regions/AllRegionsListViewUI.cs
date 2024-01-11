using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AllRegionsListViewUI : MonoBehaviour
{
    [SerializeField] private AllRegionsListViewSlotUI _regionSlotUIPrefab;
    [SerializeField] private Transform _regionsSlotsParent;
    [SerializeField] private TMP_InputField _regiopnSearchField;
    [SerializeField] private RegionRedactorUI _regionRedactorUI;
    [SerializeField] private Button _createRegionButton;

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
            regionUI.RefreshUI(region, _regionRedactorUI);
            _regionsViewSlots.Add(regionUI);
        }
        _createRegionButton.onClick.RemoveAllListeners();
        _createRegionButton.onClick.AddListener(delegate 
        {
            var region = new Region("New region "  + Random.Range(0, 100000).ToString(), 0);
            region.Provinces = new List<Province>();
            Map.Instance.MapRegions.Add(region);
            RefreshUI();
        });
    }
}
