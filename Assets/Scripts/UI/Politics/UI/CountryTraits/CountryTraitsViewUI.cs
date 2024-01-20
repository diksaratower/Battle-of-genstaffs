using System.Collections.Generic;
using UnityEngine;


public class CountryTraitsViewUI : MonoBehaviour
{
    [SerializeField] private CountryTraitUI _traitUIPrefab;
    [SerializeField] private Transform _traitsUIParent;

    private List<CountryTraitUI> _traitsUI = new List<CountryTraitUI>(); 

    public void Refresh(Country country)
    {
        _traitsUI.ForEach(trait => 
        {
            Destroy(trait.gameObject); 
        });
        _traitsUI.Clear();
        foreach (var trait in country.Politics.TraitSlots)
        {
            var traitUI = Instantiate(_traitUIPrefab, _traitsUIParent);
            traitUI.RefreshUI(trait);
            _traitsUI.Add(traitUI);
        }
    }
}
