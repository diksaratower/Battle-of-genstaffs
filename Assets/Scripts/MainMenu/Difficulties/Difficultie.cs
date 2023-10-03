using UnityEngine;

[CreateAssetMenu(fileName = "Difficultie", menuName = "ScriptableObjects/Difficulties/Difficultie", order = 1)]
public class Difficultie : ScriptableObject
{
    public string Name;
    public float PolitPowerBonusPercent;
}
