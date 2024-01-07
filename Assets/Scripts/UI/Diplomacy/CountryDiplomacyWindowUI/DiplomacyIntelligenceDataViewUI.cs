using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DiplomacyIntelligenceDataViewUI : MonoBehaviour
{
    [SerializeField] private Image _currentFocusImage;
    [SerializeField] private TextMeshProUGUI _currentFocusName;
    [SerializeField] private TextMeshProUGUI _divisionsCountText;
    [SerializeField] private Image _focusExecuteFill;
    [SerializeField] private TextMeshProUGUI _factoryCountText;
    [SerializeField] private TextMeshProUGUI _militaryFactoryCountText;

    private Country _country;
    private NationalFocus _executingFocus;

    public void RefreshUI(Country target)
    {
        _country = target;
        _executingFocus = _country.Politics.ExecutingFocus;
        if(target.Politics.ExecutingFocus == null)
        {
            _currentFocusImage.sprite = null;
            _currentFocusName.text = "Не выбрано";
            _focusExecuteFill.fillAmount = 0;
            return;
        }
        _currentFocusImage.sprite = target.Politics.ExecutingFocus.Image;
        _currentFocusName.text = target.Politics.ExecutingFocus.Name;
    }

    private void Update()
    {
        if (_country != null)
        {
            _divisionsCountText.text = $"Количество дивизий: {UnitsManager.Instance.Divisions.FindAll(divsion => divsion.CountyOwner == _country).Count}";
            _focusExecuteFill.fillAmount = _country.Politics.GetProcentOfExecuteFocus();
            _factoryCountText.text = "Обычные заводы: " + _country.CountryBuild.GetCountryBuildings(BuildingType.Factory).Count;
            _militaryFactoryCountText.text = "Военные заводы: " + _country.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory).Count;
            if (_executingFocus != _country.Politics.ExecutingFocus)
            {
                RefreshUI(_country);
            }
        }
    }
}
