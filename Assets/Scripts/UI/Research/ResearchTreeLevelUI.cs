using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResearchTreeLevelUI : MonoBehaviour
{
    public List<ResearchTechnologyUI> Technologies = new List<ResearchTechnologyUI>();
    
    [SerializeField] private ResearchTechnologyUI _technologyUIPrefab;
    [SerializeField] private HorizontalLayoutGroup _layoutGroup;


    public void RefreshUI(List<Technology> technologies)
    {
        foreach (var technology in technologies)
        {
            var techUI = Instantiate(_technologyUIPrefab, _layoutGroup.transform);
            techUI.RefreshUI(technology);
            Technologies.Add(techUI);
        }
    }
}
