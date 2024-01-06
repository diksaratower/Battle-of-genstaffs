using System;
using System.Collections.Generic;
using UnityEngine;


public class MarineRegions : MonoBehaviour, ISaveble
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

    [SerializeField] private GameObject _regionsParent;
    [SerializeField] private LayerMask _marineLayerMask;
    [SerializeField] private List<ShipSO> _shipsSO = new List<ShipSO>();


    public void Initialize()
    {
        BuildingSlot.OnAddedBuilding += (BuildingSlot building) => 
        {
            if (building.TargetBuilding is NavyBase)
            {
                NavyBases.Add(building);
            }
        };
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
                AddProvinceToMarineRegion(province);
            }
        }
        _regionsParent.SetActive(false);
        MarineRegionsList.ForEach(region =>
        {
            region.SetUpCenter();
        });
    }

    private void AddProvinceToMarineRegion(Province province)
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

    public Ship AddShip(Ship ship)
    {
        if (ship.ShipPosition == null)
        {
            ship.ShipPosition = MarineRegionsList[0];
        }
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

    string ISaveble.GetFileName()
    {
        return "marine";
    }

    string ISaveble.Save()
    {
        return MarineRegionsSerialize.Save(this);
    }

    void ISaveble.Load(string data)
    {
        var ser = JsonUtility.FromJson<MarineRegionsSerialize>(data);
        ser.Load(this);
    }

    Type ISaveble.GetSaveType()
    {
        throw new NotImplementedException();
    }

    [Serializable]
    public class MarineRegionsSerialize
    {
        public List<ShipSerialize> Ships = new List<ShipSerialize>();

        public MarineRegionsSerialize(MarineRegions marineRegions) 
        {
            foreach (var ship in marineRegions.Ships)
            {
                Ships.Add(new ShipSerialize(ship.Name, ship.Country.ID, ship.ShipPosition.ID, ship.ShipTypeID));
            }
        }

        public void Load(MarineRegions marineRegions)
        {
            foreach (var shipSerialize in Ships)
            {
                var ship = marineRegions._shipsSO.Find(ship => ship.ID == shipSerialize.ShipID).CreateShip(Map.Instance.GetCountryFromId(shipSerialize.CountryID));
                marineRegions.AddShip(ship);
            }
        }

        public static string Save(MarineRegions marineRegions)
        {
            var ser = new MarineRegionsSerialize(marineRegions);
            return JsonUtility.ToJson(ser);
        }

        [Serializable]
        public class ShipSerialize
        {
            public string Name;
            public string CountryID;
            public string PostionSeaID;
            public string ShipID;

            
            public ShipSerialize(string name, string countryID, string positionSeaID, string shipID)
            {
                Name = name;
                CountryID = countryID;
                PostionSeaID = positionSeaID;
                ShipID = shipID;
            }
        }
    }
}
