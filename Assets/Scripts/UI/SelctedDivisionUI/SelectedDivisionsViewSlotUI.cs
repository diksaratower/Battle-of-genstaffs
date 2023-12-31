using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SelectedDivisionsViewSlotUI : MonoBehaviour
{
    public Division Division { get; private set; }

    [SerializeField] private TextMeshProUGUI _divisionNameText;
    [SerializeField] private Image _divisionImage;
    [SerializeField] private Image _organizationField;
    [SerializeField] private Image _equipmentField;
    [SerializeField] private Button _onClickButton;
    [SerializeField] private Button _openDivisionDetailsWindow;


    private void Update()
    {
        _divisionNameText.text = Division.Name;
        _divisionImage.sprite = Division.DivisionAvatar;
        _organizationField.fillAmount = (Division.Organization / Division.MaxOrganization);
        _equipmentField.fillAmount = Division.GetBattleStrengh();
    }

    public void RefreshUI(Division division, GameIU gameIU)
    {
        Division = division;
        _onClickButton.onClick.AddListener(delegate 
        {
            gameIU.DeselectedAllDivisions();
            gameIU.SelectDivisions(new List<Division>(1) { Division });
        });
        _openDivisionDetailsWindow.onClick.AddListener(delegate 
        {
            var divisionDescriptionUI = FindObjectOfType<DivisionDescriptionUI>();
            divisionDescriptionUI.gameObject.SetActive(true);
            divisionDescriptionUI.RefreshUI(division);
        });
    }
}
