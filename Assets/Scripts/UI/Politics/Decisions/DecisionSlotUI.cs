using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionSlotUI : MonoBehaviour
{
    public Decision Target { get; set; }

    [SerializeField] private TextMeshProUGUI _decisionText;
    [SerializeField] private TextMeshProUGUI _decisionCostText;
    [SerializeField] private Button _activateButton;

    private void Start()
    {
        _decisionText.text = Target.Name;
        _decisionCostText.text = $"Стоимость: {Target.PolitPowerCost} полит. вл.";
        _activateButton.onClick.AddListener(delegate
        {
            Target.ActivaieDecision(Player.CurrentCountry);
            FindObjectOfType<PolticsUI>().RefreshUI();
        });
    }
}
