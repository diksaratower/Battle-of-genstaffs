using UnityEngine;


[CreateAssetMenu(fileName = "PolitPowerGrowthAdviserEffect", menuName = "ScriptableObjects/Personages/Traits/Effects/PolitPowerGrowth", order = 1)]
public class PolitPowerGrowthTraitEffect : ConstantEffect
{
    public float PolitPowerGrothIncreaseProcent = 10;

    public float GetNeedIncreaseValue(float baseValue)
    {
        return (baseValue * (PolitPowerGrothIncreaseProcent / 100f));
    }

    public override string GetEffectDescription()
    {
        return $"Прирост полит. власти {GameIU.FloatToStringAddPlus(PolitPowerGrothIncreaseProcent)}%";
    }
}
