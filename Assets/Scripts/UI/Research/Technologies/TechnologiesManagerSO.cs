using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TechnologiesManager", order = 1)]
public class TechnologiesManagerSO : ScriptableObject
{
    public List<TechnologiesTree> TechnologiesTrees = new List<TechnologiesTree>();
    public List<Battalion> AvailableBattalions = new List<Battalion>();

    private static TechnologiesManagerSO _instance;

    public static Technology GetTechFromID(string id)
    {
        return GetAllTechs().Find(x => x.ID == id);
    }

    public static List<Technology> GetAllTechs()
    {
        var list = new List<Technology>();
        var trees = GetInstance().TechnologiesTrees;
        foreach (var tree in trees)
        {
            list.AddRange(tree.Technologies);
        }
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