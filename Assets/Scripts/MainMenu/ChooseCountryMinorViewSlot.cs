using UnityEngine;
using UnityEngine.UI;

public class ChooseCountryMinorViewSlot : MonoBehaviour
{
    [SerializeField] private Image _flagImage;
    [SerializeField] private Button _selectButton;

    public void RefreshUI(CountrySO country, ChooseCountryMenu chooseCountryMenu)
    {
        _flagImage.sprite = country.CountryFlag;
        _selectButton.onClick.AddListener(delegate 
        {
            chooseCountryMenu.SetSelectedCountry(country);
        });
    }
}
