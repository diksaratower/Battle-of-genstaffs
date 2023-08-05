using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SelectedAviabaseAviationDivisionSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _aviationDivisionName;
    [SerializeField] private Button _slotButton;
    [SerializeField] private Outline _slotOutLine;
    [SerializeField] private Image _equipmentPercentFill;

    private AviationDivision _targetAviationDivision;
    private AviationModeUI _aviationUI;


    private void Update()
    {
        _slotOutLine.enabled = _aviationUI.GetSelectedAviationDivisions().Exists(division => division == _targetAviationDivision);
        _equipmentPercentFill.fillAmount = _targetAviationDivision.GetEquipmentProcent();
    }

    public void RefreshUI(AviationDivision aviationDivision, AviationModeUI aviationUI)
    {
        _targetAviationDivision = aviationDivision;
        _aviationUI = aviationUI;
        _aviationDivisionName.text = aviationDivision.Name;
        _slotButton.onClick.AddListener(delegate 
        {
            aviationUI.DeselectAllAviationDivisions();
            aviationUI.SelectAviationDivision(aviationDivision);
        });
    }
}
