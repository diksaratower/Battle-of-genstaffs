using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DecisionSlotUI : MonoBehaviour, IDecisionsUIViewSlot
{
    [SerializeField] private TextMeshProUGUI _decisionText;
    [SerializeField] private TextMeshProUGUI _decisionCostText;
    [SerializeField] private Button _activateButton;


    public void RefreshUI(Decision decision, PolticsUI politicsUI)
    {
        _decisionText.text = decision.Name;
        _decisionCostText.text = $"Стоимость: {decision.PolitPowerCost} полит. вл.";
        _activateButton.onClick.AddListener(delegate
        {
            Player.CurrentCountry.Politics.DoDecision(decision);
            politicsUI.RefreshUI();
        });
    }

    GameObject IDecisionsUIViewSlot.GetSlotGO()
    {
        return gameObject;
    }
}
