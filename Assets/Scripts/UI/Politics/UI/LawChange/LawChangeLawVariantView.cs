using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LawChangeLawVariantView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lawNameText;
    [SerializeField] private Image _lawImage;
    [SerializeField] private TextMeshProUGUI _lawChangeCost;
    [SerializeField] private Button _setLawButton;
    [SerializeField] private LawDataTooltipHandlerUI _tooltipHandler;

    private Country _country;
    private Law _targetLaw;
    private ChangeLawData _changeLawData;

    public void RefreshUI(Law law, Country country, LawChangerPoliticsUI lawChanger, ChangeLawData changeLawData)
    {
        _changeLawData = changeLawData;
        _targetLaw = law;
        _country = country;
        _lawChangeCost.text = $"Стоимость: {law.PolitPowerCost} пв.";
        _lawNameText.text = law.Name;
        _lawImage.sprite = law.LawImage;
        _tooltipHandler.SetLaw(law);
        _setLawButton.onClick.AddListener(delegate 
        {
            if (changeLawData.CurrentLaw != law && law.PolitPowerCost < country.Politics.PolitPower)
            {
                changeLawData.ChangeLaw(law);
                lawChanger.RefreshCurrentLaw();
                lawChanger.RefreshChangeMenu();
            }
        });
    }

    private void Update()
    {
        if (_targetLaw != null && _changeLawData != null)
        {
            var interactable = _targetLaw.PolitPowerCost < _country.Politics.PolitPower && _changeLawData.CurrentLaw != _targetLaw;
            if (_targetLaw.PolitPowerCost > _country.Politics.PolitPower) 
            {
                _lawChangeCost.color = new Color(0.5f, 0, 0);
            }
            else 
            {
                _lawChangeCost.color = Color.black;
            }
            _setLawButton.interactable = interactable;
        }
    }
}
