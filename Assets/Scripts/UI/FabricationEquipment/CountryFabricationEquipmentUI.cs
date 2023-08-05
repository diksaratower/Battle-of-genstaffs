using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CountryFabricationEquipmentUI : MonoBehaviour
{
    public Country TargetCountry { get; private set; }

    [SerializeField] private TextMeshProUGUI _militaryFactoriesUses;
    [SerializeField] private TextMeshProUGUI _fabricationEfficiencyText;
    [SerializeField] private Transform _fabricationSlotsUIParent;
    [SerializeField] private CountryFabricationEquipmentSlotUI _fabricationSlotUIPrefab;

    private List<CountryFabricationEquipmentSlotUI> _fabricationSlotsUI = new List<CountryFabricationEquipmentSlotUI>();

    private void Awake()
    {
        TargetCountry = Player.CurrentCountry;
    }

    private void Start()
    {
        RefreshFabricationSlots();
        TargetCountry.CountryFabrication.OnAddedSlot += delegate 
        {
            RefreshFabricationSlots();
        };
        TargetCountry.CountryFabrication.OnRemovedSlot += delegate 
        {
            RefreshFabricationSlots();
        };
    }

    private void Update()
    {
        _militaryFactoriesUses.text = $"«аводов испульзуетс€ {GetUsesFactories()} / {TargetCountry.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory).Count}";
        _fabricationEfficiencyText.text = $"Ёффективность производства: {TargetCountry.CountryFabrication.GetCorrectFabricationEfficiency() * 100}%";
    }

    private void RefreshFabricationSlots()
    {
        _fabricationSlotsUI.ForEach(sl => { Destroy(sl.gameObject); });
        _fabricationSlotsUI.Clear();
        foreach (var sl in TargetCountry.CountryFabrication.EquipmentSlots)
        {
            var newSlotUI = Instantiate(_fabricationSlotUIPrefab, _fabricationSlotsUIParent);
            newSlotUI.RefreshSlot(sl, TargetCountry);
            _fabricationSlotsUI.Add(newSlotUI);
        }
    }

    private int GetUsesFactories()
    {
        var usesFactories = 0;

        foreach (var sl in TargetCountry.CountryFabrication.EquipmentSlots)
        {
            usesFactories += sl.Factories.Count;
        }
        return usesFactories;
    }
}
