using System.Collections.Generic;
using UnityEngine;


public class DivisionView : MonoBehaviour
{
    public List<Division> Divisions = new List<Division>();
    public bool Selected => IsDivisionSelect();
    [HideInInspector] public Province TargetProvince;

    private DivisionUI _divisionUI;
    private GameIU _gameIU;


    private void Update()
    {
        if (Divisions.Count == 0)
        {
            return;
        }
    }

    private void OnDestroy()
    {
        if (_divisionUI != null)
        {
            Destroy(_divisionUI.gameObject);
        }
    }

    public void Initialize(List<Division> divisions, GameIU UI, Province province, DivisionUI divisionUIPrefab, Transform divisionsUIParent)
    {
        _gameIU = UI;
        Divisions.Clear();
        Divisions.AddRange(divisions);
        TargetProvince = province;
        _divisionUI = Instantiate(divisionUIPrefab, divisionsUIParent);
        _divisionUI.RefreshUI(this, _gameIU);
        transform.position = province.LandscapeTruePosition;
        GetComponent<DivisionMovePlanUI>().Initialize();
    }

    private bool IsDivisionSelect()
    {
        var selected = false;
        foreach (var division in Divisions)
        {
            selected = _gameIU.IsDivisionSelect(division);
        }
        return selected;
    }

    public Division GetOverageDivision()
    {
        if (Divisions.Count > 0)
        {
            return Divisions[0];
        }
        return null;
    }

    public Vector3 DivisionUIScreenPosition()
    {
        if (_divisionUI == null)
        {
            return Vector3.zero;
        }
        return _divisionUI.transform.position;
    }
}
