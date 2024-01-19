using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PoliticsDataSO", menuName = "ScriptableObjects/PoliticsDataSO", order = 1)]
public class PoliticsDataSO : ScriptableObject
{
    public List<Personage> Personages = new List<Personage>();
    public List<Personage> MinorLeaders = new List<Personage>();
    public List<Decision> StandartDecisions = new List<Decision>();

    private static PoliticsDataSO _instance;


    public static PoliticsDataSO GetInstance()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<PoliticsDataSO>("PoliticsDataSO");
        }
        return _instance;
    }
}
