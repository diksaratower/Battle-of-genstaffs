using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolticsUI : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _advisersLayoutParent;
    [SerializeField] private PoliticsAdviserUI _adviserUIPrefab;
    [SerializeField] private Button _adviserAddButtonUIPrefab;
    [SerializeField] private Image _countryFlag;
    [SerializeField] private TextMeshProUGUI _countryNameText;
    [SerializeField] private AddAdviserMenuUI _addAdviserMenu;
    [SerializeField] private PartiesPopularityChartUI _partiesPopularityChartUI;
    [SerializeField] private int _advisersMaxCount = 4;
    [SerializeField] private VerticalLayoutGroup _partiesReviewLayoutGroup;
    [SerializeField] private TextMeshProUGUI _partiesReviewTextPrefab;
    [SerializeField] private TextMeshProUGUI _aboutCountryText;
    [SerializeField] private TextMeshProUGUI _leaderName;
    [SerializeField] private Image _leaderPortrait;
    [SerializeField] private LawChangerPoliticsUI _economicLawUIPrefab;
    [SerializeField] private LawChangerPoliticsUI _conscriptionLawUIPrefab;
    [SerializeField] private Transform _changeLawsMeniesParent;
    [SerializeField] private NationalFocusTreeUI _nationalFocuseseWindow;
    [SerializeField] private Button _openNationalFocusesTree;
    [SerializeField] private Image _focusExecuteFill;
    [SerializeField] private Image _executionFocusImage;
    [SerializeField] private TextMeshProUGUI _executionFocusName;
    [SerializeField] private Sprite _nullFocusSprite;
    [SerializeField] private CountryTraitsViewUI _countryTraitsViewUI;

    private List<TextMeshProUGUI> _partiesReviewTexts = new List<TextMeshProUGUI>();
    private List<GameObject> _advisersUI = new List<GameObject>();
    private Country _country => Player.CurrentCountry;

    private void Start()
    {
        RefreshUI();
        _openNationalFocusesTree.onClick.AddListener(delegate { 
        _nationalFocuseseWindow.gameObject.SetActive(true);
        });

    }

    private void Update()
    {
        UpdateFocusExecuteView();
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        _countryFlag.sprite = _country.Flag;
        _countryNameText.text = _country.Name;
        RefreshAdvisers();
        _partiesPopularityChartUI.RefreshUI(_country.Politics.Parties);
        _aboutCountryText.text = @$"Âûáîðû: {_country.Politics.ElectionsType}
Ôîðìà ïðàâëåíèÿ: {_country.Politics.FormGovernment}
Èäåàëîãèÿ: {_country.Politics.CountryIdeology}";
        _leaderName.text = _country.Politics.CountryLeader.Name;
        _leaderPortrait.sprite = _country.Politics.CountryLeader.Portrait;
        UpdatePartiesPopular();
    }

    public bool CanAddAdviser(Personage adviser)
    {
        if(_country.Politics.PolitPower > 150)
        {
            return true;
        }
        return false;
    }

    public void AddAdviser(Personage adviser)
    {
        _country.Politics.AddAdviser(adviser);
        RefreshAdvisers();
    }

    private void UpdatePartiesPopular() 
    {
        _partiesReviewTexts.ForEach(prt => Destroy(prt.gameObject));
        _partiesReviewTexts.Clear();
        foreach (var party in _country.Politics.Parties)
        {
            var text = Instantiate(_partiesReviewTextPrefab, _partiesReviewLayoutGroup.transform);
            text.text = $"{party.Name} {System.Math.Round(party.ProcentPopularity, 2)}%";
            _partiesReviewTexts.Add(text);
        }
    }

    private void UpdateFocusExecuteView()
    {
        if(_country.Politics.ExecutingFocus != null)
        {
            _executionFocusName.text = _country.Politics.ExecutingFocus.Name;
            _executionFocusImage.sprite = _country.Politics.ExecutingFocus.Image;
            _focusExecuteFill.fillAmount = _country.Politics.GetProcentOfExecuteFocus();
        }
        else
        {
            _executionFocusName.text = "Íàïðàâëåíèå íå âûáðàíî";
            _executionFocusImage.sprite = _nullFocusSprite;
            _focusExecuteFill.fillAmount = 0;
        }
    }

    private void RefreshAdvisers()
    {
        _advisersUI.ForEach(adv => { Destroy(adv.gameObject); });
        _advisersUI.Clear();
        foreach (var pers in _country.Politics.Advisers)
        {
            var adviserUI = Instantiate(_adviserUIPrefab, _advisersLayoutParent.transform);
            adviserUI.RefreshUI(pers);
            _advisersUI.Add(adviserUI.gameObject);
        }
        if (_country.Politics.Advisers.Count < _advisersMaxCount)
        {
            for (int i = 0; i < _advisersMaxCount - _country.Politics.Advisers.Count; i++)
            {
                var addAdviserButton = Instantiate(_adviserAddButtonUIPrefab, _advisersLayoutParent.transform);
                addAdviserButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _addAdviserMenu.gameObject.SetActive(true);
                });
                _advisersUI.Add(addAdviserButton.gameObject);
            }
        }
        _addAdviserMenu.RefreshUI(_country.Politics.Preset.AvailableAdvisers.FindAll(adv => !_country.Politics.Advisers.Contains(adv)));

        AddLawsMenus();
        _countryTraitsViewUI.Refresh(_country);
    }

    private void AddLawsMenus()
    {
        var economicMenu = Instantiate(_economicLawUIPrefab, _advisersLayoutParent.transform);
        var economicLawChangeData = new ChangeLawData(
            () => _country.Politics.CurrentEconomicLaw,
            (Law law) =>
            {
                _country.Politics.ChangeCurrentEconomicLaw(law);
            },
            _country.Politics.EconomicsLaws, "Ýêîíîìèêà");

        economicMenu.CreateMenu(_changeLawsMeniesParent, economicLawChangeData);

        _advisersUI.Add(economicMenu.gameObject);

        var conscriptionMenu = Instantiate(_conscriptionLawUIPrefab, _advisersLayoutParent.transform);
        var conscriptionLawChangeData = new ChangeLawData(
            () => _country.Politics.CurrentÑonscriptionLaw,
            (Law law) =>
            {
                _country.Politics.ChangeCurrentConscrirtionLaw(law);
            },
            _country.Politics.ÑonscriptionLaws, "Ïðèçûâ");

        conscriptionMenu.CreateMenu(_changeLawsMeniesParent, conscriptionLawChangeData);

        _advisersUI.Add(conscriptionMenu.gameObject);
    }
}
