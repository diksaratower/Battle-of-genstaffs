using System;
using System.Collections.Generic;
using UnityEngine;


public class MarineRegions : MonoBehaviour
{
    public List<MarineRegion> MarineRegionsList = new List<MarineRegion>();
    public List<BuildingSlot> NavyBases = new List<BuildingSlot>();
    public List<Ship> Ships = new List<Ship>();
    public Action<Ship> OnCreateShip;
    public Action<Ship> OnRemoveShip;
    public bool ViewSelectedRegionProvinces;
    public bool ViewSelectedRegionContacts;
    public Material NeutralDominationColor;
    public Material OurDominationColor;
    public Material EnemyDominationColor;

    [SerializeField] private LayerMask _marineLayerMask;

    public void Initialize()
    {
        BuildingSlot.OnAddedBuilding += (BuildingSlot building) => 
        {
            if (building.TargetBuilding is NavyBase)
            {
                NavyBases.Add(building);
            }
        };
    }

    public void Start()
    {
        RecalculateMarineRegions();
    }

    private void Update()
    {
        if (ViewSelectedRegionContacts)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 10000f, _marineLayerMask))
                {
                    if (hit.collider.TryGetComponent<MarineRegion>(out var marineRegion))
                    {
                        foreach (var contact in marineRegion.Contacts)
                        {
                            Debug.DrawLine(marineRegion.transform.position, contact.transform.position, Color.red);
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (ViewSelectedRegionProvinces)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 10000f, _marineLayerMask))
                {
                    if (hit.collider.TryGetComponent<MarineRegion>(out var marineRegion))
                    {
                        foreach (var province in marineRegion.Provinces)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(province.Position, 0.7f);
                        }
                    }
                }
            }
        }
    }

    public void RecalculateMarineRegions()
    {
        foreach (var province in Map.Instance.Provinces)
        {
            if (province.Contacts.Count < 6)
            {
                var hex = GenerateHex(8);
                foreach (var point in hex)
                {
                    if (Physics.Raycast(new Ray(point + province.Position + (Vector3.up * 100), Vector3.down), out var hit, 1000f, _marineLayerMask))
                    {
                        if (hit.collider.TryGetComponent<MarineRegion>(out var marineRegion))
                        {
                            if (marineRegion.Provinces.Contains(province) == false)
                            {
                                marineRegion.Provinces.Add(province);
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }

    public Ship AddShip(Ship ship)
    {
        Ships.Add(ship);
        OnCreateShip?.Invoke(ship);
        return ship;
    }

    public void RemoveShip(Ship ship)
    {
        Ships.Remove(ship);
        OnRemoveShip?.Invoke(ship);
    }

    private List<Vector3> GenerateHex(float radius)
    {
        var vertecs = new List<Vector3>();
        for (int i = 0; i < 6; i++)
        {
            var q = new Quaternion();
            q.eulerAngles = new Vector3(60 * i, 0, 0);
            var vect = Matrix4x4.Rotate(q).MultiplyVector(Vector3.forward * radius);
            vertecs.Add(Matrix4x4.Rotate(new Quaternion() { eulerAngles = new Vector3(0, 0, 90) }).MultiplyVector(vect));
        }
        return vertecs;
    }
}
