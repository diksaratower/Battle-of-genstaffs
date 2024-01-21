using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCountryMajorViewSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countryNameText;
    [SerializeField] private Image _leaderPortraitImage;
    [SerializeField] private Image _flagImage;
    [SerializeField] private Button _selectCountryButton;

    public void RefreshUI(CountrySO country, ChooseCountryMenu chooseCountryMenu)
    {
        _countryNameText.text = country.Name;
        _flagImage.sprite = country.CountryFlag;
        _leaderPortraitImage.sprite = country.CountryLeader.Portrait;

        _selectCountryButton.onClick.AddListener(delegate 
        {
            chooseCountryMenu.SetSelectedCountry(country);
        });
    }
}
