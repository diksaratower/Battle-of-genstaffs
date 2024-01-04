using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TechnologiesTree", menuName = "ScriptableObjects/Technologies/TechnologiesTree", order = 1)]
public class TechnologiesTree : ScriptableObject
{
    public List<Technology> Technologies = new List<Technology>();
    public Technology BaseTechnology;
    public TechnologyCategory Category;
}

public enum TechnologyCategory
{
    Infantry,
    Tanks,
    Aviation, 
    Fleet
}