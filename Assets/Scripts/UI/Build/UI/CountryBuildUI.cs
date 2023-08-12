using System;
using System.Collections.Generic;
using UnityEngine;

public class CountryBuildUI : MonoBehaviour
{
    public Action<AvailableBuildingSlotUI> OnChangeSelectionBuilding;
    public AvailableBuildingSlotUI SelectedBuilding { get; private set; }

    [SerializeField] private Color _boardViewColor;
    [SerializeField] private Color _insideViewColor;
    [SerializeField] private Color _insideViewColorInBuildProcess;
    [SerializeField] private Transform _regionUIsParent;
    [SerializeField] private BuildRegionUI _regionUIPrefab;
    [SerializeField] private Transform _queueSlotsParent;
    [SerializeField] private BuildingQueueSlotUI _queueSlotUIPrefab;
    [SerializeField] private Transform _availableBuildingsUIParent;
    [SerializeField] private AvailableBuildingSlotUI _availableSlotUIPrefab;
    [SerializeField] private RegionMeshViewBuildingUI _regionMeshViewPrefab;

    private List<RegionMeshViewBuildingUI> _regionMeshViews = new List<RegionMeshViewBuildingUI>();
    private List<AvailableBuildingSlotUI> _availableBuildingsSlotsUI = new List<AvailableBuildingSlotUI>();
    private List<BuildingQueueSlotUI> _queueSlotsUI = new List<BuildingQueueSlotUI>();
    private Country _country => Player.CurrentCountry;


