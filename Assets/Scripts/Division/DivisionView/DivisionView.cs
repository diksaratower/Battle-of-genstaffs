using System.Collections.Generic;
using UnityEngine;


public class DivisionView : MonoBehaviour
{
    public List<Division> Divisions = new List<Division>();
    public bool Selected => IsDivisionSelect();
    [HideInInspector] public Province TargetProvince;

    [SerializeField] private int _divisionsCount;

    private DivisionUI _divisionUI;
    private GameIU _gameIU;


    private void Update()
    {
        _divisionsCount = Divisions.Count;
        RemoveNotNeedDivisions();
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

    public void Initialize(List<Division> target, GameIU UI, Province province, List<DivisionView> divisionsUI, DivisionUI divisionUIPrefab, Transform divisionsUIParent)
    {
        _gameIU = UI;
        Divisions.Clear();
        Divisions.AddRange(target);
        TargetProvince = province;
        foreach (Division division in Divisions)
        {
            division.OnDivisionRemove += delegate
            {
                if (Divisions.Contains(division))
                {
                    Divisions.Remove(division);
                }
            };
            division.OnDivisionEnterToProvince += (Province pr) =>
            {
                if (pr != TargetProvince)
                {
                    if (Divisions.Contains(division))
                    {
                        Divisions.Remove(division);
                    }
                    if (divisionsUI.Exists(dUI => dUI.TargetProvince == pr))
                    {
                        divisionsUI.Find(dUI => dUI.TargetProvince == pr).Divisions.Add(division);
                    }
                }
            };
        }
        _divisionUI = Instantiate(divisionUIPrefab, divisionsUIParent);
        _divisionUI.RefreshUI(this, _gameIU);
        transform.position = province.LandscapeTruePosition;
        GetComponent<DivisionMovePlanUI>().Initialize();
    }

    private void RemoveNotNeedDivisions()
    {
        if (Divisions.Count == 0)
        {
            return;
        }
        Divisions.RemoveAll(division => division.IsDeleted == true);
        var notNeededDivisions = new List<Division>();
        foreach (var division in Divisions)
        {
            if (division != null)
            {
                if (division.NeedDrawDivisionUI() == false || division.DivisionProvince != TargetProvince)
                {
                    notNeededDivisions.Add(division);
                }
            }
        }
        notNeededDivisions.ForEach(division =>
        {
            if (Divisions.Contains(division))
            {
                Divisions.Remove(division);
            }
        });
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
