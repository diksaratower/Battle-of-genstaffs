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
    [SerializeField] private GuaranteeIndependenceDataViewUI _guaranteeIndependenceDataPrefab;

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
        if (playerRelation.IsWar == false && playerCountry.CountryDiplomacy.IsHaveWarGoal(targetCountry) == false &&
            playerCountry.CountryDiplomacy.GetJustificationQueue().Exists(slot => slot.Target == targetCountry) == false)
        {
            var button = AddActionButton("ќправдать войну");
            button.onClick.AddListener(delegate
            {
                playerCountry.CountryDiplomacy.StartJustificationWarGoal(targetCountry);
                RefreshUI(targetCountry, playerCountry);
            });
            _buttons.Add(button.gameObject);
        }
        if (playerRelation.IsWar == false && playerCountry.CountryDiplomacy.IsHaveWarGoal(targetCountry))
        {
            var button = AddActionButton("ќбъ€вить войну");
            button.onClick.AddListener(delegate
            {
                Diplomacy.Instance.DeclareWar(playerCountry, targetCountry);
                RefreshUI(targetCountry, playerCountry);
            });
            _buttons.Add(button.gameObject);
        }
        if (playerRelation.IsWar == false && Diplomacy.Instance.HaveGuaranteeIndependence(playerCountry, targetCountry) == false)
        {
            var button = AddActionButton("√арантировать независ.");
            button.onClick.AddListener(delegate
            {
                Diplomacy.Instance.GuaranteeIndependence(playerCountry, targetCountry);
                RefreshUI(targetCountry, playerCountry);
            });
            _buttons.Add(button.gameObject);
        }
    }

    private Button AddActionButton(string text)
    {
        var button = Instantiate(_diplomacyActionButtonPrefab, _actionButtonsParent);
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        return button;
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
            if (Diplomacy.Instance.HaveGuaranteeIndependence(country, targetCountry))
            {
                var guaranteeUI = Instantiate(_guaranteeIndependenceDataPrefab, _diplomacyDataParent);
                guaranteeUI.RefreshUI(country, targetCountry);
                _datesUI.Add(guaranteeUI.gameObject);
            }
        }
    }    
}
