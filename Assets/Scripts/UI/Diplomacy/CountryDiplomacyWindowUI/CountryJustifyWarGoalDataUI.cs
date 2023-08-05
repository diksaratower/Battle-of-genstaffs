using UnityEngine;
using UnityEngine.UI;


public class CountryJustifyWarGoalDataUI : MonoBehaviour
{
    [SerializeField] private Image _countryFlag;


    public void RefreshUI(Country country)
    {
        _countryFlag.sprite = country.Flag;
    }
}
