using UnityEngine;


[CreateAssetMenu(fileName = "BuildSpeedAdviserEffect", menuName = "ScriptableObjects/Personages/Traits/Effects/BuildSpeed", order = 1)]
public class BuildSpeedTraitEffect : ConstantEffect
{
    public float BuildSpeedIncreaseProcent = 10f;

    public float GetNeedIncreaseValue(float baseValue)
    {
        return (baseValue * (BuildSpeedIncreaseProcent / 100f));
    }

    public override string GetEffectDescription()
    {
        return $"Скорость строительства +{BuildSpeedIncreaseProcent}%";
    }
}
