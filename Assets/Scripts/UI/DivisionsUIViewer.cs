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


    private void Update()
    {
        UpdateDivisionsUI();
        RemoveNotNeeedDivisionsUI();
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

    private void UpdateDivisionsUI()
    {
        var divisionsAll = UnitsManager.Instance.Divisions.FindAll(d => d.NeedDrawDivisionUI());
        var provinces = new List<Province>();
        divisionsAll.ForEach(div =>
        {
            if (provinces.Contains(div.DivisionProvince) == false)
            {
                provinces.Add(div.DivisionProvince);
            }
        });
        foreach (var province in provinces)
        {
            var divisionsForSlot = divisionsAll.FindAll(d => d.DivisionProvince == province);
            if (_divisionsUI.Exists(dUI => (dUI.TargetProvince == province && ListIsEquals(dUI.Divisions, divisionsForSlot))) == false)
            {
                var divUI = Instantiate(_divisionViewPrefab);
                divUI.Initialize(divisionsForSlot, GameIU.Instance, province, _divisionsUI, _divisionUIPrefab, _divisionsUIParent);
                _divisionsUI.Add(divUI);
            }
            else
            {
            }
        }
    }

    private bool ListIsEquals(List<Division> list1, List<Division> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }
        return list1.FindAll(element => list2.Contains(element)).Count == list1.Count;
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

    private void RemoveNotNeeedDivisionsUI()
    {
        var removeDivisionsUI = _divisionsUI.FindAll(divUI => divUI.Divisions.Count == 0);

        foreach (var oneDivisionUI in _divisionsUI)
        {
            foreach (var twoDivisionUI in _divisionsUI)
            {
                if (oneDivisionUI == twoDivisionUI)
                {
                    continue;
                }
                if(oneDivisionUI.Divisions.All(division => twoDivisionUI.Divisions.Contains(division)))
                {
                    removeDivisionsUI.Add(twoDivisionUI);
                }
            }
        }
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
