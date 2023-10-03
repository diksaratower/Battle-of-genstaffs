using System;
using System.Collections.Generic;
using UnityEngine;


public class ChooseDifficultiesUI : MonoBehaviour
{
    public ChooseDifficultiesButtonUI Selected { get; set; }

    [SerializeField] private ChooseDifficultiesButtonUI _chooseButtonPrefab;
    [SerializeField] private Transform _chooseDifficultiesButtonsParent;

    private List<ChooseDifficultiesButtonUI> _chooseDifficultiesButtons = new List<ChooseDifficultiesButtonUI>();

    public void RefreshUI()
    {
        _chooseDifficultiesButtons.ForEach(button => Destroy(button.gameObject));
        _chooseDifficultiesButtons.Clear();
        var difficulties = new List<Difficultie>(DifficultiesData.GetInstance().Difficulties);
        foreach (var difficulte in difficulties)
        {
            var button = Instantiate(_chooseButtonPrefab, _chooseDifficultiesButtonsParent);
            _chooseDifficultiesButtons.Add(button);
            button.RefreshUI(difficulte, this);
        }
        Selected = _chooseDifficultiesButtons[0];
    }
}
