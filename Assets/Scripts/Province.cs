using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Province
{
    public List<Province> Contacts => GetContacts();
    public Vector2Int PositionInTexture => PositionInGrid;
    public Country Owner { get; private set; }
    public Vector2Int PositionInGrid { get;}
    public int ID { get; }
    public Vector3 Position { get; }
    public Vector3 LandscapeTruePosition { get; }

    public List<Vector3> Vertices = new List<Vector3>();
    public List<int> ContactsIDs = new List<int>();
    public List<Division> DivisionsInProvince = new List<Division>();

    public Province(Vector2Int positionInGrid, int id, Vector3 position, Vector3 landscapeTruePosition) 
    {
        PositionInGrid = positionInGrid;
        ID = id;
        Position = position;
        LandscapeTruePosition = landscapeTruePosition;
    }

    public bool AllowedForDivision(Division division)
    {
        if(division.CountyOwner.ProvinceAllowedForCountryArmy(this))
        {
            return true;
        }
        return false;
    }

    public bool FriendlyForDivision(Division division)
    {
        if(division.CountyOwner == Owner)
        {
            return true;
        }
        return false;
    }

    public void OnDivisonEnter(Division division)
    {
        if (Owner != division.CountyOwner)
        {
            SetOwner(division.CountyOwner);
        }
        DivisionsInProvince.Add(division);
        division.OnDivisionEnterToProvince += (Province province) =>
        {
            if (province != this)
            {
                DivisionsInProvince.Remove(division);
            }
        };
    }

    public void SetOwner(Country newOwner)
    {
        Owner = newOwner;
        Map.Instance.ColoredProvince(this);
    }

    private List<Province> GetContacts()
    {
        var contacts = new List<Province>();
        foreach (var conId in ContactsIDs)
        {
            contacts.Add(Map.Instance.GetProvinceById(conId));
        }
        return contacts;
    }

    public List<Vector3> GetIntersectionsPoints(List<Province> provinces)
    {
        var points = new List<Vector3>();
        foreach (var province in provinces)
        {
            points.AddRange(GetIntersectionsPoints(province));
        }
        return points;
    }

    public List<Vector3> GetIntersectionsPoints(Province province)
    {
        var points = new List<Vector3>();
        foreach (var vertOne in Vertices)
        {
            foreach (var vertTwo in province.Vertices)
            {
                if (Vector2.Distance(new Vector2(vertOne.x, vertOne.z), new Vector2(vertTwo.x, vertTwo.z)) < 0.3f)
                {
                    points.Add(vertOne);
                }
            }
        }
        return points;
    }
}