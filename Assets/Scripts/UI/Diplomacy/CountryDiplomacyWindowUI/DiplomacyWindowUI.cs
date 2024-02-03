using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DiplomacyWindowUI : MonoBehaviour
{
    [SerializeField] private Diplomacy _diplomacy;
    [SerializeField] private TextMeshProUGUI _countryNameText;
    [SerializeField] private TextMeshProUGUI _aboutCountryText;
    [SerializeField] private TextMeshProUGUI _relationsText;
    [SerializeField] private Button _intelligenceButton;
    [SerializeField] private Button _diplomacyButton;
    [SerializeField] private TextMeshProUGUI _leaderNameText;
    [SerializeField] private Image _leaderImage;
    [SerializeField] private Image _flagImage;
    [SerializeField] private DiplomacyIntelligenceDataViewUI _intelligenceDataViewUI;
    [SerializeField] private DiplomacyCountryViewUI _diplomacyViewUI;
    [SerializeField] private CountryTraitsViewUI _countryTraitsViewUI;


    private void Start()
    {
        _intelligenceButton.onClick.AddListener(delegate 
        {
            _diplomacyViewUI.gameObject.SetActive(false);
            _intelligenceDataViewUI.gameObject.SetActive(true);
        });
        _diplomacyButton.onClick.AddListener(delegate 
        {
            _intelligenceDataViewUI.gameObject.SetActive(false);
            _diplomacyViewUI.gameObject.SetActive(true);
        });
    }

    public void RefreshUI(Country targetCountry, Country playerCountry)
    {
        _diplomacyViewUI.RefreshUI(targetCountry, playerCountry);
        _intelligenceDataViewUI.RefreshUI(targetCountry);
        var playerRelation = _diplomacy.GetRelationWithCountry(playerCountry, targetCountry);
     
        _countryNameText.text = targetCountry.Name;
        _relationsText.text = "Отношения: " + playerRelation.Relation;
        _aboutCountryText.text = @$"
Выборы: {UtilsPoliticsUI.ElectionTypeToString(targetCountry.Politics.ElectionsType)}
Идеалогия: {targetCountry.Politics.RulingParty.Name}
";
        _leaderNameText.text = targetCountry.Politics.CountryLeader.Name;
        _flagImage.sprite = targetCountry.Flag;
        if(targetCountry.Politics.CountryLeader.Portrait != null)
        {
            _leaderImage.sprite = targetCountry.Politics.CountryLeader.Portrait;
        }
        _countryTraitsViewUI.Refresh(targetCountry);
    }
}
