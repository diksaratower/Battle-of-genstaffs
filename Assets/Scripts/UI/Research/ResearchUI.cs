using System;
using TMPro;
using UnityEngine;


public class ResearchUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _researchPointsText;

    private Country _country;

    private void Start()
    {
        _country = Player.CurrentCountry;
    }


    private void Update()
    {
        _researchPointsText.text = "Очки исследования: " + Math.Round(_country.Research.ResearchPointCount, 2).ToString("0.00");
    }
}
