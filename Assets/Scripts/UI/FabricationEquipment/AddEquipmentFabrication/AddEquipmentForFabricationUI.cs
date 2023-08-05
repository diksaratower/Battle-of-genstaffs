using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AddEquipmentForFabricationUI : MonoBehaviour
{
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private AddEquipmentForFabricationSlotUI _slotUIPrefab;
    [SerializeField] private CountryFabricationEquipmentUI _fabricationEquipmentUI;

    private List<AddEquipmentForFabricationSlotUI> _slotsUI = new List<AddEquipmentForFabricationSlotUI>();

    private void Start()
    {
        RefreshUI(_fabricationEquipmentUI.TargetCountry);
        _fabricationEquipmentUI.TargetCountry.CountryFabrication.OnAddedSlot += delegate
        {
            RefreshUI(_fabricationEquipmentUI.TargetCountry);
        };
        _fabricationEquipmentUI.TargetCountry.CountryFabrication.OnRemovedSlot += delegate
        {
            RefreshUI(_fabricationEquipmentUI.TargetCountry);
        };
        _fabricationEquipmentUI.TargetCountry.Research.OnResearchedTech += delegate
        {
            RefreshUI(_fabricationEquipmentUI.TargetCountry);
        };
    }

    public void RefreshUI(Country country)
    {
        _slotsUI.ForEach(sl => { Destroy(sl.gameObject); });
        _slotsUI.Clear();
        var techs = country.Research.GetOpenedTechnologies();
        foreach (var tech in techs) 
        {
            if (tech is EquipmentTechnology)
            {
                if (!country.CountryFabrication.EquipmentIsFabricating((tech as EquipmentTechnology).UnlockEquipment))
                {
                    var slot = Instantiate(_slotUIPrefab, _slotsParent);
                    slot.RefreshUI(tech, _fabricationEquipmentUI, this);
                    _slotsUI.Add(slot);
                }
            }
        }
    }
}
