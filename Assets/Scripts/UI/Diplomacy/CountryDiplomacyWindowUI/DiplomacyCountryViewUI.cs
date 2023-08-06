using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DiplomacyCountryViewUI : MonoBehaviour
{
    [SerializeField] private Button _diplomacyActionButtonPrefab;
    [SerializeField] private Transform _actionButtonsParent;
    [SerializeField] private Transform _diplomacyDataParent;
    [SerializeField] private CountryJustifyWarGoalDataUI _diplomacyJustifyWarGoalDataPrefab;
    [SerializeField] private CountryWarDataUI _diplomacyWarDataPrefab;

    private List<GameObject> _buttons = new List<GameObject>();
    private List<GameObject> _datesUI = new List<GameObject>();

    public void RefreshUI(Country targetCountry, Country playerCountry)
    {
        var playerRelation = Diplomacy.Instance.GetRelationWithCountry(playerCountry, targetCountry);
        playerCountry.CountryDiplomacy.OnAddWarGoal += delegate
        {
            RefreshUI(targetCountry, playerCountry);
        };
        RefreshDates(targetCountry);
        RefreshButtons(playerRelation, playerCountry, targetCountry);
    }

    private void RefreshButtons(DiplomaticRelationsWithCountry playerRelation, Country playerCountry, Country targetCountry)
    {
        _buttons.ForEach(button => Destroy(button));
        _buttons.Clear();
        if (playerRelation.IsWar == false && playerCountry.CountryDiplomacy.IsHaveWarGoal(targetCountry) == false)
        {
            var button = Instantiate(_diplomacyActionButtonPrefab, _actionButtonsParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "ќправдать войну";
            button.onClick.AddListener(delegate
            {
                playerCountry.CountryDiplomacy.StartJustificationWarGoal(targetCountry);
                RefreshUI(targetCountry, playerCountry);
            });
            _buttons.Add(button.gameObject);
        }
        if (playerRelation.IsWar == false && playerCountry.CountryDiplomacy.IsHaveWarGoal(targetCountry))
        {
            var button = Instantiate(_diplomacyActionButtonPrefab, _actionButtonsParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "ќбъ€вить войну";
            button.onClick.AddListener(delegate
            {
                Diplomacy.Instance.DeclareWar(playerCountry, targetCountry);
                RefreshUI(targetCountry, playerCountry);
            });
            _buttons.Add(button.gameObject);
        }
    }

    private void RefreshDates(Country targetCountry)
    {
        _datesUI.ForEach(dataUI => Destroy(dataUI));
        _datesUI.Clear();
        foreach (var country in Map.Instance.Countries)
        {
            if (country == targetCountry)
            {
                continue;
            }
            var relation = Diplomacy.Instance.GetRelationWithCountry(country, targetCountry);
            if (relation.IsWar == true)
            {
                var warUI = Instantiate(_diplomacyWarDataPrefab, _diplomacyDataParent);
                warUI.RefreshUI(country);
                _datesUI.Add(warUI.gameObject);
            }
            if (country.CountryDiplomacy.GetJustificationQueue().Exists(slot => slot.Target == targetCountry))
            {
                var justifyUI = Instantiate(_diplomacyJustifyWarGoalDataPrefab, _diplomacyDataParent);
                justifyUI.RefreshUI(country, country.CountryDiplomacy.GetJustificationQueue().Find(slot => slot.Target == targetCountry));
                _datesUI.Add(justifyUI.gameObject);
            }
        }
    }    
}
