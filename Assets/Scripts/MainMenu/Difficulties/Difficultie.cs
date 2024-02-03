using UnityEngine;

[CreateAssetMenu(fileName = "Difficultie", menuName = "ScriptableObjects/Difficulties/Difficultie", order = 1)]
public class Difficultie : ScriptableObject
{
    public string Name;
    public string ID;
    public float PolitPowerBonusPercent;
    public float ResearchPointsBonusPercent;
    public float ProductionFactor;
    public CountryTrait AIBuffTrait;
}
