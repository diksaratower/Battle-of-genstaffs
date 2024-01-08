using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DivisionsUIViewer : MonoBehaviour
{
    [SerializeField] private DivisionView _divisionViewPrefab;
    [SerializeField] private DivisionUI _divisionUIPrefab;
    [SerializeField] private Transform _divisionsUIParent;
    [SerializeField] private SelectedDivisionsViewUI _selectedDivisionsViewUI;

    private GUISkin _selectedGUISkin;
    private List<DivisionView> _divisionsUI = new List<DivisionView>();
    private Vector3 _selectedRectStartPos;
    private bool _selectedRectDraw;


    public void Start()
    {
        foreach (var division in UnitsManager.Instance.Divisions)
        {
            OnDivisionEnterToProvince(division, division.DivisionProvince);
        }
        UnitsManager.OnDivisionEnterToProvince += OnDivisionEnterToProvince;
    }

    private void Update()
    {
        UpdateSelectedDivisionsViewUI();
    }

    private void OnGUI()
    {

        GUI.skin = _selectedGUISkin;
        GUI.depth = 99;
        if (GameIU.Instance.BlockDivisionSelecting)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !PointerConstainsUI())
        {
            GameIU.Instance.DeselectedAllDivisions();
            _selectedRectStartPos = Input.mousePosition;
            _selectedRectDraw = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _selectedRectDraw = false;
        }

        if (_selectedRectDraw)
        {
            var _selectedRectEndPos = Input.mousePosition;
            if (_selectedRectStartPos == _selectedRectEndPos) return;

            var rect = new Rect(Mathf.Min(_selectedRectEndPos.x, _selectedRectStartPos.x),
                            Screen.height - Mathf.Max(_selectedRectEndPos.y, _selectedRectStartPos.y),
                            Mathf.Max(_selectedRectEndPos.x, _selectedRectStartPos.x) - Mathf.Min(_selectedRectEndPos.x, _selectedRectStartPos.x),
                            Mathf.Max(_selectedRectEndPos.y, _selectedRectStartPos.y) - Mathf.Min(_selectedRectEndPos.y, _selectedRectStartPos.y)
                            );

            GUI.Box(rect, "");

            foreach (var divisionUI in _divisionsUI)
            {
                Vector2 tmp = new Vector2(divisionUI.DivisionUIScreenPosition().x,
                    Screen.height - divisionUI.DivisionUIScreenPosition().y);
                if (rect.Contains(tmp))
                {
                    if (divisionUI.Divisions[0].CountyOwner == Player.CurrentCountry)
                    {
                        GameIU.Instance.SelectDivisions(divisionUI.Divisions);
                    }
                }

            }
        }
    }

    private void OnDivisionEnterToProvince(Division division, Province province)
    {
        if (division.NeedDrawDivisionUI() == false)
        {
            return;
        }
        if (_divisionsUI.Exists(dUI => dUI.Divisions.Contains(division)))
        {
            _divisionsUI.Find(dUI => dUI.Divisions.Contains(division)).Divisions.Remove(division);
        }
        var divisionUI = _divisionsUI.Find(dUI => dUI.TargetProvince == province);
        if (divisionUI != null)
        {
            divisionUI.Divisions.Add(division);
            division.OnDivisionRemove += delegate 
            {
                divisionUI.Divisions.Remove(division);
                RemoveNotNeeedDivisionsUI();
            };
        }
        else
        {
            var divUI = Instantiate(_divisionViewPrefab);
            divUI.Initialize(new List<Division>(1) { division }, GameIU.Instance, province, _divisionUIPrefab, _divisionsUIParent);
            division.OnDivisionRemove += delegate 
            {
                divUI.Divisions.Remove(division);
                RemoveNotNeeedDivisionsUI();
            };
            _divisionsUI.Add(divUI);
        }
        RemoveNotNeeedDivisionsUI();
        if (division.CountyOwner == Player.CurrentCountry)
        {
            UpdateNotPlayerDivisions();
        }
    }

    private void UpdateSelectedDivisionsViewUI()
    {
        var selectedDivisions = GameIU.Instance.GetSelectedDivisions();
        if (selectedDivisions.Count > 0)
        {
            _selectedDivisionsViewUI.gameObject.SetActive(true);
            if (_selectedDivisionsViewUI.NeedUpdate(selectedDivisions))
            {
                _selectedDivisionsViewUI.RefreshUI(selectedDivisions);
            }
        }
        else
        {
            _selectedDivisionsViewUI.gameObject.SetActive(false);
        }
    }

    private void UpdateNotPlayerDivisions()
    {
        foreach (var division in UnitsManager.Instance.Divisions)
        {
            if (division.CountyOwner != Player.CurrentCountry && division.DivisionProvince.Contacts.Exists(p => p.Owner == Player.CurrentCountry))
            {
                OnDivisionEnterToProvince(division, division.DivisionProvince);
            }

            if (division.CountyOwner != Player.CurrentCountry && !division.NeedDrawDivisionUI())
            {
                if (_divisionsUI.Exists(dUI => dUI.Divisions.Contains(division)))
                {
                    _divisionsUI.Find(dUI => dUI.Divisions.Contains(division)).Divisions.Remove(division);
                }
            }
        }
    }

    private void RemoveNotNeeedDivisionsUI()
    {
        var removeDivisionsUI = _divisionsUI.FindAll(divUI => divUI.Divisions.Count == 0);
        foreach (var divisionUI in removeDivisionsUI)
        {
            _divisionsUI.Remove(divisionUI);
            Destroy(divisionUI.gameObject);
        }
    }

    private bool PointerConstainsUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
