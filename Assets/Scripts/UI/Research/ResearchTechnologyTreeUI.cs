using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTechnologyTreeUI : MonoBehaviour
{
    [SerializeField] private ResearchTreeLevelUI _treeLevelUIPrefab;

    private Transform _connectionsParent;
    private List<ResearchTreeLevelUI> _focusesUILevels = new List<ResearchTreeLevelUI>();
    private List<GameObject> _connectionsLines = new List<GameObject>();

    public void RefreshUI(TechnologiesTree techTree, Transform connectionParent, RectTransform treesParent)
    {
        _connectionsParent = connectionParent;
        _focusesUILevels.ForEach(
        level =>
        {
            Destroy(level.gameObject);
        });
        _focusesUILevels.Clear();
        var currnetFocuses = new List<Technology>() { techTree.BaseTechnology };
        AddFocusesLevel(currnetFocuses);
        for (int i = 0; i < 10000; i++)
        {
            var newCurrnetFocuses = techTree.Technologies.FindAll(tech => tech.NeededTech.Intersect(currnetFocuses).Count() > 0);
            currnetFocuses = newCurrnetFocuses;
            if (currnetFocuses.Count == 0)
            {
                break;
            }
            AddFocusesLevel(currnetFocuses);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(treesParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        StartCoroutine(DrawFocusesBranchsIEnumerator());
        //DrawFocusesBranchs();
    }

    private void AddFocusesLevel(List<Technology> technologies)
    {
        var level = Instantiate(_treeLevelUIPrefab, transform);
        level.RefreshUI(technologies);
        _focusesUILevels.Add(level);
    }

    private IEnumerator DrawFocusesBranchsIEnumerator()
    {
        yield return null;
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
                    if (focUIOne.TargetTechnology.NeededTech.Contains(focUITwo.TargetTechnology))
                    {
                        var line = UILineCreation.MakeLine(focUIOne.GetComponent<RectTransform>(), focUITwo.GetComponent<RectTransform>(), _connectionsParent, Color.red);
                        _connectionsLines.Add(line);
                    }
                }
            }
        }
    }

    private List<ResearchTechnologyUI> GetAllFocusUIs()
    {
        var result = new List<ResearchTechnologyUI>();
        foreach (var focusLevel in _focusesUILevels)
        {
            result.AddRange(focusLevel.Technologies);
        }
        return result;
    }

    private void OnDestroy()
    {
        _connectionsLines.ForEach(line => 
        {
            Destroy(line);
        });
    }
}
