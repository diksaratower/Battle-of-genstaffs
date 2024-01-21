using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddAdviserButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _adviserImage;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _traitText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private AdviserDataTooltipHandlerUI _tooltipHandler;

    private Country _country;
    private Personage _personage;


    public void RefreshUI(Personage personage, PolticsUI polticsUI, Country country)
    {
        _country = country;
        _personage = personage;
        _nameText.text = personage.GetName();
        _adviserImage.sprite = personage.GetPortrait();
        _costText.text = $"Стоимость: {personage.AdviserCost} пв.";
        _tooltipHandler.SetAdviser(personage);
        var trait = personage.Traits.Find(tr => tr is AdviserTrait);
        if (trait != null)
        {
            _traitText.text = (trait as AdviserTrait).TraitName;
        }
        _button.onClick.AddListener(delegate 
        {
            if (_country.Politics.CanAddAdviser(_personage))
            {
                _country.Politics.AddAdviser(personage);
            }
        });
    }

    public void Update()
    {
        _button.interactable = _country.Politics.CanAddAdviser(_personage);
    }
}
