using UnityEngine;
using UnityEngine.UI;


public class CountryWarDataUI : MonoBehaviour
{
    [SerializeField] private Image _countryFlag;


    public void RefreshUI(Country country)
    {
        _countryFlag.sprite = country.Flag;
    }
}
