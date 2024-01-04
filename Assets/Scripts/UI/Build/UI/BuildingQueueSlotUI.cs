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
    [SerializeField] private Button _removeQueueSlot;


    public void RefreshUI(CountryBuildSlot buildSlot, CountryBuild countryBuild)
    {
        BuildSlot = buildSlot;
        _buildingTypeNameText.text = buildSlot.Building.Name;
        _buildingImage.sprite = buildSlot.Building.BuildingImage;
        _buildingRegionNameText.text = buildSlot.BuildRegion.Name;
        _removeQueueSlot.onClick.AddListener(delegate 
        {
            countryBuild.RemoveSlotFromBuildQueue(buildSlot);
        });
    }

    private void Update()
    {
        if (BuildSlot != null)
        {
            _buildProgressFillImage.fillAmount = BuildSlot.BuildProgress / BuildSlot.Building.BuildCost;
        }
    }
}
