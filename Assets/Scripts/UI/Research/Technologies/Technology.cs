using System.Collections.Generic;
using UnityEngine;


public abstract class Technology : ScriptableObject
{
    public float OpenCost = 1000;
    public int OptimalDate = 1936;
    public List<Technology> NeededTech = new List<Technology>();
    public abstract Sprite TechImage { get; } 
    public abstract string TechName { get; }
    public abstract string ID { get; }
}
