using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BuildingQueueSlotUI : MonoBehaviour
{
    public CountryBuildSlot BuildSlot { get; private set; }

    [SerializeField] private TextMeshProUGUI _buildingTypeNameText;
    [SerializeField] private Image _buildingImage;
    [SerializeField] private TextMeshProUGUI _buildingRegionNameText;
    [SerializeField] private Image _buildProgressFillImage;


    public void RefreshUI(CountryBuildSlot buildSlot)
    {
        BuildSlot = buildSlot;
        _buildingTypeNameText.text = buildSlot.Building.Name;
        _buildingImage.sprite = buildSlot.Building.BuildingImage;
        _buildingRegionNameText.text = buildSlot.BuildRegion.Name;
    }

    private void Update()
    {
        if (BuildSlot != null)
        {
            _buildProgressFillImage.fillAmount = BuildSlot.BuildProgress / BuildSlot.Building.BuildCost;
        }
    }
}
