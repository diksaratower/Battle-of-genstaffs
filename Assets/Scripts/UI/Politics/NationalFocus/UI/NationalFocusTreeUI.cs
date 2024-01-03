using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class NationalFocusTreeUI : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _focusLevelUIPrefab;
    [SerializeField] private NationalFocusUI _focusUIPrefab;
    [SerializeField] private GridLayoutGroup _focusesLevelsLayout;
    [SerializeField] private Transform _focusesConnectionsParent;
    [SerializeField] private StartNationalFocusMenu _startFocusMenu;

    private List<FocusUILevel> _focusesUILevels = new List<FocusUILevel>();
    private Country _country => Player.CurrentCountry;

    private void Start()
    {
        RefreshUI();
        _country.Politics.OnFocusExecuted += RefreshUI;
    }

    private void RefreshUI()
    {
        var focusTree = _country.Politics.Preset.FocusTree;
        _focusesUILevels.ForEach(
        level => 
        { 
            Destroy(level.FocLayoutGroup.gameObject); 
        });
        _focusesUILevels.Clear();
        var currnetFocuses = new List<NationalFocus>() { focusTree.BaseFocus };
        AddFocusesLevel(currnetFocuses);
        for (int i = 0; i < 10000; i++)
        {
            var newCurrnetFocuses = focusTree.NationalFocuses.FindAll(foc => foc.NeedsForExecution.Intersect(currnetFocuses).Count() > 0);
            currnetFocuses = newCurrnetFocuses;
            if (currnetFocuses.Count == 0)
            {
                break;
            }
            AddFocusesLevel(currnetFocuses);
        }
        StartCoroutine(DrawFocusesBranchsIEnumerator());
    }

    private IEnumerator DrawFocusesBranchsIEnumerator()
    {
        yield return new WaitForEndOfFrame();
        DrawFocusesBranchs();
    }

    private void DrawFocusesBranchs()
    {
        var allUIs = GetAllFocusUIs();
        foreach (var focUIOne in allUIs)
        {
            foreach (var focUITwo in allUIs)
            {
                if (focUIOne != focUITwo)
                {
                    if (focUIOne.Focus.NeedsForExecution.Contains(focUITwo.Focus))
                    {
                        UILineCreation.MakeLine(focUIOne.GetComponent<RectTransform>(), focUITwo.GetComponent<RectTransform>(), _focusesConnectionsParent, Color.red);
                    }
                }
            }
        }
    }

    private List<NationalFocusUI> GetAllFocusUIs()
    {
        var result = new List<NationalFocusUI>();
        foreach (var focusLevel in _focusesUILevels)
        {
            result.AddRange(focusLevel.Focuses);
        }
        return result;
    }

    private void AddFocusesLevel(List<NationalFocus> focuses)
    {
        var layout = Instantiate(_focusLevelUIPrefab, _focusesLevelsLayout.transform);
        var focusesUI = new List<NationalFocusUI>();
        foreach (var focus in focuses) 
        {
            var focusUI = Instantiate(_focusUIPrefab, layout.transform);
            focusesUI.Add(focusUI);
            focusUI.RefreshUI(focus, _country.Politics, _startFocusMenu);
        }
        _focusesUILevels.Add(new FocusUILevel() { Focuses = focusesUI, FocLayoutGroup = layout});
    }


    private class FocusUILevel
    {
        public HorizontalLayoutGroup FocLayoutGroup;
        public List<NationalFocusUI> Focuses = new List<NationalFocusUI>();
    }
}

public static class UILineCreation
{
    public static GameObject MakeLine(RectTransform rectTransformA, RectTransform rectTransformB, Transform lineParent, Color col, float lineWidth = 0.1f)
    {
        GameObject lineObj = new GameObject();
        lineObj.transform.SetParent(lineParent);
        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = col;

        RectTransform rect = lineObj.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, lineWidth);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Vector3.Distance(rectTransformA.position, rectTransformB.position));
        rect.position = Vector3Extend.GetMiddlePoint(rectTransformA.position, rectTransformB.position);
        rect.LookAt(rectTransformB, Vector3.right);
        Vector3 dif = rectTransformA.position - rectTransformB.position;
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        return lineObj;
    }
}