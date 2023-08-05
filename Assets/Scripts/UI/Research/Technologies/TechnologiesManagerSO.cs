using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TechnologiesManager", order = 1)]
public class TechnologiesManagerSO : ScriptableObject
{
    public List<Technology> TechnologyList = new List<Technology>();
    public List<Battalion> AvailableBattalions = new List<Battalion>();

    private static TechnologiesManagerSO _instance;

    public static Technology GetTechFromID(string id)
    {
        return GetInstance().TechnologyList.Find(x => x.ID == id);
    }

    public static List<Technology> GetAllTechs()
    {
        var list = new List<Technology>();
        list.AddRange(GetInstance().TechnologyList);
        return list;
    }

    public static TechnologiesManagerSO GetInstance()
    {
        if (_instance == null)
        {
            _instance = TechnologiesManagerInstancer.GetInstance();
        }
        return _instance;
    }
}
