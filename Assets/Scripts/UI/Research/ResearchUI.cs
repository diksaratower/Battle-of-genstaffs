using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchUI : MonoBehaviour
{
    public Action<TechnologyCategory> OnChangeCategory;

    [SerializeField] private TechnologiesManagerSO _technologiesManager;
    [SerializeField] private TextMeshProUGUI _researchPointsText;
    [SerializeField] private Transform _techTreesParent;
    [SerializeField] private Transform _connectionsParent;
    [SerializeField] private ResearchTechnologyTreeUI _technologyTreeUIPrefab;
    [SerializeField] private RectTransform _reserchPointTextParent;

    private Country _country;
    private List<ResearchTechnologyTreeUI> _treesUI = new List<ResearchTechnologyTreeUI>();
    private int _researchPointsTextCharsCount = 0;


    private void Start()
    {
        _country = Player.CurrentCountry;
        RefreshUI(TechnologyCategory.Infantry);
    }

    private void Update()
    {
        UpdateResearchPoints();
    }
    
    public void RefreshUI(TechnologyCategory category)
    {
        OnChangeCategory?.Invoke(category);
        _treesUI.ForEach(tree => 
        {
            Destroy(tree.gameObject);
        });
        _treesUI.Clear();
        foreach (var tree in _technologiesManager.TechnologiesTrees)
        {
            if (tree.Category != category)
            {
                continue;
            }
            var treeUI = Instantiate(_technologyTreeUIPrefab, _techTreesParent);
            treeUI.RefreshUI(tree, _connectionsParent, _techTreesParent as RectTransform);
            _treesUI.Add(treeUI);
        }
    }

    private void UpdateResearchPoints()
    {
        var researchPointsStringText = ("Очки исследования: " + Math.Round(_country.Research.ResearchPointCount, 2).ToString("0.00"));
        _researchPointsText.text = researchPointsStringText;
        if (researchPointsStringText.Length != _researchPointsTextCharsCount)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_reserchPointTextParent);
            _researchPointsTextCharsCount = researchPointsStringText.Length;
        }
    }
}
