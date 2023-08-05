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

    public void RefreshUI(Personage personage, PolticsUI polticsUI)
    {
        _nameText.text = personage.GetName();
        _adviserImage.sprite = personage.GetPortrait();
        _costText.text = $"Стоимость: {CountryPolitics.AdviserAddCost} пв.";
        _tooltipHandler.SetAdviser(personage);
        var trait = personage.Traits.Find(tr => tr is AdviserTrait);
        if (trait != null)
        {
            _traitText.text = (trait as AdviserTrait).TraitName;
        }
        _button.onClick.AddListener(() => polticsUI.AddAdviser(personage));
    }

    public void Update()
    {
        _button.interactable = Player.CurrentCountry.Politics.PolitPower > CountryPolitics.AdviserAddCost;
    }
}
