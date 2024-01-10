using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SelectedAviabaseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _aviabaseStatusText;
    [SerializeField] private Button _createAviationDivisionButton;
    [SerializeField] private SelectedAviabaseAviationDivisionSlotUI _aviationDivisionSlotUIPrefab;
    [SerializeField] private Transform _divisionsUISlotsParent;
    [SerializeField] private AviationRadiusViewUI _aviationRadiusViewPrefab;

    private List<SelectedAviabaseAviationDivisionSlotUI> _slotsUI = new List<SelectedAviabaseAviationDivisionSlotUI>();
    private List<AviationRadiusViewUI> _aviationRadiuses = new List<AviationRadiusViewUI>();
    private BuildingSlotRegion _targetAviabase;
    private AviationModeUI _aviationUI;


    private void Awake()
    {
        UnitsManager.Instance.OnCreateAviationDivision += delegate
        {
            if (_targetAviabase != null)
            {
                RefreshUI(_targetAviabase, _aviationUI);
            }
        };
        UnitsManager.Instance.OnRemoveAviationDivision += delegate
        {
            if (_targetAviabase != null)
            {
                RefreshUI(_targetAviabase, _aviationUI);
            }
        };
    }

    private void OnDisable()
    {
        ClearRadiuses();
    }

    public void RefreshUI(BuildingSlotRegion aviabase, AviationModeUI aviationUI)
    {
        if (aviabase.TargetBuilding.BuildingType != BuildingType.Airbase)
        {
            throw new ArgumentException();
        }
        _targetAviabase = aviabase;
        _aviationUI = aviationUI;
        var playerDivisions = UnitsManager.Instance.AviationDivisions.FindAll(division =>
(division.CountryOwner == Player.CurrentCountry && division.PositionAviabase == aviabase));
        _aviabaseStatusText.text = $"Авиабаза {aviabase.Region.Name} доступные части {playerDivisions.Count}.";
        _createAviationDivisionButton.onClick.RemoveAllListeners();
        _createAviationDivisionButton.onClick.AddListener(delegate 
        {
            var avibaseDivisionsCount = UnitsManager.Instance.AviationDivisions.FindAll(aviationDivision => aviationDivision.PositionAviabase == aviabase).Count;
            if (avibaseDivisionsCount < (aviabase.TargetBuilding as Airbase).BaseCapacity)
            {
                UnitsManager.Instance.AddAviationDivision(aviabase, Player.CurrentCountry);
            }
        });
        RefreshDivisionsInBase(playerDivisions, aviationUI);
        InstantiateRadiusesView(playerDivisions);
    }

    private void RefreshDivisionsInBase(List<AviationDivision> playerDivisions, AviationModeUI aviationUI)
    {
        _slotsUI.ForEach(slot => Destroy(slot.gameObject));
        _slotsUI.Clear();
        foreach (var aviationDivsion in playerDivisions)
        {
            var slotUI = Instantiate(_aviationDivisionSlotUIPrefab, _divisionsUISlotsParent);
            slotUI.RefreshUI(aviationDivsion, aviationUI);
            _slotsUI.Add(slotUI);
        }
    }

    private void ClearRadiuses()
    {
        _aviationRadiuses.ForEach((AviationRadiusViewUI slot) => 
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        });
        _aviationRadiuses.Clear();
    }

    private void InstantiateRadiusesView(List<AviationDivision> playerDivisions)
    {
        ClearRadiuses();
        foreach (var aviationDivision in playerDivisions)
        {
            if (_aviationRadiuses.Exists(radiusView => radiusView.TragetRadius == aviationDivision.AttackDistance) == false)
            {
                var radiusView = Instantiate(_aviationRadiusViewPrefab);
                radiusView.UpdateRadiusView(aviationDivision);
                aviationDivision.OnGetSupply += delegate
                {
                    if (radiusView != null)
                    {
                        radiusView.UpdateRadiusView(aviationDivision);
                    }
                };
                _aviationRadiuses.Add(radiusView);
            }
        }
    }
}
