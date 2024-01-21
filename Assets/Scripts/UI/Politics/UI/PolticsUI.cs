using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private DecisionsUI _decisionsUI;

    private List<TextMeshProUGUI> _partiesReviewTexts = new List<TextMeshProUGUI>();
    private List<GameObject> _advisersUI = new List<GameObject>();
    private Country _country => Player.CurrentCountry;

    private void Start()
    {
        RefreshUI();
        _openNationalFocusesTree.onClick.AddListener(delegate 
        { 
        _nationalFocuseseWindow.gameObject.SetActive(true);
        });
        _decisionsUI.RefreshUI(this);
        AddActionsListeners();
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
       
        _aboutCountryText.text = @$"������: {UtilsPoliticsUI.ElectionTypeToString(_country.Politics.ElectionsType)}
���������: {_country.Politics.RulingParty.Name}";
        _leaderName.text = _country.Politics.CountryLeader.Name;
        _leaderPortrait.sprite = _country.Politics.CountryLeader.Portrait;
        UpdatePartiesPopular();
    }

    public void CloseAllChangeWindows()
    {
        foreach (var advisersChange in _advisersUI)
        {
            if (advisersChange.TryGetComponent<LawChangerPoliticsUI>(out var lawChangerUI))
            {
                lawChangerUI.CloseMenu();
            }
        }
    }

    private void AddActionsListeners()
    {
        _country.Politics.OnUpdateBlockedDecisions += delegate
        {
            _decisionsUI.RefreshUI(this);
        };
        _country.Politics.OnUpdatePartiesPopular += delegate
        {
            UpdatePartiesPopular();
        };
        _country.Politics.OnFocusExecuted += delegate
        {
            RefreshUI();
        };
        _country.Politics.OnAdvisersChange += delegate
        {
            RefreshAdvisers();
        };
    }

    private void UpdatePartiesPopular() 
    {
        _partiesReviewTexts.ForEach(prt => Destroy(prt.gameObject));
        _partiesReviewTexts.Clear();
        var sortedParties = _country.Politics.PartiesPopularData.OrderBy(party => -party.ProcentPopularity).ToList();
        foreach (var party in sortedParties)
        {
            var text = Instantiate(_partiesReviewTextPrefab, _partiesReviewLayoutGroup.transform);
            text.text = $"{party.Name} {System.Math.Round(party.ProcentPopularity, 2)}%";
            _partiesReviewTexts.Add(text);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_partiesReviewLayoutGroup.transform as RectTransform);
        _partiesPopularityChartUI.RefreshUI(_country.Politics.PartiesPopularData.ToList());
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
            _executionFocusName.text = "����������� �� �������";
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
            adviserUI.RefreshUI(pers, _country);
            _advisersUI.Add(adviserUI.gameObject);
        }
        if (_country.Politics.Advisers.Count < _advisersMaxCount)
        {
            for (int i = 0; i < _advisersMaxCount - _country.Politics.Advisers.Count; i++)
            {
                var addAdviserButton = Instantiate(_adviserAddButtonUIPrefab, _advisersLayoutParent.transform);
                addAdviserButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    CloseAllChangeWindows();
                    _addAdviserMenu.gameObject.SetActive(true);
                });
                _advisersUI.Add(addAdviserButton.gameObject);
            }
        }
        _addAdviserMenu.RefreshUI(_country.Politics.Preset.AvailableAdvisers.FindAll(adv => !_country.Politics.Advisers.Contains(adv)), _country);

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
            _country.Politics.EconomicsLaws, "���������");

        economicMenu.CreateMenu(_changeLawsMeniesParent, economicLawChangeData, this);

        _advisersUI.Add(economicMenu.gameObject);

        var conscriptionMenu = Instantiate(_conscriptionLawUIPrefab, _advisersLayoutParent.transform);
        var conscriptionLawChangeData = new ChangeLawData(
            () => _country.Politics.Current�onscriptionLaw,
            (Law law) =>
            {
                _country.Politics.ChangeCurrentConscrirtionLaw(law);
            },
            _country.Politics.�onscriptionLaws, "������");

        conscriptionMenu.CreateMenu(_changeLawsMeniesParent, conscriptionLawChangeData, this);

        _advisersUI.Add(conscriptionMenu.gameObject);
    }
}

public class UtilsPoliticsUI
{
    private UtilsPoliticsUI() 
    {
    }

    public static string ElectionTypeToString(CountryElectionsType electionsType)
    {
        var electionString = "";
        if (electionsType == CountryElectionsType.Constantly)
        {
            electionString = "���������";
        }
        if (electionsType == CountryElectionsType.NoElections)
        {
            electionString = "���";
        }
        return electionString;
    }
}
