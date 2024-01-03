using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTechnologyUI : MonoBehaviour
{
    public Technology TargetTechnology { get; private set; }

    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _alreadyResearchText;
    [SerializeField] private Image _techImage;
    [SerializeField] private Button _buyButton;

    private Country _country;


    private void Start()
    {
        _country = Player.CurrentCountry;
        _buyButton.onClick.AddListener(ByTech);
    }

    private void Update()
    {
        if(_country.Research.AlreadyResearched(TargetTechnology))
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
        var canBuy =  (_country.Research.ResearchPointCount - TargetTechnology.OpenCost) > 0;
        if (TargetTechnology.NeededTech.Count > 0)
        {
            if (TargetTechnology.NeededTech.FindAll(tech => _country.Research.AlreadyResearched(tech)).Count != TargetTechnology.NeededTech.Count)
            {
                canBuy = false;
            }   
        }
        _buyButton.interactable = canBuy;
    }

    public void RefreshUI(Technology technology)
    {
        TargetTechnology = technology;
        _costText.text = "Купить: " + TargetTechnology.OpenCost.ToString();
        _nameText.text = TargetTechnology.TechName.ToString();
        _techImage.sprite = TargetTechnology.TechImage;
    }

    private void ByTech()
    {
        if ((_country.Research.ResearchPointCount - TargetTechnology.OpenCost) > 0)
        {
            _country.Research.BuyTech(TargetTechnology);
        }
    }
}
