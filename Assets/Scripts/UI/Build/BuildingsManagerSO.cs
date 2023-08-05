using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingsManager", menuName = "ScriptableObjects/Build/BuildingsManager", order = 1)]
public class BuildingsManagerSO : ScriptableObject
{
    public List<Building> AvalibleBuildings = new List<Building>();

    private static BuildingsManagerSO _instance;

  

    public static BuildingsManagerSO GetInstance()
    {
        if (_instance == null)
        {
            _instance = BuildingsManagerInstancer.GetInstance();
        }
        return _instance;
    }
}
