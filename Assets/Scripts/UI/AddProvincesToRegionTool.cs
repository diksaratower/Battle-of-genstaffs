using System.Collections.Generic;
using UnityEngine;


public class AddProvincesToRegionTool : MonoBehaviour
{
    [SerializeField] private Rect _windowRect = new Rect();
    [SerializeField] private GameIU _gameIU;
    [SerializeField] private bool _drawInside;
    [SerializeField] private BrushForDeveloperTools _brush = new BrushForDeveloperTools(3f);

    private List<Province> _provinces = new List<Province>();
    private Region _region;
    private string _regionID;
    private bool _selectingProvs;

    public void Update()
    {
        _gameIU.BlockDivisionSelecting = _selectingProvs;
        if (_selectingProvs)
        {
            _brush.UpdateBrushPosition();
            if (Input.GetKey(KeyCode.Mouse0))
            {
                var newProvinces = _brush.GetBrushProvinces(true);
                foreach (var province in newProvinces)
                {
                    if (_provinces.Contains(province) == false)
                    {
                        _provinces.Add(province);
                    }
                }
            }
            if (_regionID != "")
            {
                if (int.TryParse(_regionID, out int RegionIndex))
                {
                    _region = Map.Instance.MapRegions[RegionIndex];
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
        if (GUI.Button(new Rect(10, 20, 190, 20), "Set region"))
        {
            if (_region != null)
            {
                foreach (var province in _provinces)
                {
                    var regions = Map.Instance.MapRegions.FindAll(reg => reg.Provinces.Contains(province));//на всякий 
                    foreach (var region in regions)
                    {
                        region.Provinces.Remove(province);
                    }
                }
                foreach (var province in _provinces)
                {
                    if (_region.Provinces.Contains(province) == false)
                    {
                        _region.Provinces.Add(province);
                    }
                }
            }
            _provinces.Clear();
        }
        _selectingProvs = GUI.Toggle(new Rect(10, 40, 150, 20), _selectingProvs, "Is selecting");

        GUI.Label(new Rect(10, 60, 150, 20), "Reg ID:");
        _regionID = GUI.TextField(new Rect(10, 120, 150, 20), _regionID);
        if (_region != null)
        {
            GUI.Label(new Rect(10, 100, 210, 20), $"Region: {_region.Name}");
        }
        else
        {
            GUI.Label(new Rect(10, 100, 210, 20), $"Region:");
        }

        _brush.BrushSize = GUI.HorizontalSlider(new Rect(10, 180, 150, 20), _brush.BrushSize, 0, 20);

        if (GUI.Button(new Rect(10, 200, 190, 20), "Clear"))
        {
            _provinces.Clear();
        }

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));

    }

    private void OnDrawGizmos()
    {
        
        if (this.enabled == false)
        {
            return;
        }
        if (_region != null)
        {
            foreach (var province in _region.Provinces)
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawSphere(province.Position + Vector3.up, 0.5f);
            }
        }
        if (_selectingProvs)
        {
            Gizmos.color = Color.red;
            foreach (var province in _provinces)
            {
                Gizmos.DrawSphere(province.Position + Vector3.up, 0.5f);
            }
            _brush.DrawBrushGizmos();
        }
       
    }
}
