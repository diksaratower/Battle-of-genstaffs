using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDivisionsViewUI : MonoBehaviour
{
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private SelectedDivisionsViewSlotUI _slotPrefab;
    [SerializeField] private GameIU _gameIU;

    private List<SelectedDivisionsViewSlotUI> _slotsUI = new List<SelectedDivisionsViewSlotUI>();

    public void RefreshUI(List<Division> divisions)
    {
        _slotsUI.ForEach(slot => { Destroy(slot.gameObject); });
        _slotsUI.Clear();
        foreach (Division division in divisions) 
        {
            var slot = Instantiate(_slotPrefab, _slotsParent);
            slot.RefreshUI(division, _gameIU);
            _slotsUI.Add(slot);
        }
    }

    public void DeleteSelectedDvision()
    {
        var divisions = new List<Division>();

        foreach (var slot in _slotsUI)
        {
            divisions.Add(slot.Division);
        }
        GameIU.Instance.DeselectDivisions(divisions);
        var removeCount = 0;
        foreach (var division in divisions)
        {
            if (division.CountyOwner == Player.CurrentCountry)
            {
                division.KillDivision();
                removeCount++;
            }
        }    
        divisions.RemoveAll(div => div == null);
        if(divisions.Count == removeCount)
        {
            gameObject.SetActive(false);
        }
        RefreshUI(divisions);
    }

    public void StopDivisions()
    {
        var divisions = new List<Division>();

        foreach (var slot in _slotsUI)
        {
            divisions.Add(slot.Division);
        }
        foreach (var division in divisions)
        {
            if (division.CountyOwner == Player.CurrentCountry)
            {
                division.StopDivision();
            }
        }
    }

    public bool NeedUpdate(List<Division> divisions)
    {
        var viewed = GetViewedDivisions();
        if ((divisions.FindAll(d => viewed.Contains(d)).Count == divisions.Count) && (divisions.Count == viewed.Count))
        {
            return false;
        }
        return true;
    }

    private List<Division> GetViewedDivisions()
    {
        var list = new List<Division>();
        foreach (var sl in _slotsUI)
        {
            list.Add(sl.Division);
        }
        return list;
    }
}
