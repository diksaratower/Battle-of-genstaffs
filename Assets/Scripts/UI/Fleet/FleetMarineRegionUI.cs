using UnityEngine;


public class FleetMarineRegionUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    private MarineRegion _target;
    private Vector3 _targetPosition;

    private void Update()
    {
        _rectTransform.anchoredPosition = GameCamera.Instance.WorldToScreenPointResolutionTrue(_targetPosition);
    }

    public void SetTarget(MarineRegion marineRegion)
    {
        _target = marineRegion;
        _targetPosition = marineRegion.Center.position;
    }
}
