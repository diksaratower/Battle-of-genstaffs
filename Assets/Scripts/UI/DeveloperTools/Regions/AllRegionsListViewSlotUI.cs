using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AllRegionsListViewSlotUI : MonoBehaviour
{
    [SerializeField] private Button _openRegionChangerButton;
    [SerializeField] private TextMeshProUGUI _openRegionChangerButtonText;


    public void RefreshUI(Region region)
    {
        _openRegionChangerButtonText.text = region.Name + " " + Map.Instance.MapRegions.IndexOf(region);
        _openRegionChangerButton.onClick.AddListener(delegate 
        {

        });
    }
}
