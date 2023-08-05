using IJunior.TypedScenes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCountrySelectedView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countryNameText;
    [SerializeField] private Image _flagImage;
    [SerializeField] private Image _leaderImage;
    [SerializeField] private TextMeshProUGUI _leaderNameText;
    [SerializeField] private TextMeshProUGUI _countryDescriptionText;
    [SerializeField] private Button _startWithCountryButton;

    public void RefreshUI(CountrySO country)
    {
        _countryNameText.text = country.Name;
        _flagImage.sprite = country.CountryFlag;
        _leaderImage.sprite = country.Politics.CountryLeader.Portrait;
        _leaderNameText.text = country.Politics.CountryLeader.Name;
        _startWithCountryButton.onClick.AddListener(delegate 
        {
            FindObjectOfType<SinglePlayerModMenu>().LoadGameSceneUpdateQuick(country.ID);
        });
    }

}
