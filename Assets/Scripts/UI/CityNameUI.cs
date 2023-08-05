using TMPro;
using UnityEngine;

public class CityNameUI : MonoBehaviour
{
    public City Target { get; set; }

    [SerializeField] private TextMeshProUGUI _cityName;
    
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _cityName.text = Target.Name;
        _rectTransform.anchoredPosition = GameCamera.Instance.WorldToScreenPointResolutionTrue(Target.CityProvince.Position + Vector3.up);
    }
}
