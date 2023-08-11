using System.Collections.Generic;
using UnityEngine;


public class AviationModeUI : MonoBehaviour
{
    private List<AviationDivision> _selectedAviationDivsions = new List<AviationDivision>();
    
    [SerializeField] private SelectedAviabaseUI _selectedAviabaseUI;

    public void RefreshUI(BuildingSlotRegion aviabase)
    {
        _selectedAviationDivsions.Clear();
        _selectedAviabaseUI.RefreshUI(aviabase, this);
    }

    public void MoveSelectedDivisions(BuildingSlotRegion newAviabase)
    {
        foreach (var selectedDivision in _selectedAviationDivsions)
        {
            if (newAviabase == selectedDivision.PositionAviabase)
            {
                continue;
            }
            selectedDivision.Move(newAviabase);
        }
        RefreshUI(newAviabase);
    }

    public void SelectAviationDivision(AviationDivision aviationDivision)
    {
        if (_selectedAviationDivsions.Contains(aviationDivision) == false)
        {
            _selectedAviationDivsions.Add(aviationDivision);
        }
    }

    public void DeselectAllAviationDivisions()
    {
        _selectedAviationDivsions.Clear();
    }

    public List<AviationDivision> GetSelectedAviationDivisions()
    {
        return new List<AviationDivision>(_selectedAviationDivsions);
    }
}
