using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SelectedAviabaseAviationDivisionSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _aviationDivisionName;
    [SerializeField] private Button _slotButton;
    [SerializeField] private Outline _slotOutLine;
    [SerializeField] private Image _equipmentPercentFill;
    [SerializeField] private Image _divisionImage;
    [SerializeField] private Button _deleteAviationDivisionButton;

    private AviationDivision _targetAviationDivision;
    private AviationModeUI _aviationUI;


    private void Update()
    {
        _slotOutLine.enabled = _aviationUI.GetSelectedAviationDivisions().Exists(division => division == _targetAviationDivision);
        _equipmentPercentFill.fillAmount = _targetAviationDivision.GetEquipmentProcent();
        if (_targetAviationDivision.EquipmentInDivision.Count > 0)
        {
            if (_divisionImage.gameObject.activeSelf == false)
            {
                _divisionImage.gameObject.SetActive(true);
            }
            _divisionImage.sprite = _targetAviationDivision.GetAverageAirplane().EquipmentImage;
        }
        else
        {
            if (_divisionImage.gameObject.activeSelf == true)
            {
                _divisionImage.gameObject.SetActive(false);
            }
        }
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
        _deleteAviationDivisionButton.onClick.AddListener(delegate 
        {
            aviationDivision.RemoveDivision();
        });
    }
}