    private void Start()
    {
        RefreshUI();
        _country.CountryBuild.OnAddedBuildingToQueue += () => RefreshQueueUI();
        _country.CountryBuild.OnRemovedBuildingFromQueue += () => RefreshQueueUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            AddNewBuildings();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RemoveBuildings();
        }
    }

    public void SetSelectedBuilding(AvailableBuildingSlotUI newSelectingBuilding)
    {
        if (SelectedBuilding != null)
        {
            if ((SelectedBuilding.Target is BuildingInProvince) != (newSelectingBuilding.Target is BuildingInProvince))
            {
                SelectedBuilding = newSelectingBuilding;
                DrawRegionsMeshes();
            }
        }
        SelectedBuilding = newSelectingBuilding;
        OnChangeSelectionBuilding?.Invoke(newSelectingBuilding);
    }

    public void RefreshUI()
    {
        RefreshAvailableBuildingsUI();
        DrawRegionsMeshes();
        RefreshQueueUI();
    }

    private void OnDisable()
    {
        DeleteRegionMeshes();
    }

    private void DrawRegionsMeshes()
    {
        DeleteRegionMeshes();
        if (SelectedBuilding.Target is not BuildingInProvince)
        {
            foreach (var region in Map.Instance.MapRegions)
            {
                if (region.Provinces[0].Owner == _country)
                {
                    var regionUI = Instantiate(_regionUIPrefab, _regionUIsParent);
                    regionUI.RefreshUI(region, _country.CountryBuild);
                    DrawProvincesMeshWithBoard(region, region.Provinces);
                }
            }
        }
        if (SelectedBuilding.Target is BuildingInProvince)
        {
            DrawMeshesForProvinceBuilding();   
        }
    }

    private void DrawMeshesForProvinceBuilding()
    {
        foreach (var region in Map.Instance.MapRegions)
        {
            if (region.Provinces[0].Owner == _country)
            {
                foreach (var province in region.Provinces)
                {
                    if (province.Buildings.Count > 0)
                    {
                        foreach (var building in province.Buildings)
                        {
                            if (building.TargetBuilding == SelectedBuilding.Target)
                            {
                                DrawProvincesMeshWithBoard(region, new List<Province>() { province });
                            }
                        }
                    }
                }
            }
        }
        foreach (var slot in _country.CountryBuild.BuildingsQueue)
        {
            if (slot.Building == SelectedBuilding.Target)
            {
                DrawProvincesMeshWithBoard(slot.BuildRegion, new List<Province>() { slot.BuildingProvince });
            }
        }
    }

    private void DrawProvincesMeshWithBoard(Region region, List<Province> provinces)
    {
        var regionMeshUI = Instantiate(_regionMeshViewPrefab);
        regionMeshUI.RefreshView(region, provinces, _insideViewColor, _insideViewColorInBuildProcess, _country.CountryBuild);
        _regionMeshViews.Add(regionMeshUI);
    }

    private void DeleteRegionMeshes()
    {
        _regionMeshViews.ForEach(regMesh =>
        {
            if (regMesh) 
            {
                Destroy(regMesh.gameObject); 
            }
        });
        _regionMeshViews.Clear();
    }

    private void RefreshAvailableBuildingsUI()
    {
        _availableBuildingsSlotsUI.ForEach(availableSlotUI => Destroy(availableSlotUI.gameObject));
        _availableBuildingsSlotsUI.Clear();
        var oneSlot = true;
        foreach (var avalibleBuildings in BuildingsManagerSO.GetInstance().AvalibleBuildings)
        {
            var slot = Instantiate(_availableSlotUIPrefab, _availableBuildingsUIParent);
            slot.RefreshUI(avalibleBuildings, this);
            _availableBuildingsSlotsUI.Add(slot);
            if (oneSlot == true)
            {
                SetSelectedBuilding(slot);
                oneSlot = false;
            }
        }
    }

    private void RefreshQueueUI()
    {
        _queueSlotsUI.ForEach(slot => Destroy(slot.gameObject));
        _queueSlotsUI.Clear();
        foreach (var queueSlot in _country.CountryBuild.BuildingsQueue)
        {
            var slot = Instantiate(_queueSlotUIPrefab, _queueSlotsParent);
            slot.RefreshUI(queueSlot);
            _queueSlotsUI.Add(slot);
        }
    }

    private void AddNewBuildings()
    {
        if (SelectedBuilding.Target is not BuildingInProvince)
        {
            AddRegionBuilding();
        }
        if (SelectedBuilding.Target is BuildingInProvince)
        {
            AddProvinceBuilding();
        }
    }

    private void AddRegionBuilding()
    {
        if (Physics.Raycast(GameCamera.Instance.GCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out RegionMeshViewBuildingUI regionView))
            {
                if (SelectedBuilding != null)
                {
                    if (_country.CountryBuild.CanAddBuildingToQueue(SelectedBuilding.Target, regionView.RegionTarget))
                    {
                        _country.CountryBuild.AddBuildingToBuildQueue(SelectedBuilding.Target, regionView.RegionTarget);
                    }
                }
            }
        }
    }

    private void AddProvinceBuilding()
    {
        if (GameCamera.Instance.ChekHitToProvinceWithMousePosition(out var province))
        {
            if (province.Owner == _country)
            {
                var region = Region.GetProvinceRegion(province);
                if ((SelectedBuilding.Target as BuildingInProvince).CanBuildInProvince(province))
                {
                    if (_country.CountryBuild.CanAddBuildingToQueue(SelectedBuilding.Target, region))
                    {
                        _country.CountryBuild.AddBuildingToBuildQueue(SelectedBuilding.Target, region, province);
                        DrawRegionsMeshes();
                    }
                }
            }
        }
    }

    private void RemoveBuildings()
    {
        if (SelectedBuilding.Target is not BuildingInProvince)
        {
            RemoveRegionBuilding();
        }
    }

    private void RemoveRegionBuilding()
    {
        if (Physics.Raycast(GameCamera.Instance.GCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out RegionMeshViewBuildingUI regionView))
            {
                if (SelectedBuilding != null)
                {
                    var slots = _country.CountryBuild.BuildingsQueue.FindAll(slot => (slot.BuildRegion == regionView.RegionTarget) && (slot.Building == SelectedBuilding.Target));
                    if (slots.Count > 0)
                    {
                        _country.CountryBuild.RemoveSlotFromBuildQueue(slots[0]);
                    }
                }
            }
        }
    }

    private void RemoveProvinceBuilding()
    {

    }
}