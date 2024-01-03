using System;
using TMPro;
using UnityEngine;


public class FleetMarineRegionUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _regionName;
    [SerializeField] private TextMeshProUGUI _procentDominationText;
    [SerializeField] private Color _neutralDominationColor;
    [SerializeField] private Color _ourDominationColor;
    [SerializeField] private Color _enemyDominationColor;

    private MarineRegion _target;
    private Vector3 _targetPosition;

    private void Update()
    {
        _rectTransform.anchoredPosition = GameCamera.Instance.WorldToScreenPointResolutionTrue(_targetPosition);
        CalculateProcentDomination();
    }

    public void SetTarget(MarineRegion marineRegion)
    {
        _target = marineRegion;
        _targetPosition = marineRegion.Center.position;

        _regionName.text = marineRegion.Name;
    }

    private void CalculateProcentDomination()
    {
        _target.IsDominate(Player.CurrentCountry, out var percentDomination);
        if (percentDomination == 0f)
        {
            _procentDominationText.color = _neutralDominationColor;
        }
        if (percentDomination > 0.5f)
        {
            _procentDominationText.color = _ourDominationColor;
        }
        if (percentDomination <= 0.5f && percentDomination != 0)
        {
            _procentDominationText.color = _enemyDominationColor;
        }
        _procentDominationText.text = Math.Round(percentDomination * 100, 2) + "%";
    }
}
