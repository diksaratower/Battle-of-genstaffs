using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DifficultiesData", menuName = "ScriptableObjects/Difficulties/DifficultiesData", order = 1)]
public class DifficultiesData : ScriptableObject
{
    public List<Difficultie> Difficulties = new List<Difficultie>();
    public Difficultie StandartDifficultie;

    public static DifficultiesData GetInstance()
    {
        return Resources.Load<DifficultiesData>("DifficultiesData");
    }
}
