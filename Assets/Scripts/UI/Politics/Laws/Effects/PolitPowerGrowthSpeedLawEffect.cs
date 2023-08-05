using UnityEngine;


[CreateAssetMenu(fileName = "PolitPowerGrowthSpeed", menuName = "ScriptableObjects/Law/Effects/PolitPowerGrowthSpeed", order = 1)]
public class PolitPowerGrowthSpeedLawEffect : LawEffect
{
    public float PolitPowerGrothIncrease = 0.1f;

    public override string GetEffectDescription()
    {
        return "";
    }
}
