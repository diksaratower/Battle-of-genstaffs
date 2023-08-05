using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Technology : ScriptableObject
{
    public float OpenCost = 1000;
    public int OptimalDate = 1936;
    public virtual Sprite TechImage { get; set; } 
    public virtual string TechName { get => _techName; set => _techName = value; }
    public virtual string ID { get => _id; set => _id = value; }

    [SerializeField] protected string _techName = "*use_equipment";
    [SerializeField] protected Sprite _techImage;
    [SerializeField] protected string _id = "*use_equipment";
}
