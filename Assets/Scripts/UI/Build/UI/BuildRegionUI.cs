using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildRegionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _regionNameText;
    [SerializeField] private TextMeshProUGUI _buildingsCountText;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _inProgressBuildingsCountText;

    private Region _target;
    private Vector3 _targetPosition;

    private void Update()
    {
        _rectTransform.anchoredPosition = GameCamera.Instance.WorldToScreenPointResolutionTrue(_targetPosition);
        if (_target != null)
        {
            _buildingsCountText.text = $"{_target.GetAllBuildingsCount()}/{_target.MaxBuildingsCount}";
        }
    }

    public void RefreshUI(Region region, CountryBuild countryBuild)
    {
        _target = region;
        _regionNameText.text = region.Name;
        _targetPosition = _target.GetProvincesAveragePostion();
        Action updateProgressBuildingsCountDelegate = delegate
        {
            UpdateProgressBuildingsCount(countryBuild);
        };
        countryBuild.OnAddedBuildingToQueue += updateProgressBuildingsCountDelegate;
        countryBuild.OnRemovedBuildingFromQueue += updateProgressBuildingsCountDelegate;
        UpdateProgressBuildingsCount(countryBuild);
    }

    private void UpdateProgressBuildingsCount(CountryBuild countryBuild)
    {
        var bouildingsInProcessCount = countryBuild.BuildingsQueue.FindAll(slot => slot.BuildRegion == _target).Count;
        if (bouildingsInProcessCount > 0)
        {
            _inProgressBuildingsCountText.gameObject.SetActive(true);
            _inProgressBuildingsCountText.text = "+" + bouildingsInProcessCount.ToString();
        }
        else
        {
            _inProgressBuildingsCountText.gameObject.SetActive(false);
        }
    }
}
