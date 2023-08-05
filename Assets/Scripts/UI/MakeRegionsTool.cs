using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MakeRegionsTool : MonoBehaviour
{
    [SerializeField] private Rect _windowRect = new Rect();
    [SerializeField] private List<Province> Provinces = new List<Province>();
    [SerializeField] private GameIU _gameIU;
    [SerializeField] private bool _drawInside;
    [SerializeField] private BrushForDeveloperTools _brush = new BrushForDeveloperTools(3f);

    private string _countryTag;
    private string _regionName;
    private bool _selectingProvs;
    private bool _drawOtherRegions = false;

    private void Update()
    {
        _gameIU.BlockDivisionSelecting = _selectingProvs;
        if (_selectingProvs)
        {
            _brush.UpdateBrushPosition();
            if (Input.GetKey(KeyCode.Mouse1))
            {
                var newProvinces = _brush.GetBrushProvinces();
                foreach (var province in newProvinces)
                {
                    if (province.Owner.ID == _countryTag || _countryTag == "")
                    {
                        if (!Provinces.Contains(province))
                        {
                            if (!Map.Instance.MapRegions.Exists(reg => reg.Provinces.Contains(province)))
                            {
                                Provinces.Add(province);
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnGUI()
    {
        _windowRect = GUI.Window(0, _windowRect, DoMyWindow, "Regions create");
    }

    private void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(10, 20, 190, 20), "Clear"))
        {
            Provinces.Clear();
        }
        _selectingProvs = GUI.Toggle(new Rect(10, 40, 150, 20), _selectingProvs, "Is selecting");
        GUI.Label(new Rect(10, 60, 150, 20), "Select country tag:");
        _countryTag = GUI.TextField(new Rect(10, 80, 100, 20), _countryTag);

        GUI.Label(new Rect(10, 100, 180, 20), "Region name:");
        _regionName = GUI.TextField(new Rect(10, 120, 150, 20), _regionName);


        if (GUI.Button(new Rect(10, 140, 150, 20), "Create region"))
        {
            var region = new Region(_regionName, 0);
            region.Provinces = Provinces;
            Map.Instance.MapRegions.Add(region);
            Provinces = new List<Province>();
        }

        _brush.BrushSize = GUI.HorizontalSlider(new Rect(10, 180, 150, 20), _brush.BrushSize, 0, 20);

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));

    }

    private void OnDrawGizmos()
    {
        if(this.enabled == false) 
        {
            return;
        }
        if (_selectingProvs) 
        {
            Gizmos.color = Color.red;
            foreach (var province in Provinces) 
            {
                Gizmos.DrawSphere(province.Position + Vector3.up, 0.5f);
            }
            _brush.DrawBrushGizmos();
        }
        if (Map.Instance != null)
        {
            if (_drawOtherRegions)
            {
                var allRegionsProvs = new List<Province>();
                foreach (var reg in Map.Instance.MapRegions)
                {
                    if (reg.GetRegionCountry().ID == _countryTag)
                    {
                        continue;
                    }
                    allRegionsProvs.AddRange(reg.Provinces);
                }
                var board = allRegionsProvs.FindAll(prov => prov.Contacts.Exists(con => GetProvinceRegion(con) != GetProvinceRegion(prov)));
                var inside = allRegionsProvs.FindAll(pr => !board.Contains(pr));
                Gizmos.color = Color.gray;
                foreach (var province in board)
                {
                    if (province.Owner.ID == _countryTag)
                    {
                        Gizmos.DrawSphere(province.Position + Vector3.up, 0.5f);
                    }
                }
                if (_drawInside)
                {

                    Gizmos.color = Color.white;
                    foreach (var province in inside)
                    {
                        if (province.Owner.ID == _countryTag)
                        {
                            Gizmos.DrawSphere(province.Position + Vector3.up, 0.3f);
                        }
                    }
                }
            }
        }
    }

    private Region GetProvinceRegion(Province province)
    {
        foreach (var reg in Map.Instance.MapRegions)
        {
            if (reg.Provinces.Contains(province))
            {
                return reg;
            }
        }
        return null;
    }
}

[Serializable]
public class BrushForDeveloperTools
{
    public Vector3 BrushPosition { get; private set; }
    public float BrushSize;

    public BrushForDeveloperTools(float brushSize)
    {
        BrushSize = brushSize;
    }

    public void UpdateBrushPosition()
    {
        if (Physics.Raycast(GameCamera.Instance.GCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.GetComponent<TerrainCollider>())
            {
                BrushPosition = hit.point;
            }
        }
    }

    public List<Province> GetBrushProvinces(bool ignoreRegion = false)
    {
        var provinces = new List<Province>();
        if (GameCamera.Instance.ChekProvincesWithRadius(out var newProvinces, BrushSize))
        {
            foreach (var province in newProvinces)
            {
                if (!provinces.Contains(province))
                {
                    if (ignoreRegion || (!Map.Instance.MapRegions.Exists(reg => reg.Provinces.Contains(province))))
                    {
                        provinces.Add(province);
                    }
                }
            }
        }
        return provinces;
    }

    public void DrawBrushGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(BrushPosition + Vector3.up, BrushSize);
    }
}