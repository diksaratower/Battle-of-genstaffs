using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DiplomacyIntelligenceDataViewUI : MonoBehaviour
{
    [SerializeField] private Image _currentFocusImage;
    [SerializeField] private TextMeshProUGUI _currentFocusName;
    [SerializeField] private TextMeshProUGUI _divisionsCountText;
    [SerializeField] private Image _focusExecuteFill;

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
        _divisionsCountText.text = $"Количество дивизий: {UnitsManager.Instance.Divisions.FindAll(divsion => divsion.CountyOwner == target).Count}";
        _currentFocusName.text = target.Politics.ExecutingFocus.Name;
    }

    private void Update()
    {
        if (_country != null)
        {
            _focusExecuteFill.fillAmount = _country.Politics.GetProcentOfExecuteFocus();
            if (_executingFocus != _country.Politics.ExecutingFocus)
            {
                RefreshUI(_country);
            }
        }
    }
}
