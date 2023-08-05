using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTechnologyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _alreadyResearchText;
    [SerializeField] private Image _techImage;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Technology _technology;

    private Country _country;
    private void Start()
    {
        _country = Player.CurrentCountry;
        RefreshUI();
        _buyButton.onClick.AddListener(ByTech);
    }

    private void Update()
    {
        if(_country.Research.AlreadyResearched(_technology))
        {
            _buyButton.gameObject.SetActive(false);
            _alreadyResearchText.gameObject.SetActive(true);
            return;
        }
        else
        {
            _buyButton.gameObject.SetActive(true);
            _alreadyResearchText.gameObject.SetActive(false) ;
        }
        var canBuy =  (_country.Research.ResearchPointCount - _technology.OpenCost) > 0;
        _buyButton.interactable = canBuy;
    }

    public void RefreshUI()
    {
        _costText.text = "Купить: " + _technology.OpenCost.ToString();
        _nameText.text = _technology.TechName.ToString();
        _techImage.sprite = _technology.TechImage;
    }

    private void ByTech()
    {
        if ((_country.Research.ResearchPointCount - _technology.OpenCost) > 0)
        {
            _country.Research.BuyTech(_technology);
        }
    }
}
