using UnityEngine;


[CreateAssetMenu(fileName = "ChangeStabilityEffect", menuName = "ScriptableObjects/Personages/Traits/Effects/ChangeStability", order = 1)]
public class ChangeStabilityTraitEffect : ConstantEffect
{
    public float ChangeStabilityProcent = 10f;

    public float GetNeedIncreaseValue(float baseValue)
    {
        return (baseValue * (ChangeStabilityProcent / 100f));
    }

    public override string GetEffectDescription()
    {
        return $"Устойчивость режима {GameIU.FloatToStringAddPlus(ChangeStabilityProcent)}%";
    }
}
