using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Law", menuName = "ScriptableObjects/Law/Simple", order = 1)]
public class Law : ScriptableObject
{
    public List<ConstantEffect> LawEffects = new List<ConstantEffect>();
    public string Name;
    public string ID;
    public float PolitPowerCost;
    public Sprite LawImage;
}
