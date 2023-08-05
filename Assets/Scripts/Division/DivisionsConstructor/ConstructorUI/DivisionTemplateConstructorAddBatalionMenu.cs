using System.Collections.Generic;
using UnityEngine;


public class DivisionTemplateConstructorAddBatalionMenu : MonoBehaviour
{
    [SerializeField] private Transform _battalionsParent;
    [SerializeField] private DivisionTemplateConstructorAddBatalionMenuSlotUI _slotPrefab;

    private List<DivisionTemplateConstructorAddBatalionMenuSlotUI> _slotsUI = new List<DivisionTemplateConstructorAddBatalionMenuSlotUI>();


    public void RefreshUI(DivisionLine targetLine, DivisionTemplateConstructorUI constructorUI, Country country)
    {
        _slotsUI.ForEach(slot => Destroy(slot.gameObject));
        _slotsUI.Clear(); 
        var manager = TechnologiesManagerSO.GetInstance();
        foreach (var battalion in manager.AvailableBattalions)
        {
            if (battalion.CanUsed(country))
            {
                var battlionUI = Instantiate(_slotPrefab, _battalionsParent);
                battlionUI.RefreshUI(battalion, targetLine, constructorUI, this);
                _slotsUI.Add(battlionUI);
            }
        }
    }
}
