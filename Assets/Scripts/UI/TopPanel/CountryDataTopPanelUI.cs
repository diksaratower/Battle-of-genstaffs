using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CountryDataTopPanelUI : MonoBehaviour
{
    [SerializeField] private Image _currebtCountryFlag;
    [SerializeField] private TextMeshProUGUI _politPowerText;
    [SerializeField] private TextMeshProUGUI _manPowerText;
    [SerializeField] private TextMeshProUGUI _militaryFactoriesCountText;
    [SerializeField] private TextMeshProUGUI _simpleFactoriesCountText;
    [SerializeField] protected TextMeshProUGUI _stabilityText;

    private void Update()
    {
        _currebtCountryFlag.sprite = Player.CurrentCountry.Flag;
        _politPowerText.text = "Полит. власть: " + Mathf.Round(Player.CurrentCountry.Politics.PolitPower);
        _manPowerText.text = "Резерв: " + ToMillions(Player.CurrentCountry.EquipmentStorage.GetEquipmentCount(EquipmentType.Manpower)) + "млн чел.";
        _militaryFactoriesCountText.text = "Военых заводов: " + Player.CurrentCountry.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory).Count;
        _simpleFactoriesCountText.text = "Обычных заводов: " + Player.CurrentCountry.CountryBuild.GetCountryBuildings(BuildingType.Factory).Count;
        _stabilityText.text = $"Устойчивость режима: {Player.CurrentCountry.Politics.CalculateStability()} %";
        //ToMillions(Player.CurrentCountry.EquipmentStorage.GetEquipmentCount(EquipmentType.Manpower))
    }

    private float ToMillions(float value)
    {
        return (float)System.Math.Round((float)(value / 1000000), 2);
    }
}
