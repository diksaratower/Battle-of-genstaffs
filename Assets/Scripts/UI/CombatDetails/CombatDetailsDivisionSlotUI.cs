using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CombatDetailsDivisionSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _divisionNameText;
    [SerializeField] private TextMeshProUGUI _divisionAttackText;
    [SerializeField] private TextMeshProUGUI _divisionDefenseText;
    [SerializeField] private Image _divisionImage;
    [SerializeField] private Image _organizationField;

    private Division _division;

    public void RefreshUI(Division division, DivisionCombat combat)
    {
        _division = division;
        _divisionNameText.text = division.Name;
        _divisionAttackText.text = division.GetAttack().ToString();
        _divisionDefenseText.text = division.GetDefense().ToString();
        _divisionImage.sprite = division.DivisionAvatar;
        var tooltip = gameObject.AddComponent<CombatDetailsDivisionDataTooltipHandlerUI>();
        tooltip.Target = division;
        tooltip.TargetCombat = combat;
        tooltip.Initialize<CombatDetailsDivisionDataTooltipViewMenu>();
    }

    public void Update()
    {
        _organizationField.fillAmount = (_division.Organization / _division.MaxOrganization);
    }
}
