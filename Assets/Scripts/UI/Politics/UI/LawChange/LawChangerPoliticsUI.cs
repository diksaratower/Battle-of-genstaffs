using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LawChangerPoliticsUI : MonoBehaviour
{
    [SerializeField] private Button _openChangeLawMenuButton;
    [SerializeField] private Image _currentLawImage;
    [SerializeField] private TextMeshProUGUI _lawNameText;
    [SerializeField] private LawChangeLawVariantView _lawViewUIPrefab;
    [SerializeField] private GameObject _changeMenuPrefab;

    private GameObject _changeMenu;
    private GridLayoutGroup _lawViewsParent;
    private List<LawChangeLawVariantView> _lawUIs = new List<LawChangeLawVariantView>();
    private ChangeLawData _changeLawData;
    private Country _country => Player.CurrentCountry;

    private void Start()
    {
        _openChangeLawMenuButton.onClick.AddListener(delegate 
        {
            _changeMenu.SetActive(true);
            RefreshChangeMenu();
        });
        RefreshCurrentLaw();
    }

    public void CreateMenu(Transform menuParent, ChangeLawData changeLawData)
    {
        _changeLawData = changeLawData;
        _lawNameText.text = _changeLawData.LawName;
        _changeMenu = Instantiate(_changeMenuPrefab, menuParent);
        _lawViewsParent = _changeMenu.GetComponentInChildren<GridLayoutGroup>();
        _changeMenu.SetActive(false);
    }

    public void RefreshCurrentLaw()
    {
        _currentLawImage.sprite = _changeLawData.CurrentLaw.LawImage;
    }

    public void RefreshChangeMenu()
    {
        _lawUIs.ForEach(la => Destroy(la.gameObject));
        _lawUIs.Clear();
        foreach (var law in _changeLawData.AvailbleLaws)
        {
            var lawUI = Instantiate(_lawViewUIPrefab, _lawViewsParent.transform);
            lawUI.RefreshUI(law, _country, this, _changeLawData);
            _lawUIs.Add(lawUI);
        }
    }
}

public class ChangeLawData
{
    public Law CurrentLaw => _getCurrentLawFunc();
    public List<Law> AvailbleLaws => _availbleLaws;
    public string LawName { get; }

    private Func<Law> _getCurrentLawFunc;
    private Action<Law> _setCurrentLawFunc;
    private List<Law> _availbleLaws;

    public ChangeLawData(Func<Law> getCurrentLawFunc, Action<Law> setCurrentLawFunc, List<Law> availbleLaws, string lawName)
    {
        _getCurrentLawFunc = getCurrentLawFunc;
        _setCurrentLawFunc = setCurrentLawFunc;
        _availbleLaws = availbleLaws;
        LawName = lawName;
    }

    public void ChangeLaw(Law newLaw)
    {
        _setCurrentLawFunc?.Invoke(newLaw);
    }
}